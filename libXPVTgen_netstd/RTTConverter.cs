using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using libXPVTgen.acftSim;
using libXPVTgen.coordlib;
using libXPVTgen.LiveTraffic;

namespace libXPVTgen
{
  /// <summary>
  /// Converts an RTT stream to a route script
  /// </summary>
  public class RTTConverter
  {

    public string Error { get; private set; } = "";
    /// <summary>
    /// Converts from a list of RTTraffic messages to a route
    /// returns null if something was wrong
    /// </summary>
    /// <param name="aitStream">A list of RTTraffic messages as strings</param>
    /// <returns>A created route or null</returns>
    private CmdList RouteFromRTT( List<string> aitStream, bool absoluteTrack, bool ignoreAirborne )
    {
      if (aitStream.Count < 2) { Error = "# messages 2"; return null; }; // we don't handle below 3 reports

      var vac = RealTraffic.FromRTTrafficString( aitStream[0] );

      if (vac == null) { Error = "RTT message conversion error"; return null; }; // converter error
      long cTs = vac.TStamp; // start time,
      var cPos = new LatLon( vac.LatLon );
      double cAlt = vac.Alt_ft;
      double cGs = vac.GS;
      double cVsi = vac.VSI;
      double cTrack = vac.TRK;

      if (cGs == 0) {
        //Initial Speed is Zero - we may not have GS at all so try to calculate some..
        // try to calculate a GS
        var vac1 = RealTraffic.FromRTTrafficString( aitStream[1] );
        if (vac1 == null) return null; // ERROR Exit, this one is not usable at all
        cGs = cPos.DistanceTo( vac1.LatLon, ConvConsts.EarthRadiusNm ) * 3600.0 / (vac1.TStamp - vac.TStamp); // let's try the calculated one..
      };
      if (cGs == 0) {
        //Still Speed is Zero - we may not have GS at all so try to calculate some..
        Error = "Speed is not available, cannot continue";
        return null;
      }

      double maxGS = cGs;  // track Max Speed to adjust for the AcftType later

      var route = new CmdList( );
      CmdA a = null;

      if (absoluteTrack) {
        a = new CmdA( vac.AcftType, CmdA.FlightT.MsgAbsolute );
        a.CreateForMsgAbsolute( cPos, cTrack, cAlt, cGs );
      }
      else {
        a = new CmdA( vac.AcftType, CmdA.FlightT.MsgRelative );
        a.CreateForMsgRelative( cAlt, cGs );
      }

      route.Enqueue( a );
      // walk through route
      for (int i = 1; i < aitStream.Count; i++) {
        if (string.IsNullOrEmpty( aitStream[i] )) continue; // empty CRLFs ??
        vac = RealTraffic.FromAITrafficString( aitStream[i] );
        if (vac == null) {
          Error = "AIT message conversion error"; return null;
        }; // converter error
        if ((!ignoreAirborne) && (!vac.Airborne)) continue; // catch some incomplete ones

        // see what delta we have
        long dT = vac.TStamp - cTs;
        cTs = vac.TStamp;  // update time

        if (vac.GS == 0) {
          // try to calculate a GS ( but this does not account for turns - so it is likely too low
          vac.GS = cPos.DistanceTo( vac.LatLon, ConvConsts.EarthRadiusNm ) * 3600.0 / dT; // let's try the calculated one..
        }
        if (vac.GS < 5) continue; // plane is not moving

        double dAlt = vac.Alt_ft - cAlt;
        double dGs = vac.GS - cGs;
        double dTrk = vac.TRK - cTrack;
        double dVsi = vac.VSI - cVsi;
        maxGS = (vac.GS > cGs) ? vac.GS : cGs;

        // change speed if delta is large enough
        // don't collect speeds under 5 else the model does not move anymore..
        if ((vac.GS >= 5) && Math.Abs( dGs ) > 4) {
          route.Enqueue( new CmdS( vac.GS ) );
          cGs = vac.GS; // update if used
        }
        // change Alt if delta is large enough
        if (Math.Abs( dAlt ) > 25) {
          if (dAlt > 0 && cVsi < 0) cVsi = 1200; // wrong sign, just apply a VSI
          if (dAlt < 0 && cVsi > 0) cVsi = 1200; // wrong sign, just apply a VSI
          route.Enqueue( new CmdV( cVsi, vac.Alt_ft ) ); // we change at old VSI as this was the start to get to the new alt
          cAlt = vac.Alt_ft; // update if used
          cVsi = vac.VSI; // update if used
        }
        // Goto Pos
        if (absoluteTrack) {
          //just goto absolute location (let the model do the flying..)
          route.Enqueue( new CmdG( vac.LatLon ) );
        }
        else {
          // OR calc Direction and Distance for relative flying..
          route.Enqueue( new CmdH( cPos.BearingTo( vac.LatLon ) ) );
          route.Enqueue( new CmdD( cPos.DistanceTo( vac.LatLon, ConvConsts.EarthRadiusNm ) ) );
        }
        cPos = new LatLon( vac.LatLon ); // move position
        cTrack = vac.TRK;  // update track
      }
      route.Enqueue( new CmdE( ) ); // mandatory end command

      // we don't know the real acft type (no lookup in the converter)
      // so take a jet for above 180 and a GA one below
      if (maxGS > 180) {
        route.Descriptor.UpdateAcftType( Aircrafts.AircraftSelection.GetRandomAcftTypeOp( ).AcftType );
      }
      else {
        route.Descriptor.UpdateAcftType( Aircrafts.AircraftSelection.GetRandomGAAcftTypeOp( ).AcftType );
      }
      return route;
    }

    /// <summary>
    /// Create a route script from an AITraffic file in the same folder with the extension ".vsc" added
    /// The file may contain only AITFC,...  messages (nothing else)
    /// </summary>
    /// <param name="aitFile">A file with AITraffic messages</param>
    /// <param name="absoluteTrack">If true the message LatLon positions are flewn</param>
    /// <returns>True if successfull</returns>
    public bool CreateRouteScript( string aitFile, bool absoluteTrack, bool ignoreAirborne )
    {
      if (!File.Exists( aitFile )) return false;

      var aitStream = new List<string>( );
      using (var sr = new StreamReader( aitFile )) {
        do {
          aitStream.Add( sr.ReadLine( ) );
        } while (!sr.EndOfStream);
      }

      var route = RouteFromRTT( aitStream, absoluteTrack, ignoreAirborne );
      if (route == null) return false;

      var ret = true;
      using (var sw = new StreamWriter( aitFile + ".vsc", false )) {
        sw.WriteLine( $"# XPVTraffic-RTTConverter: {DateTime.Now.ToString( "s" )}" );
        sw.WriteLine( $"# Route script created from: {Path.GetFileName( aitFile )}" );
        sw.WriteLine( $"# Converted {aitStream.Count} messages into {route.Count} script commands" );
        sw.WriteLine( $"# " );
        ret &= route.WriteRoute( sw );
      }
      return ret;
    }

  }
}

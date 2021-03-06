﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using libXPVTgen.Aircrafts;
using libXPVTgen.coordlib;

namespace libXPVTgen.LiveTraffic
{
  /// <summary>
  /// RealTraffic formatter 
  /// https://rtweb.flyrealtraffic.com/RTdev1.4.pdf
  /// </summary>
  internal class RealTraffic
  {
    /*
     LiveTraffic has to open a TCP port (10747 by default) to which RealTraffic connects. 
     Once established, LiveTraffic then periodically sends its current location via this link back to RealTraffic so that 
     RealTraffic can filter and send the traffic data of that area.
    */
    /// <summary>
    /// The LinkPort - Connect to LifeTraffic and receive messages
    /// </summary>
    public const int PortLinkTCP = 10747;

    /*
     * extended format, used by LiveTraffic
     */
    /// <summary>
    /// The Traffic port to send traffic to LifeTraffic
    /// </summary>
    public const int PortTrafficUDP = 49003;

    /*
     * is the port on which RealTraffic periodically sends weather data
     */
    /// <summary>
    /// The weather port to send weather to LifeTraffic
    /// </summary>
    public const int PortWeatherUDP = 49004;

    /*
     * SimPos String:
     Your simulator plugin needs to provide a TCP socket server connection. By default,
      RealTraffic expects port 10747 (but this can be changed and is configurable in the Settings
      pane of the RealTraffic User Interface). Once your plugin detects that RealTraffic has
      connected to it, you need to transmit the following parameters at between 1 - 5 Hz (i.e. 1 – 5 times per second):
      • Pitch in radians * 100000
      • Bank in radians * 100000
      • Heading (or Track) in radians
      • Altitude in feet * 1000
      • True air speed in meters per second
      • Latitude in radians
      • Longitude in radians
      • All of that preceded by the characters “Qs121=”
      An example string would look like this:
      “Qs121=6747;289;5.449771266137578;37988724;501908;0.6564195830703577;-2.1443275933742236”
     */

    /// <summary>
    /// Convert a LinkMessage from LifeTraffic to only a latLon
    /// </summary>
    /// <param name="linkString">string from LifeTraffic</param>
    /// <param name="latLon">out LatLon received</param>
    /// <returns>True if successfull</returns>
    public static bool GetFromLinkString( ref string linkString, out LatLon latLon )
    {
      latLon = new LatLon( );
      if ( !linkString.Contains( "Qs121=" ) ) return false; // not enough

      // cheap... actually expensive CPU
      while ( !linkString.StartsWith( "Qs121=" ) ) {
        linkString = linkString.Substring( 1 );
      }
      // Have a Qs starter now
      string[] e = linkString.Split( new char[] { ';', '=' } );
      if ( e.Length < 8 ) return false; // not enough

      // remove from linkString 
      int len = 0;
      for ( int i = 0; i < 8; i++ ) {
        len += e[i].Length;
      }
      len += 7; // div chars
      if ( linkString.Length > len ) {
        linkString = linkString.Substring( len );
      }

      // try to process
      bool retval = false;
      if ( double.TryParse( e[6], out double lat ) && double.TryParse( e[7], out double lon ) ) {
        latLon.Lat = lat.ToDegrees( );
        latLon.Lon = lon.ToDegrees( );
        retval = true;
      }

      return retval;
    }


    /*
     * “AITFC,hexid,lat,lon,alt,vs,airborne,hdg,spd,cs,type,tail,from,to,timestamp”

      • Hexid: the hexadecimal ID of the transponder of the aircraft. This is a unique ID, and
          you can use this ID to track individual aircraft.     
      • Lat: latitude in degrees
      • Lon: longitude in degrees
      • Alt: altitude in feet
      • Vs: vertical speed in ft/min
      • Airborne: 1 or 0
      • Hdg: The heading of the aircraft (it’s actually the true track, strictly speaking. )
      • Spd: The speed of the aircraft in knots
      • Cs: the ICAO callsign (Emirates 413 = UAE413 in ICAO speak, = EK413 in IATA speak)
      • Type: the ICAO type of the aircraft, e.g. A388 for Airbus 380-800. B789 for Boeing 787-9 etc.
      • Tail: The registration number of the aircraft
      • From: The origin airport where known (in IATA or ICAO code)
      • To: The destination airport where known (in IATA or ICAO code)
      • Timestamp: The UNIX epoch timestamp when this position was valid
    */

    /// <summary>
    /// Format an AI-Traffic message from a virtual aircraft
    /// </summary>
    /// <param name="vac">The V-Aircraft record</param>
    /// <returns>An AI-Traffic string to send to LiveTraffic</returns>
    public static string AITrafficString( VAcft vac )
    {
      string airborne = ( vac.Airborne ) ? "1" : "0";
      //
      string rt = $"AITFC,{vac.AcftHex},{vac.LatLon.Lat:##0.0000000},{vac.LatLon.Lon:##0.0000000},{vac.Alt_ft},{(int)vac.VSI}"
        + $",{airborne},{vac.TRK:##0.0},{vac.GS:##0.0}"
        + $",{vac.AcftCallsign},{vac.AcftType},{vac.AcftTailReg}"
        + $",{vac.AcftFrom},{vac.AcftTo},{vac.TStamp}";
      return rt;
    }

    /// <summary>
    /// Convert an AI Traffic string to a AITvAcft moment
    /// </summary>
    /// <param name="aString">One AI Traffic message as string</param>
    /// <returns>A AITvAcft Moment of that string or null if invalid</returns>
    public static AITvAcft FromAITrafficString( string aString )
    {
      if ( !aString.StartsWith( "AITFC," ) ) return null;
      string[] e = aString.Split( new char[] { ',' } );
      if ( e.Length < 15 ) return null;

      var vac = new AITvAcft( );
      var ret = true;
      vac.AcftHex = e[1];
      ret &= double.TryParse( e[2], out double lat );
      ret &= double.TryParse( e[3], out double lon );
      ret &= double.TryParse( e[4], out double alt );
      ret &= double.TryParse( e[5], out double vsi );
      ret &= int.TryParse( e[6], out int airborne );
      ret &= double.TryParse( e[7], out double track );
      ret &= double.TryParse( e[8], out double gs );
      vac.AcftCallsign = e[9];
      vac.AcftType = e[10];
      vac.AcftTailReg = e[11];
      vac.AcftFrom = e[12];
      vac.AcftTo = e[13];
      ret &= long.TryParse( e[14], out long ts );

      if ( !ret ) return null; // ERROR one of the Parses failed

      // complete the Acft data
      vac.LatLon = new LatLon( lat, lon );
      vac.Alt_ft = (int)alt;
      vac.VSI = vsi;
      vac.Airborne = ( airborne != 0 );
      vac.TRK = track;
      vac.GS = gs;
      vac.TStamp = ts;

      // sanity checks - we need some items
      if ( vac.TStamp <= 0 ) return null;
      if ( vac.Alt_ft <= 0 ) return null;

      // TODO lookup acftType from hexcode in a database      
      return vac;
    }


    /*
     The weather messages are broadcast as UDP packets once every 10 seconds on port 49004
      containing a JSON string with the following format:
      "{"ICAO": "XXXX","QNH": 1013, "METAR": "XXXX", "NAME": "XXXX", "IATA": "XXXX" , "DISTNM": 0}";
      The fields should be self-explanatory, in case they’re not:
      • ICAO is the ICAO code of the nearest airport,
      • QNH is the reported pressure in hPa,
      • METAR contains the full METAR received,
      • NAME shows the airport name (usually long),
      • IATA is the IATA code of the airport (YSSY = Sydney in ICAO speak, SYD = Sydney in
      IATA speak), and lastly,
      • DISTNM is the distance to said airport in nautical miles.
     */
    public static string WeatherString()
    {
      // just a generic string reporting the standard pressure - we don't adjust heights anyway
      // METAR remains empty and the APT is Zurich... may be a long way from some...
      return $"{{\"ICAO\": \"LSZH\",\"QNH\": 1013, \"METAR\": \"\", \"NAME\": \"ZURICH\", \"IATA\": \"ZRH\" , \"DISTNM\": 10}}";
    }

  }
}

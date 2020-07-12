using System;
using System.Collections.Generic;
using System.Text;
using libXPVTgen.coordlib;
using libXPVTgen.my_awlib;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// A virtual IFR aircraft
  /// </summary>
  internal class IFRvAcft : VAcft
  {

    private static readonly Random m_random = new Random( );

    private LatLon startPos { get => Route.start_latlon; }
    private LatLon endPos { get => Route.end_latlon; }
    private uint m_stepLen_Sec = 0;
    private int m_nSteps = 0;   // step
    private int m_step = 0;   // step

    private double m_vsi = 0;
    private double m_hdg = 0;
    private double m_tas = 0;
    private string m_hex = "";
    private string m_type = "";
    private string m_reg = "";
    private long m_tstamp = 0;

    public awyRec Route; // Current segment (local copy)
    public override double VSI { get => m_vsi; }
    public override double HDG { get => m_hdg; }
    public override double TAS { get => m_tas; }

    public override string AcftHex { get => m_hex; }  // S-Mode Code "000nnn"
    public override string AcftType { get => m_type; }
    public override string AcftReg { get => m_reg; }   // XY-000

    public override string AcftFrom { get => Route.start_icao_id; }
    public override string AcftTo { get => Route.end_icao_id; }

    public override long TStamp { get=>m_tstamp; }

    public string AcftStartID { get => Route.startID; }
    public string AcftEndID { get => Route.endID; }

    public override bool Out { get => m_step > m_nSteps; }

    public IFRvAcft( string ID, awyRec routeRef, double tas, int alt, uint stepLen_Sec, string aType, string aHex )
    {
      base.ID = ID;
      Route = routeRef.DeepCopy( );
      m_stepLen_Sec = stepLen_Sec;
      m_tas = (float)tas;

      Alt_ft = alt;
      m_vsi = 0; // TODO..
      m_hdg = (float)startPos.BearingTo( endPos );
      // calc the increments for data sent
      var dist_nm = startPos.DistanceTo( endPos, ConvConsts.EarthRadiusNm );
      m_nSteps = (int)( dist_nm * ( 3600.0 / m_stepLen_Sec ) / tas );

      m_type = aType;
      m_hex = aHex;

      // init step
      StepModel( );
    }

    /// <summary>
    /// Create the same aircraft with a new leg
    /// </summary>
    /// <param name="vac">Current aircraft</param>
    /// <param name="routeRef">New segment</param>
    public IFRvAcft( IFRvAcft vac, awyRec routeRef )
    {
      // clone from vac
      base.ID = vac.ID;
      m_hex = vac.AcftHex;
      m_type = vac.AcftType;
      m_reg = vac.AcftReg;
      m_stepLen_Sec = vac.m_stepLen_Sec;

      base.Alt_ft = vac.Alt_ft;
      m_tas = vac.TAS;
      m_vsi = vac.VSI;
      m_step = 0; // reset

      // set submitted new segment
      Route = routeRef.DeepCopy( );
      // calculated
      m_hdg = startPos.BearingTo( endPos );
      // calc the increments for data sent
      var dist_nm = startPos.DistanceTo( endPos, ConvConsts.EarthRadiusNm );
      m_nSteps = (int)( dist_nm * ( 3600.0 / m_stepLen_Sec ) / TAS );
      // init step
      StepModel( );
    }

    /// <summary>
    /// Assign the aircraft a new route
    /// </summary>
    /// <param name="routeRef"></param>
    public void Update( awyRec routeRef )
    {
      Route = routeRef.DeepCopy( );
      m_step = 0; // reset
      // calculated
      m_hdg = startPos.BearingTo( endPos );
      // calc the increments for data sent
      var dist_nm = startPos.DistanceTo( endPos, ConvConsts.EarthRadiusNm );
      m_nSteps = (int)( dist_nm * ( 3600.0 / m_stepLen_Sec ) / TAS );
      // init step
      StepModel( );
    }



    /// <summary>
    /// Get to the next pos on the route
    /// </summary>
    public override void StepModel()
    {
      if ( Out ) return; // at end

      if ( m_step == 0 ) {
        // Init - first step
        base.LatLon = new LatLon( startPos );
      }
      else {
        double frac = (double)m_step / m_nSteps;
        base.LatLon = startPos.IntermediatePointTo( endPos, frac ); // where are we right now..
      }
      m_tstamp = DateTimeOffset.Now.ToUnixTimeSeconds( );
      m_step++;
    }

  }
}

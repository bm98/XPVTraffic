using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.coordlib;
using libXPVTgen.my_awlib;

namespace libXPVTgen
{
  /// <summary>
  /// A Virtual Aircraft
  /// </summary>
  public class VAcft : Acft
  {
    private static Random m_random = new Random( );

    private static List<string> m_acTypes = new List<string>( )
    { "B732", "B733", "B734", "	B735", "B736", "B737", "B738",
      "B743", "B753", "B762", "B763", "B764", "	B772", "B773", "B788", "B789",
      "A320", "A332", "A333", "A342", "A343", "A345", "A346", "A388", "A20N", "A21N",
      "	MC23", "T134", "T144", "MD11", "E120", "AN26", "JS41", "C130", "SB20",
      "C525", "C550", "E50P", "LJ24", "PA47", "PC24" , "PC12" , "EA50" };
    private static List<string> m_acTypesGA = new List<string>( )
    { "PC12", "PC6T", "C172", "C162", "C182", "BE58", "BE88", "CRBN", "DHC6", "VELT", "SF50", "BE9L"};

    public static string GetAcftType { get => m_acTypes.ElementAt( m_random.Next( 0, m_acTypes.Count ) ); }
    public static string GetGAAcftType { get => m_acTypesGA.ElementAt( m_random.Next( 0, m_acTypesGA.Count ) ); }


    // CLASS

    public float VSI; // ft/min
    public float HDG;
    public float TAS;
    public awyRec Route; // Current segment (local copy)
    public string AcftHex;  // S-Mode Code "000nnn"
    public string AcftType;
    public string AcftReg;   // XY-000
    public string AcftFrom { get => Route.start_icao_id; }
    public string AcftStartID { get => Route.startID; }
    public string AcftTo { get => Route.end_icao_id; }
    public string AcftEndID { get => Route.endID; }
    public long TStamp;

    private LatLon startPos { get => Route.start_latlon; }
    private LatLon endPos { get => Route.end_latlon; }
    private uint m_stepLen_Sec = 0;
    private int m_nSteps = 0;   // step
    private int m_step = 0;   // step

    public VAcft( string ID, awyRec routeRef, double tas, uint stepLen_Sec )
    {
      base.ID = ID;
      Route = routeRef.DeepCopy();
      m_stepLen_Sec = stepLen_Sec;
      TAS = (float)tas;

      VSI = 0; // TODO..
      HDG = (float)startPos.BearingTo( endPos );
      // calc the increments for data sent
      var dist_nm = startPos.DistanceTo( endPos, ConvConsts.EarthRadiusNm );
      m_nSteps = (int)( dist_nm * ( 3600.0 / m_stepLen_Sec ) / tas );
      // init step
      IncPos( );
    }

    /// <summary>
    /// Create the same aircraft with a new leg
    /// </summary>
    /// <param name="vac">Current aircraft</param>
    /// <param name="routeRef">New segment</param>
    public VAcft( VAcft vac, awyRec routeRef )
    {
      // clone from vac
      base.ID = vac.ID;
      AcftHex = vac.AcftHex;
      AcftType = vac.AcftType;
      AcftReg = vac.AcftReg;
      m_stepLen_Sec = vac.m_stepLen_Sec;

      base.Alt_ft = vac.Alt_ft;
      TAS = vac.TAS;
      VSI = vac.VSI;
      m_step = 0; // reset

      // set submitted new segment
      Route = routeRef.DeepCopy();
      // calculated
      HDG = (float)startPos.BearingTo( endPos );
      // calc the increments for data sent
      var dist_nm = startPos.DistanceTo( endPos, ConvConsts.EarthRadiusNm );
      m_nSteps = (int)( dist_nm * ( 3600.0 / m_stepLen_Sec ) / TAS );
      // init step
      IncPos( );
    }

    /// <summary>
    /// Assign the aircraft a new route
    /// </summary>
    /// <param name="routeRef"></param>
    public void Update(awyRec routeRef )
    {
      Route = routeRef.DeepCopy();
      m_step = 0; // reset
      // calculated
      HDG = (float)startPos.BearingTo( endPos );
      // calc the increments for data sent
      var dist_nm = startPos.DistanceTo( endPos, ConvConsts.EarthRadiusNm );
      m_nSteps = (int)( dist_nm * ( 3600.0 / m_stepLen_Sec ) / TAS );
      // init step
      IncPos( );
    }


    /// <summary>
    /// True if the acft is no longer active (end of route)
    /// </summary>
    public bool Out { get => m_step > m_nSteps; }

    /// <summary>
    /// Get to the next pos on the route
    /// </summary>
    public void IncPos()
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
      TStamp = DateTimeOffset.Now.ToUnixTimeSeconds( );
      m_step++;
    }


  }
}

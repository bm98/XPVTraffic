using System;
using System.Collections.Generic;
using System.Text;
using libXPVTgen.coordlib;

namespace libXPVTgen.my_awlib
{
  /// <summary>
  /// One XPlane 11 Airway database record
  /// </summary>
  public class awyRec
  {
    // fields in db
    public string ident = ""; // Key ?! start_icao_id + "_" + start_icao_region + "_" + end_icao_id + "_" + end_icao_region

    public string start_icao_id = "";     //01 Identifier of enroute fix or navaid at the beginning of this segment
    public string start_icao_region = ""; //02 ICAO region code of enroute NDB or terminal area airport Must be region code according to ICAO document No. 7910 
    public LatLon start_latlon = new LatLon( );
    public string end_icao_id = "";       //04 Identifier of enroute fix or navaid at the beginning of this segment
    public string end_icao_region = "";   //05 ICAO region code of enroute NDB or terminal area airport Must be region code according to ICAO document No. 7910 
    public LatLon end_latlon = new LatLon( );
    public short layer = 0;               //08 This is a "High" airway (1 = "low", 2 = "high"). 
                                          //   If an airway segment is both High and Low, then it should be listed twice( once in each category ). 
                                          //   This determines if the airway is shown on X-Plane's "High Enroute" or "Low Enroute" charts
    public int baseFt = 0;                //09 Base of airway in hundreds of feet (18000 ft in this example) Integer between 0 and 600
    public int topFt = 60000;             //10 Top of airways in hundreds of feet (45000 ft in this example) Integer between 0 and 600
    public string segments = "";          //11 Airway segment name. Up to five characters per name, names separated by hyphens
                                          //  If multiple airways share this segment, then all names will be included separated by a hyphen( eg. "J13-J14-J15")
    public LatLon mid_latlon = new LatLon( );
    public float brg = 0;   //  bearing start to end
    public string startID = "";
    public string endID = "";

    /// <summary>
    /// cTor: populate the record
    /// </summary>
    public awyRec( string sid, string sreg, string sLat, string sLon,
                   string eid, string ereg, string eLat, string eLon,
                   string lay, string bFt, string tFt, string seg )
    {
#if !DEBUG
      // catch XP11 db insconsistencies while debugging
      try {
#endif
      start_icao_id = sid.ToUpperInvariant( );
      start_icao_region = sreg.ToUpperInvariant( );
      start_latlon = new LatLon( double.Parse( sLat ), double.Parse( sLon ) );
      end_icao_id = eid.ToUpperInvariant( ); ;
      end_icao_region = ereg.ToUpperInvariant( );
      end_latlon = new LatLon( double.Parse( eLat ), double.Parse( eLon ) );
      layer = short.Parse( lay );
      baseFt = int.Parse( bFt );
      topFt = int.Parse( tFt );
      segments = seg;
#if !DEBUG
      }
      catch {
        return; // error leave ident empty
      }
#endif

      // calculated ones
      startID = $"{sid}_{sreg}";
      endID = $"{eid}_{ereg}";
      ident = $"{startID}_{endID}";
      mid_latlon = start_latlon.MidpointTo( end_latlon );
      brg = (float)start_latlon.BearingTo( end_latlon );
    }

    /// <summary>
    /// Creates a deep copy of this record
    /// </summary>
    /// <returns>An independent copy of this record</returns>
    public awyRec DeepCopy()
    {
      var awr = (awyRec)this.MemberwiseClone( );
      // need to alloc new LatLons
      awr.start_latlon = new LatLon( this.start_latlon );
      awr.end_latlon = new LatLon( this.end_latlon );
      awr.mid_latlon = new LatLon( this.mid_latlon );
      return awr;
    }

    /// <summary>
    /// returns true if the record is valid
    /// </summary>
    public bool IsValid { get => !string.IsNullOrEmpty( ident ); }

    /// <summary>
    /// Returns the distance from start to end latLon
    /// </summary>
    public double Distance_nm
    {
      get {
        return start_latlon.DistanceTo( end_latlon, ConvConsts.EarthRadiusNm );
      }
    }


    public override string ToString()
    {
      return $"{start_icao_id} {start_icao_region} {start_latlon.Lat} {start_latlon.Lon} "
           + $"{end_icao_id} {end_icao_region} {end_latlon.Lat} {end_latlon.Lon} "
           + $"{layer} {baseFt} {topFt} {segments}";
    }

  }
}

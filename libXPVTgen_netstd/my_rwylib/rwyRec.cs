using System;
using System.Collections.Generic;
using System.Text;
using libXPVTgen.coordlib;

namespace libXPVTgen.my_rwylib
{
  /// <summary>
  /// One XPTV Runway database record
  /// </summary>
  public class rwyRec
  {
    // fields in db
    public string ident = ""; // Key ?! icao_id + "_" rwy_id

    public string icao_id = "";     // Identifier of airport
    public string rwy_id = "";      // RW07L  RW07 ..
    public int rwy_num = 0;
    public string rwy_side = "";
    public int elevation = 0;
    public LatLon start_latlon = new LatLon( ); // start of rwy
    public LatLon end_latlon = new LatLon( );   // end of rwy
    public double brg = 0;

    /// <summary>
    /// cTor: populate the record
    /// </summary>
    public rwyRec( string icaoName, string rwNum, string rwSide, string landElev, string sLat, string sLon, string eLat, string eLon )
    {
      icao_id = icaoName.ToUpperInvariant( );
      rwy_num = int.Parse( rwNum );
      rwy_side = rwSide;
      start_latlon = new LatLon( double.Parse( sLat ), double.Parse( sLon ) );
      end_latlon = new LatLon( double.Parse( eLat ), double.Parse( eLon ) );
      elevation = int.Parse( landElev );

      // calculated ones

      rwy_id = $"RW{rwy_num:00}{rwy_side}";
      brg = start_latlon.BearingTo( end_latlon );
      ident = $"{icao_id}_{rwy_id}";
    }

    /// <summary>
    /// returns true if the record is valid
    /// </summary>
    public bool IsValid
    {
      get => !string.IsNullOrEmpty( icao_id ) && !string.IsNullOrEmpty( rwy_id ) && !string.IsNullOrEmpty( ident );
    }

    /// <summary>
    /// Creates a deep copy of this record
    /// </summary>
    /// <returns>An independent copy of this record</returns>
    public rwyRec DeepCopy()
    {
      var awr = (rwyRec)this.MemberwiseClone( );
      // need to alloc new LatLons
      awr.start_latlon = new LatLon( this.start_latlon );
      awr.end_latlon = new LatLon( this.end_latlon );
      return awr;
    }

    /// <summary>
    /// Returns the record as string
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString()
    {
      /*   0  1  2   3      4        5        6        7
         LSZH,14,R,1452,42.395806,8.320987,42.395847,8.32456 
     */
      return $"{icao_id},{rwy_num},{rwy_side},{elevation},"
           + $"{start_latlon.Lat},{start_latlon.Lon},"
           + $"{end_latlon.Lat},{end_latlon.Lon}";
    }

  }
}

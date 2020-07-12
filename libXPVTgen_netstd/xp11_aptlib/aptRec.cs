using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;

namespace libXPVTgen.xp11_aptlib
{
  /// <summary>
  /// One XPlane 11 Airport database record
  /// </summary>
  public class aptRec
  {
    // fields in db
    public string ident = ""; // Key ?! icao_id + "_" rwy_id

    public string icao_id = "";   // Identifier of airport
    public string rwy_id = "";    // RW07L  RW07 ..
    public string elevation = "";
    public string lat = "";
    public string lon = "";

    public string rwy_num = "";
    public string rwy_side = "";
    public string rev_rwy_ident = ""; // other rwy name

    // Convert from File DMS Format lat/lon
    private LatLon CnvAptDMS( string lat_dms, string lon_dms )
    {
      lat_dms = lat_dms.Trim( ).ToUpperInvariant( );
      //N|Shhmmssss ->  51° 28′ 40.12″ N
      string dms = $"{lat_dms.Substring( 1, 2 )}° {lat_dms.Substring( 3, 2 )}′ {lat_dms.Substring( 5, 4 ).Insert( 2, "." )}″ {lat_dms.Substring( 0, 1 )}";
      var ll = new LatLon( );
      ll.Lat = Dms.ParseDMS( dms );
      //W|Ehhhmmssss ->  51° 28′ 40.12″ W
      dms = $"{lon_dms.Substring( 1, 3 )}° {lon_dms.Substring( 4, 2 )}′ {lon_dms.Substring( 6, 4 ).Insert( 2, "." )}″ {lon_dms.Substring( 0, 1 )}";
      ll.Lon = Dms.ParseDMS( dms );
      return ll;
    }

    /// <summary>
    /// cTor: populate the record
    /// </summary>
    /// <param name="id">ident (UPPERCASE)</param>
    public aptRec( string icaoName, string rwyIdent, string landElev, string latS, string lonS )
    {
      icao_id = icaoName.ToUpperInvariant( );

      if ( rwyIdent.Length < 4 ) return;  // ERROR cannot use without runway
      rwy_id = rwyIdent.ToUpperInvariant( );
      rwy_num = rwy_id.Substring( 2, 2 );
      if ( rwyIdent.Length > 4 )
        rwy_side = rwy_id.Substring( 4, 1 );

      string revSide = rwy_side; // can be empty or "C"
      if ( rwy_side == "R" ) revSide = "L"; // reverse
      if ( rwy_side == "L" ) revSide = "R"; // reverse
      double forward = int.Parse( rwy_num ) * 10;
      double reverse = forward.AddDegrees( 180 ); // takes care of the circle overflow
      rev_rwy_ident = $"{icao_id}_RW{(int)Math.Round( reverse / 10 ):00}{revSide}";

      if ( !int.TryParse( landElev, out int e ) ) return;  // ERROR cannot use without valid elevation
      elevation = landElev;

      // translate lat, lon  ;N42395806 , W083260384, E008320987
      if ( latS.Length < 9 ) return;    // ERROR cannot use without Lat
      if ( lonS.Length < 10 ) return;  // ERROR cannot use without Lon
      var ll = CnvAptDMS( latS, lonS );
      lat = ll.Lat.ToString( );
      lon = ll.Lon.ToString( );

      ident = $"{icao_id}_{rwy_id}";
    }


    /// <summary>
    /// returns true if the record is valid
    /// </summary>
    public bool IsValid
    {
      get => !string.IsNullOrEmpty( icao_id ) && !string.IsNullOrEmpty( rwy_id ) && !string.IsNullOrEmpty( ident );
    }


  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.my_rwylib
{
  /// <summary>
  /// Reader for our own my_rwy.dat file
  /// </summary>
  public class rwyReader
  {
    /// <summary>
    /// Translates from native to generic record format
    /// </summary>
    /// <param name="native"></param>
    /// <returns></returns>
    private static rwyRec FromNative( string native )
    {
      if ( string.IsNullOrEmpty( native ) ) return null;

      /*   0  1  2   3      4        5        6        7
         LSZH,14,R,1452,42.395806,8.320987,42.395847,832456 
     */
      // should be the coma separated variant
      string[] e = native.Split( new char[] { ',' }, StringSplitOptions.None );

      string icao_id = "", rwy_num = "", rwy_side = "";
      string elev = "";
      string sLat = "", sLon = "", eLat = "", eLon = "";

      int idx = 0;
      if ( e.Length > idx )
        icao_id = e[idx]; idx++;
      if ( e.Length > idx )
        rwy_num = e[idx]; idx++;
      if ( e.Length > idx )
        rwy_side = e[idx]; idx++;
      if ( e.Length > idx )
        elev = e[idx]; idx++;
      if ( e.Length > idx )
        sLat = e[idx]; idx++;
      if ( e.Length > idx )
        sLon = e[idx]; idx++;
      if ( e.Length > idx )
        eLat = e[idx]; idx++;
      if ( e.Length > idx )
        eLon = e[idx]; idx++;

      return new rwyRec( icao_id, rwy_num, rwy_side, elev, sLat, sLon, eLat, eLon );

    }

    /// <summary>
    /// Reads one file to fill the db
    /// </summary>
    /// <param name="db">The awyDatabase to fill</param>
    /// <param name="fName">The qualified filename</param>
    /// <returns>The result string, either empty or error</returns>
    private static string ReadDbFile( ref rwyDatabase db, string fName )
    {
      string ret = "";
      using ( var sr = new StreamReader( fName ) ) {
        do {
          string buffer = sr.ReadLine( );
          var rec = FromNative( buffer );
          if ( rec != null && rec.IsValid ) {
            ret += db.Add( rec ); // collect adding information
          }
        } while ( !sr.EndOfStream );
        //
      }
      return ret;
    }

    /// <summary>
    /// Reads the XPlane 11 earth_awy.dat file and populates the supplied database
    /// </summary>
    /// <param name="db">The aptDatabase to fill</param>
    /// <param name="fName">The file to read</param>
    /// <returns>The result string, either empty or error</returns>
    public static string ReadDb( ref rwyDatabase db, string fName )
    {
      if ( !File.Exists( fName ) ) return $"File does not exist\n";

      return ReadDbFile( ref db, fName );
    }

  }
}

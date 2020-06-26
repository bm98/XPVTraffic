using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.my_awlib
{
  /// <summary>
  /// Reader for our own my_awy.dat file
  /// </summary>
  public class awyReader
  {
    /// <summary>
    /// Translates from native to generic record format
    /// </summary>
    /// <param name="native"></param>
    /// <returns></returns>
    private static awyRec FromNative( string native )
    {
      if ( string.IsNullOrEmpty( native ) ) return null;

      /* 0    1  2     3    4     5  6    7    8 9      10   
         070N CY 2.001 4.02 NADMA CY 1.23 3.25 2 10000  50000
   */
      // should be the space separated variant
      string[] e = native.Split( new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );

      string start_icao_id = "", start_icao_region = "", end_icao_id = "", end_icao_region = "";
      string layer = "", baseFt = "", topFt = "";
      string sLat = "", sLon = "", eLat = "", eLon = "", seg = "";

      int idx = 0;
      if ( e.Length > idx )
        start_icao_id = e[idx].Trim( new char[] { ' ', '"' } ).ToUpperInvariant( ); idx++;
      if ( e.Length > idx )
        start_icao_region = e[idx].Trim( new char[] { ' ', '"' } ).ToUpperInvariant( ); idx++;
      if ( e.Length > idx )
        sLat = e[idx].Trim( new char[] { ' ', '"' } ); idx++;
      if ( e.Length > idx )
        sLon = e[idx].Trim( new char[] { ' ', '"' } ); idx++;
      if ( e.Length > idx )
        end_icao_id = e[idx].Trim( new char[] { ' ', '"' } ).ToUpperInvariant( ); idx++;
      if ( e.Length > idx )
        end_icao_region = e[idx].Trim( new char[] { ' ', '"' } ).ToUpperInvariant( ); idx++;
      if ( e.Length > idx )
        eLat = e[idx].Trim( new char[] { ' ', '"' } ); idx++;
      if ( e.Length > idx )
        eLon = e[idx].Trim( new char[] { ' ', '"' } ); idx++;

      if ( e.Length > idx )
        layer = e[idx].Trim( new char[] { ' ', '"' } ); idx++;
      if ( e.Length > idx )
        baseFt = e[idx].Trim( new char[] { ' ', '"' } ); idx++;
      if ( e.Length > idx )
        topFt = e[idx].Trim( new char[] { ' ', '"' } ); idx++;
      if ( e.Length > idx )
        seg = e[idx].Trim( new char[] { ' ', '"' } ); idx++;

      return new awyRec( start_icao_id, start_icao_region, sLat, sLon, end_icao_id, end_icao_region, eLat, eLon, layer, baseFt, topFt, seg );

    }

    /// <summary>
    /// Reads one file to fill the db
    /// </summary>
    /// <param name="db">The awyDatabase to fill</param>
    /// <param name="fName">The qualified filename</param>
    /// <returns>The result string, either empty or error</returns>
    private static string ReadDbFile( ref awyDatabase db, string fName )
    {
      string ret = "";
      using ( var sr = new StreamReader( fName ) ) {
        string buffer = sr.ReadLine( );
        while ( !sr.EndOfStream ) {
          if ( buffer.StartsWith( "99 " ) ) break;
          var rec = FromNative( buffer );
          if ( rec != null && rec.IsValid ) {
            ret += db.Add( rec ); // collect adding information
          }
          buffer = sr.ReadLine( );
        }
        //
      }
      return ret;
    }

    /// <summary>
    /// Reads the XPlane 11 earth_awy.dat file and populates the supplied database
    /// </summary>
    /// <param name="db">The awyDatabase to fill</param>
    /// <param name="fName">The file to read</param>
    /// <returns>The result string, either empty or error</returns>
    public static string ReadDb( ref awyDatabase db, string fName )
    {
      if ( !File.Exists( fName ) ) return $"File does not exist\n";

      return ReadDbFile( ref db, fName );
    }

  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.xp11_navlib
{
  /// <summary>
  /// Reader for XPlane 11 earth_fix.dat file
  /// </summary>
  public class fixReader
  {
    /// <summary>
    /// Translates from native to generic record format
    /// </summary>
    /// <param name="native"></param>
    /// <returns></returns>
    private static navRec FromNative( string native )
    {
      if ( string.IsNullOrEmpty( native ) ) return null;

      /*   0                  1           2     3  4     5
           33.492513889    9.217400000  07EBA ENRT DT
          -10.910555556   16.123333333  102KU ENRT FN
   */
      // should be the space separated variant
      string[] e = native.Split( new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );
      var recType = navRec.NavTypes.FIX;

      string lat = "", lon = "", icao_id = "", terminal_id = "", icao_region = "", wpType = "";

      if ( e.Length > 0 )
        lat = e[0].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 1 )
        lon = e[1].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 2 )
        icao_id = e[2].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 3 )
        terminal_id = e[3].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 4 )
        icao_region = e[4].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 5 )
        wpType = e[5].Trim( new char[] { ' ', '"' } );


      return new navRec( recType, lat, lon, "", "", "", "", icao_id, terminal_id, icao_region, wpType, "" );

    }


    /// <summary>
    /// Reads one file to fill the db
    /// </summary>
    /// <param name="db">The navDatabase to fill</param>
    /// <param name="fName">The qualified filename</param>
    /// <returns>The result string, either empty or error</returns>
    private static string ReadDbFile( ref navDatabase db, string fName )
    {
      var icaoPre = Path.GetFileNameWithoutExtension( fName );
      string ret = "";
      using ( var sr = new StreamReader( fName ) ) {
        string buffer = sr.ReadLine( ); // header line
        buffer = sr.ReadLine( ); // header line 2
        buffer = sr.ReadLine( );
        while ( !sr.EndOfStream ) {
          if ( buffer.StartsWith( "99" ) ) break;
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
    /// Reads the XPlane 11 earth_fix.dat file and populates the supplied database
    /// </summary>
    /// <param name="db">The navDatabase to fill</param>
    /// <param name="fName">The file to read</param>
    /// <returns>The result string, either empty or error</returns>
    public static string ReadDb( ref navDatabase db, string fName )
    {
      if ( !File.Exists( fName ) ) return $"File does not exist\n";

      return ReadDbFile( ref db, fName );
    }

  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.xp11_awylib
{
  /// <summary>
  /// Reader for XPlane 11 earth_awy.dat file
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

      /*   0  1  2   3    4  5  6 7   8   9  10 
         070N CY 11 NADMA CY 11 N 2   0   0 NCAN
        07BAN OS 11 NIKAS LC 11 N 1 290 600 R785
   */
      // should be the space separated variant
      string[] e = native.Split( new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );

      string start_icao_id = "", start_icao_region = "", start_navaid = "", end_icao_id = "", end_icao_region = "", end_navaid = "";
      string restriction = "", layer = "", baselevel = "", toplevel = "", name = "";


      if ( e.Length > 0 )
        start_icao_id = e[0].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 1 )
        start_icao_region = e[1].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 2 )
        start_navaid = e[2].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 3 )
        end_icao_id = e[3].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 4 )
        end_icao_region = e[4].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 5 )
        end_navaid = e[5].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 6 )
        restriction = e[6].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 7 )
        layer = e[7].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 8 )
        baselevel = e[8].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 9 )
        toplevel = e[9].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 10 )
        name = e[10].Trim( new char[] { ' ', '"' } );

      return new awyRec( start_icao_id, start_icao_region, start_navaid, end_icao_id, end_icao_region, end_navaid, restriction, layer, baselevel, toplevel, name );

    }

    /// <summary>
    /// Reads one file to fill the db
    /// </summary>
    /// <param name="db">The awyDatabase to fill</param>
    /// <param name="fName">The qualified filename</param>
    /// <returns>The result string, either empty or error</returns>
    private static string ReadDbFile( ref awyDatabase db, string fName )
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.xp11_navlib
{
  /// <summary>
  /// Reader for XPlane 11 earth_nav.dat file
  /// </summary>
  public class navReader
  {

    /// <summary>
    /// Translates from native to generic record format
    /// Record Types: 
    ///    2 NDB, 3 VOR, 13 DME (from earth_nav)
    /// (11) FIX (from earth_fix)
    /// </summary>
    /// <param name="native"></param>
    /// <returns></returns>
    private static navRec FromNative( string native )
    {
      if ( string.IsNullOrEmpty( native ) ) return null;
      /*   0      1                2            3       4     5          6     7   8   9     10         11     (12)    (13)   (14)
           2  47.632522222 -122.389516667       0      362    25       0.000   BF ENRT K1 NOLLA/KBFI   LMM    RW13R     NDB
           3  30.321916667   -9.383472222      300    11720   130     -3.000  ADM ENRT GM AL-MASSIRA (AGADIR) VOR/DME
          12  30.321916667   -9.383472222      300    11720   130      0.000  ADM ENRT GM AL-MASSIRA (AGADIR) VOR/DME

   */
      // should be the space separated variant
      string[] e = native.Split( new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );

      var recType = (navRec.NavTypes)int.Parse( e[0] );
      if ( !( ( recType == navRec.NavTypes.DME )
           || ( recType == navRec.NavTypes.NDB )
           || ( recType == navRec.NavTypes.VOR )
           || ( recType == navRec.NavTypes.FIX ) ) ) {

        return null; // ignore unwanted ones
      }
      string lat = "", lon = "", elevation = "", freq = "", range = "", deviation = "", icao_id = "", terminal_id = "", icao_region = "", name = "", locName = "";

      if ( e.Length > 1 )
        lat = e[1].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 2 )
        lon = e[2].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 3 )
        elevation = e[3].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 4 )
        freq = e[4].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 4 )
        range = e[5].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 6 )
        deviation = e[6].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 7 )
        icao_id = e[7].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 8 )
        terminal_id = e[8].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 9 )
        icao_region = e[9].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 10 )
        name = e[10].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 11 )
        locName = e[11].Trim( new char[] { ' ', '"' } );
      if ( e.Length > 12 )
        locName += " " + e[12];
      if ( e.Length > 13 )
        locName += " " + e[13];
      if ( e.Length > 14 )
        locName += " " + e[14];

      return new navRec( recType, lat, lon, elevation, freq, range, deviation, icao_id, terminal_id, icao_region, name, locName );

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
    /// Reads the XPlane 11 earth_nav.dat file and populates the supplied database
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

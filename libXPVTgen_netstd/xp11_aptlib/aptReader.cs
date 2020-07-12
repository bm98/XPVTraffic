using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.xp11_aptlib
{
  /// <summary>
  /// Reader for XPlane 11 CIFP\AptID.dat file
  /// Read only RWY: records
  /// </summary>
  public class aptReader
  {
    /// <summary>
    /// Translates from native to generic record format
    /// </summary>
    /// <param name="native"></param>
    /// <returns></returns>
    private static aptRec FromNative( string native, string icaoName )
    {
      if ( string.IsNullOrEmpty( native ) ) return null;

      /*   0    1     2      3     4   5  6   7  8   9          10        11
          RWY:RW09L,     ,      ,00974, ,    , ,   ;N42395806,W083260384,0000;

          RWY:RW10 ,     ,      ,01391, ,    , ,   ;N47273218,E008321493,0000;
          RWY:RW28 ,     ,      ,01416, ,IZW ,1,   ;N47272376,E008341363,0000;

          RWY:RW14 ,     ,      ,01402, ,IKL ,3,   ;N47285553,E008320987,0493;
          RWY:RW32 ,     ,      ,01402, ,    , ,   ;N47274065,E008335206,0000;

          RWY:RW16 ,     ,      ,01390, ,IZH ,3,   ;N47283257,E008320937,0000;
          RWY:RW34 ,     ,      ,01388, ,IZS ,1,   ;N47265739,E008331491,1542;

          RWY:RW16 ,     ,      ,02372, ,    , ,   ;N47160378,E008180336,0000;
          RWY:RW34 ,     ,      ,02372, ,    , ,   ;N47154865,E008181149,0591;

          RWY:RW08R,-1000,      ,01024, ,IATL,1,   ;N33384843,W084261810,0000;
          RWY:RW09L,-0300,      ,01019, ,IHZK,1,   ;N33380494,W084265268,0000;
          RWY:RW09R,-0400,      ,01026, ,IFUN,3,   ;N33375453,W084265268,0000;

      0: const RWY:
      1: Rwy Ident
      2: Rwy Grad
      3: LTP Ellipsoid Height
      4: Landing Threshold Elevation
      5: TCH ?
      6: LOC/MLS/GLS Ident
      7: CAT / Class
      8: TCH ?
      9: Latitude
      10: Longitude
      11: Displaced Threshold 
   */
      // should be the space separated variant
      string[] e = native.Split( new char[] { ':', ',', ';' }, StringSplitOptions.None );

      // fields from 1..
      string rwyIdent = "", landElev = "", lat = "", lon = "";


      if ( e.Length > 1 )
        rwyIdent = e[1].Trim( );
      if ( e.Length > 4 )
        landElev = e[4].Trim( );
      if ( e.Length > 9 )
        lat = e[9].Trim( );
      if ( e.Length > 10 )
        lon = e[10].Trim( );
      return new aptRec( icaoName, rwyIdent, landElev, lat, lon );

    }

    /// <summary>
    /// Reads one file to fill the db
    /// </summary>
    /// <param name="db">The awyDatabase to fill</param>
    /// <param name="fName">The qualified filename</param>
    /// <returns>The result string, either empty or error</returns>
    private static string ReadDbFile( ref aptDatabase db, string fName )
    {
      var icaoPre = Path.GetFileNameWithoutExtension( fName );
      string ret = "";
      using ( var sr = new StreamReader( fName ) ) {
        do {
          string buffer = sr.ReadLine( );
          if ( buffer.StartsWith( "RWY:" ) ) {
            var rec = FromNative( buffer, Path.GetFileNameWithoutExtension( fName ) );
            if ( rec != null && rec.IsValid ) {
              ret += db.Add( rec ); // collect adding information
            }
          }
        } while ( !sr.EndOfStream );
        //
      }
      return ret;
    }

    /// <summary>
    /// Reads the XPlane 11 earth_awy.dat file and populates the supplied database
    /// </summary>
    /// <param name="db">The awyDatabase to fill</param>
    /// <param name="path">The file to read</param>
    /// <returns>The result string, either empty or error</returns>
    public static string ReadDb( ref aptDatabase db, string path )
    {
      if ( !Directory.Exists( path ) ) return $"Directory does not exist\n";

      string ret = "";
      foreach ( var f in Directory.EnumerateFiles( path ) ) {
        var e = ReadDbFile( ref db, f );
        if ( !string.IsNullOrEmpty( e ) )
          ret += $"{e}\n"; // collect errors
      }
      return ret;
    }

  }
}

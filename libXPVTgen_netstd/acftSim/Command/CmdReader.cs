using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Read .vsc scripts
  /// </summary>
  class CmdReader
  {

    /// <summary>
    /// Reads one Script file
    /// NOTE: Does not check for command inconsistencies whatsoever
    /// only a limited format check is done
    /// </summary>
    /// <param name="filename">The file to read</param>
    /// <returns>A CmdList either populated or empty on Error</returns>
    public static CmdList ReadCmdScript( string filename )
    {
      var list = new CmdList( );
      if ( !File.Exists( filename ) ) return list;// ERROR return an empty list

      using ( var sr = new StreamReader( filename ) ) {
        bool expectCmdA = true;
        do {
          string buffer = sr.ReadLine( ).Trim( );
          string[] x = buffer.Split( new char[] { '#' } ); // cut comments
          if ( x.Length > 0 && x[0].Length > 0 ) {
            // use first part only
            string[] e = x[0].Split( new char[] { '=', ';' } );
            if ( e.Length < 2 ) continue; // at least 2 items should be there... Cmd and Arg1

            if ( expectCmdA ) {
              // start of script expected - cmd A accepted only
              var a = CmdA.CreateFromStrings( e );
              if ( a == null ) return list; // ERROR in CmdA 

              list.Enqueue( a );
              expectCmdA = false; // we have it
            }

            else {
              // rest of script
              switch ( e[0].Substring( 0, 1 ) ) {
                case "D":
                  var d = CmdD.CreateFromStrings( e );
                  if ( d != null ) list.Enqueue( d );
                  break;

                case "T":
                  var t = CmdT.CreateFromStrings( e );
                  if ( t != null ) list.Enqueue( t );
                  break;

                case "H":
                  var h = CmdH.CreateFromStrings( e );
                  if ( h != null ) list.Enqueue( h );
                  break;

                case "G":
                  var g = CmdG.CreateFromStrings( e );
                  if ( g != null ) list.Enqueue( g );
                  break;

                case "S":
                  var s = CmdS.CreateFromStrings( e );
                  if ( s != null ) list.Enqueue( s );
                  break;

                case "V":
                  var v = CmdV.CreateFromStrings( e );
                  if ( v != null ) list.Enqueue( v );
                  break;

                case "M":
                  var m = CmdM.CreateFromStrings( e );
                  if ( m != null ) list.Enqueue( m );
                  break;

                default:
                  // not supported ..
                  break;

              }
            }
          }
        } while ( !sr.EndOfStream );
      }
      list.Enqueue( new CmdE( ) ); // MUST be the last one
      return list;
    }

    /// <summary>
    /// Reads all scripts in the given folder
    /// </summary>
    /// <param name="scriptPath">Path to script files</param>
    /// <returns>A list of CmdList read from the folder</returns>
    public static List<CmdList> ReadScripts( string scriptPath )
    {
      var list = new List<CmdList>( );
      if ( !Directory.Exists( scriptPath ) ) return list;
      foreach ( var f in Directory.EnumerateFiles( scriptPath, "*.vsc", SearchOption.AllDirectories ) ) {
        var cl = ReadCmdScript( f );
        if ( !cl.IsEmpty ) {
          list.Add( cl );
        }
      }
      return list;
    }


  }
}

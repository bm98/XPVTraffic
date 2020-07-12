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
          string buffer = sr.ReadLine( ).Trim( ).ToUpperInvariant( );
          string[] x = buffer.Split( new char[] { '#' } ); // cut comments
          if ( x.Length > 0 && x[0].Length>0) {
            // use first part only
            string[] e = x[0].Split( new char[] { '=', ';' } );
            if ( e.Length > 1 ) {  // at least 2 items should be there...
              if ( expectCmdA ) {
                // start of script expected - cmd A only
                if ( e[0].StartsWith( "A" ) ) {
                  var c = new CmdA( ) { AcftType = e[1].Trim().ToUpperInvariant( ) };
                  if ( e.Length > 2 ) c.RwyID = e[2].Trim().ToUpperInvariant(); // optional
                  list.Enqueue( c );
                  expectCmdA = false;
                }
              }
              else {
                // rest of script
                switch ( e[0].Substring( 0, 1 ) ) {
                  case "D":
                    if ( double.TryParse( e[1], out double dist ) ) {
                      var c = new CmdD( ) { Dist = dist };
                      list.Enqueue( c );
                    }
                    break;
                  case "T":
                    if ( double.TryParse( e[1], out double turn ) ) {
                      var c = new CmdT( ) { TurnAngle = turn };
                      list.Enqueue( c );
                    }
                    break;
                  case "H":
                    if ( double.TryParse( e[1], out double hdg ) ) {
                      var c = new CmdH( ) { Heading = hdg };
                      list.Enqueue( c );
                    }
                    break;

                  case "S":
                    if ( int.TryParse( e[1], out int tas ) ) {
                      var c = new CmdS( ) { TAS = tas };
                      list.Enqueue( c );
                    }
                    break;
                  case "V":
                    if ( int.TryParse( e[1], out int vs ) ) {
                      var c = new CmdV( ) { VSI = vs };
                      if ( e.Length > 2 ) {
                        // must have the Alt
                        if ( int.TryParse( e[2], out int alt ) ) {
                          c.AltAGL = alt;
                          list.Enqueue( c );
                        }
                      }
                    }
                    break;
                  default:
                    // not usable ..
                    break;

                }
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

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
          if ( x.Length > 0 && x[0].Length > 0 ) {
            // use first part only
            string[] e = x[0].Split( new char[] { '=', ';' } );
            if ( e.Length < 2 ) continue; // at least 2 items should be there... Cmd and Arg1

            if ( expectCmdA ) {
              // start of script expected - cmd A accepted only
              if ( e[0].StartsWith( "A" ) ) {
                var c = new CmdA( e[1].Trim( ).ToUpperInvariant( ) );
                if ( e.Length > 2 )
                  c.RunwayPreference = e[2].Trim( ).ToUpperInvariant( ); // optional, preferred RWY
                if ( e.Length > 3 )
                  c.RunwayPrefStrict = e[3].Trim( ).ToUpperInvariant( ) == "S"; // optional, preferred RWY is strict

                list.Enqueue( c );
                expectCmdA = false; // we have it
              }
            }

            else {
              // rest of script
              switch ( e[0].Substring( 0, 1 ) ) {
                case "D":
                  if ( double.TryParse( e[1], out double dist ) ) {
                    list.Enqueue( new CmdD( dist ) );
                  }
                  break;
                case "T":
                  if ( double.TryParse( e[1], out double turn ) ) {
                    var c = new CmdT( turn );
                    if ( e.Length > 2 ) {
                      // may have turnrate
                      if ( double.TryParse( e[2], out double tRate ) ) {
                        c.TurnRate = tRate;
                      }
                    }
                    list.Enqueue( c );
                  }
                  break;
                case "H":
                  if ( double.TryParse( e[1], out double brg ) ) {
                    var c = new CmdH( brg );
                    if ( e.Length > 2 ) {
                      // may have turnrate
                      if ( double.TryParse( e[2], out double tRate ) ) {
                        c.TurnRate = tRate;
                      }
                    }
                    list.Enqueue( c );
                  }
                  break;

                case "S":
                  if ( int.TryParse( e[1], out int gs ) ) {
                    var c = new CmdS( gs );
                    if ( e.Length > 2 ) {
                      // may have accel
                      if ( double.TryParse( e[2], out double accel ) ) {
                        c.Accel = accel;
                      }
                    }
                    list.Enqueue( c );
                  }
                  break;

                case "V":
                  if ( e.Length > 2 ) {
                    // must have the VSI and Alt
                    if ( double.TryParse( e[1], out double vs ) && double.TryParse( e[2], out double alt ) ) {
                      list.Enqueue( new CmdV( Math.Abs( vs ), alt ) ); // can use old scripts with +- VS, new determines sign based on delta Alt
                    }
                  }
                  break;

                case "M":
                  if ( e.Length > 1 ) {
                    if ( int.TryParse( e[1], out int msl ) ) {
                      list.Enqueue( new CmdM( msl > 0 ) );
                    }
                  }
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Head towards (Set trak to bearing) Command
  /// e.g. H=221.5[;3.5]  # head towards 221.5 degm and turn at a rate of 3.5 deg/sec
  /// </summary>
  class CmdH : CmdBase
  {
    /// <summary>
    /// Create a CmdH from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdH CreateFromStrings( string[] script )
    {
      // must be 2 at least
      if ( script.Length < 2 ) return null; // ERROR exit

      if ( double.TryParse( script[1], out double brg ) ) {
        var cmd = new CmdH( brg );
        if ( script.Length > 2 ) {
          // may have turnrate
          if ( double.TryParse( script[2], out double tRate ) ) {
            cmd.TurnRate = tRate;
          }
        }
        return cmd;
      }
      return null; // ERROR number parse
    }


    // CLASS

    public double Bearing { get; private set; } = 0;      // Bearing degm
    public double TurnRate { get; set; } = 3.0;   // Turnrate deg/sec (optional)

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="brg">New track [degm]</param>
    public CmdH( double brg )
    {
      Cmd = Cmd.H;
      Bearing = brg;
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      stream.WriteLine( $"H={Bearing:#0.0};{TurnRate:#0.0}" );
    }


  }
}

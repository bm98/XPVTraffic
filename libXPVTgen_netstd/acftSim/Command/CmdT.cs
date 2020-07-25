using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Turn Command
  /// e.g. T=230.5[;3.5]  # turn 230.5 deg (right) at an optional turnrate of 3.5 deg/sec
  /// </summary>
  class CmdT : CmdBase
  {
    /// <summary>
    /// Create a CmdT from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdT CreateFromStrings( string[] script )
    {
      // must be 2 at least
      if ( script.Length < 2 ) return null; // ERROR exit

      if ( double.TryParse( script[1], out double turn ) ) {
        var cmd = new  CmdT( turn );
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

    public double TurnAngle { get; private set; } = 0;  // +-deg
    public double TurnRate { get; set; } = 3.0;         // Turnrate deg/sec (optional)

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="angle">Turnangle [deg]</param>
    public CmdT( double angle )
    {
      Cmd = Cmd.T;
      TurnAngle = angle;
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      stream.WriteLine( $"T={TurnAngle:#0.0};{TurnRate:#0.0}" );
    }


  }
}

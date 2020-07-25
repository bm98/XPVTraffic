using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Speed Command
  /// e.g. S=140[;2.5]  # set ground speed to 140 kt with an optional ac/decel of +-2.5 kt/sec
  /// </summary>
  class CmdS : CmdBase
  {
    /// <summary>
    /// Create a CmdS from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdS CreateFromStrings( string[] script )
    {
      // must be 2 at least
      if ( script.Length < 2 ) return null; // ERROR exit

      if ( double.TryParse( script[1], out double gs ) ) {
        var cmd = new CmdS( gs );
        if ( script.Length > 2 ) {
          // may have accel
          if ( double.TryParse( script[2], out double accel ) ) {
            cmd.Accel = accel;
          }
          if ( script.Length > 3 ) {
            // may have immediate
            if ( int.TryParse( script[3], out int immediate ) ) {
              cmd.Immediate = ( immediate > 0 );
            }
          }
        }
        return cmd;
      }
      return null; // ERROR number parse
    }


    // CLASS

    public double GS { get; private set; } = 0;  // Ground speed kt
    public double Accel { get; set; } = 3.0;     // Accel  kt/sec (optional)
    public bool Immediate { get; set; } = false; // Set speed immediately - used for IFR startup (don't confuse LT with slow speeds..)
    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="gs">Groundspeed [kt]</param>
    public CmdS( double gs )
    {
      Cmd = Cmd.S;
      GS = gs;
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      if ( Immediate ) {
        stream.WriteLine( $"S={GS:#0.0};{Accel:#0.00};1" );
      }
      else {
        stream.WriteLine( $"S={GS:#0.0};{Accel:#0.00}" ); // don't flag if not immediate
      }
    }


  }
}

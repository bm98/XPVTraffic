using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Distance Command
  /// e.g. D=1.5  # go straight for 1.5 nm
  /// </summary>
  class CmdD : CmdBase
  {
    /// <summary>
    /// Create a CmdD from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdD CreateFromStrings( string[] script )
    {
      // must be 2 at least
      if ( script.Length < 2 ) return null; // ERROR exit

      if ( double.TryParse( script[1], out double dist ) ) {
        return new CmdD( dist );
      }
      return null; // ERROR number parse
    }


    // CLASS

    public double Dist { get; private set; } = 0.0;  // segment length nm

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="dist">Distance [nm]</param>
    public CmdD( double dist )
    {
      Cmd = Cmd.D;
      Dist = dist;
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      stream.WriteLine( $"D={Dist:#0.00}" );
    }

  }
}

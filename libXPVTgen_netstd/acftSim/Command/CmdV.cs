using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A vertical Climb / Descend Command
  /// e.g. V=1200;4000  # Climb or descend to 4000ft at 1200 ft/min
  /// </summary>
  class CmdV : CmdBase
  {
    /// <summary>
    /// Create a CmdV from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdV CreateFromStrings( string[] script )
    {
      // must be 3 at least
      if ( script.Length < 3 ) return null; // ERROR exit

      if ( double.TryParse( script[1], out double vs ) && double.TryParse( script[2], out double alt ) ) {
        var cmd = new CmdV( Math.Abs( vs ), alt );// can use old scripts with +- VS, new determines sign based on delta Alt
        return cmd;
      }
      return null; // ERROR number parse
    }


    // CLASS

    public double VSI { get; private set; } = 0;     // Vertical rate ft/min
    public double AltAGL { get; private set; } = 0;  // Target altitude ft AGL

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="vsi">Vertical rate [ft/min]</param>
    /// <param name="altAgl">Altitude AGL [ft]</param>
    public CmdV( double vsi, double altAgl )
    {
      Cmd = Cmd.V;
      VSI = vsi;
      AltAGL = altAgl;
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      stream.WriteLine( $"V={VSI:#0};{AltAGL:#0}" );
    }


  }
}

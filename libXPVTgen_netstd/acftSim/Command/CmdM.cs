using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Height Base MSL Command
  /// e.g. M=1  # base for V commands is MSL not AGL
  /// e.g. M=0  # base for V commands is AGL
  /// e.g. M=1;alt  # base for V commands is MSL and set new alt
  /// </summary>
  class CmdM : CmdBase
  {
    /// <summary>
    /// Create a CmdM from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdM CreateFromStrings( string[] script )
    {
      // must be 2 at least
      if ( script.Length < 2 ) return null; // ERROR exit

      if ( int.TryParse( script[1], out int mslBased ) ) {
        double altMsl = -1; // default - not used
        if ( script.Length > 2 ) {
          // may have altMSL
          if ( double.TryParse( script[2], out double alt ) ) {
            altMsl = alt;
          }
        }
        return new CmdM( mslBased > 0, altMsl );
      }
      return null; // ERROR number parse
    }


    // CLASS

    public bool MslBased { get; private set; } =false;
    public double AltMsl { get; private set; } = -1; // Optional to set an absolute alt when switching, does not apply if <0

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="msl">Msl based [bool]</param>
    public CmdM( bool msl )
    {
      Cmd = Cmd.M;
      MslBased = msl;
    }

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="msl">Msl based [bool]</param>
    public CmdM( bool msl, double alt )
    {
      Cmd = Cmd.M;
      MslBased = msl;
      AltMsl = ( MslBased ) ? alt : -1; // applies only if switching to MSL
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      string flag = ( MslBased ) ? "1" : "0";
      stream.WriteLine( $"M={flag};{AltMsl:#0}" );
    }


  }
}

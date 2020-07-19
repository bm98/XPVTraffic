using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Distance Command
  /// e.g. D=1.5  # go straight for 1.5 nm
  /// </summary>
  class CmdD : CmdBase
  {
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

  }
}

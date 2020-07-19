using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Speed Command
  /// e.g. S=140[;2.5]  # set ground speed to 140 kt with an optional ac/decel of +-2.5 kt/sec
  /// </summary>
  class CmdS : CmdBase
  {

    public double GS { get; private set; } = 0;  // Ground speed kt
    public double Accel { get; set; } = 2.0;     // Accel  kt/sec (optional)

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="gs">Groundspeed [kt]</param>
    public CmdS( double gs )
    {
      Cmd = Cmd.S;
      GS = gs;
    }

  }
}

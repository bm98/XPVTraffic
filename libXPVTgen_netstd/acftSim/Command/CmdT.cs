using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Turn Command
  /// e.g. T=230.5[;3.5]  # turn 230.5 deg (right) at an optional turnrate of 3.5 deg/sec
  /// </summary>
  class CmdT : CmdBase
  {
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

  }
}

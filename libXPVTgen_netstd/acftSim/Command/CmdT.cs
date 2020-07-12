using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Turn Command
  /// </summary>
  class CmdT:CmdBase
  {
    public double TurnAngle = 0;

    public CmdT()
    {
      Cmd = Cmd.T;
    }

  }
}

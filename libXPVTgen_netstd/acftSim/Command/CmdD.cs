using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Distance Command
  /// </summary>
  class CmdD:CmdBase
  {
    public double Dist = 0.0;

    public CmdD()
    {
      Cmd = Cmd.D;
    }

  }
}

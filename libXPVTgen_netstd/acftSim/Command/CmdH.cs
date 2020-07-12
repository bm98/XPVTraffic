using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Heading Command
  /// </summary>
  class CmdH:CmdBase
  {

    public double Heading = 0;

    public CmdH()
    {
      Cmd = Cmd.H;
    }

  }
}

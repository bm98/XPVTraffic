using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A Speed Command
  /// </summary>
  class CmdS:CmdBase
  {

    public int TAS = 0;

    public CmdS()
    {
      Cmd = Cmd.S;
    }

  }
}

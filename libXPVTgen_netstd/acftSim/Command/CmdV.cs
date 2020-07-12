using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// An Ascend / Descend Command
  /// </summary>
  class CmdV:CmdBase
  {

    public int VSI = 0;
    public int AltAGL = 0;

    public CmdV()
    {
      Cmd = Cmd.V;
    }

  }
}

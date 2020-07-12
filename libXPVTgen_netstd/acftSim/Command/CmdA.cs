using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Aircraft Command
  /// </summary>
  class CmdA : CmdBase
  {

    public string AcftType = "";
    public string RwyID = "";

    public CmdA()
    {
      Cmd = Cmd.A;
    }


  }
}

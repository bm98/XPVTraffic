using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Height Base MSL Command
  /// e.g. M=1  # base for V commands is MSL not AGL
  /// e.g. M=0  # base for V commands is AGL
  /// </summary>
  class CmdM : CmdBase
  {
    public bool MslBased { get; private set; } =false;

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="msl">Msl based [bool]</param>
    public CmdM( bool msl )
    {
      Cmd = Cmd.M;
      MslBased = msl;
    }

  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Head towards (Set trak to bearing) Command
  /// e.g. H=221.5[;3.5]  # head towards 221.5 degm and turn at a rate of 3.5 deg/sec
  /// </summary>
  class CmdH : CmdBase
  {
    public double Bearing { get; private set; } = 0;      // Bearing degm
    public double TurnRate { get; set; } = 3.0;   // Turnrate deg/sec (optional)

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="brg">New track [degm]</param>
    public CmdH( double brg )
    {
      Cmd = Cmd.H;
      Bearing = brg;
    }

  }
}

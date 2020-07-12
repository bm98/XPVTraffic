using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  public enum Cmd
  {
    X=-1,   // UNDEF

    A = 0,  // Aircraft
    S,      // Set Speed
    V,      // Set Ascend / Descend
    D,      // Set Dist Leg
    T,      // Set Turn
    H,      // Set Heading

    E,      // END Segment (auto created by reader)
  }

  abstract class CmdBase
  {
    public Cmd Cmd { get; set; } = Cmd.X;

  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Defined Commands
  /// </summary>
  public enum Cmd
  {
    X=-1,   // UNDEF

    A = 0,  // Aircraft
    S,      // Set Speed
    V,      // Set Climb / Descend to AGL
    D,      // Set Dist Leg
    T,      // Set Turn
    H,      // Set Heading
    G,      // Goto Destination point  (for IFR only)
    M,      // Set Base MSL (or back to AGL default)

    E,      // END Segment (auto created by reader)
  }

  /// <summary>
  /// Class to derive all commands from
  /// </summary>
  abstract class CmdBase
  {
    public Cmd Cmd { get; set; } = Cmd.X;

  }
}

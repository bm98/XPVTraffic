using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A vertical Climb / Descend Command
  /// e.g. V=1200;4000  # Climb or descend to 4000ft at 1200 ft/min
  /// </summary>
  class CmdV : CmdBase
  {

    public double VSI { get; private set; } = 0;     // Vertical rate ft/min
    public double AltAGL { get; private set; } = 0;  // Target altitude ft AGL

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="vsi">Vertical rate [ft/min]</param>
    /// <param name="altAgl">Altitude AGL [ft]</param>
    public CmdV( double vsi, double altAgl )
    {
      Cmd = Cmd.V;
      VSI = vsi;
      AltAGL = altAgl;
    }

  }
}

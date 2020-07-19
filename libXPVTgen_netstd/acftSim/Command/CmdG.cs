using System;
using System.Collections.Generic;
using System.Text;
using libXPVTgen.coordlib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Goto Lat Lon Command
  /// e.g. G=47.2;8.5[;3.5]  # head towards pt 47.2;8.5 and turn at a rate of 3.5 deg/sec
  /// </summary>
  class CmdG : CmdBase
  {
    public LatLon Destination { get; private set; } = new LatLon( ); // dest point Lat/Lon
    public double TurnRate { get; private set; } = 9.0;   // Turnrate deg/sec (optional); steep due to calc. inaccuracies (we are not a GPS...)

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="destination">Dest to goto [lat/lon]</param>
    public CmdG( LatLon destination )
    {
      Cmd = Cmd.G;
    }

  }
}

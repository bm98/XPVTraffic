using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A simulation for a Virtual General Aviation type aircrafts
  /// </summary>
  internal class GAModel : AcftModel
  {
    /// <summary>
    /// cTor: Init a new VFR flight model
    /// </summary>
    /// <param name="acftID">Aircraft registration</param>
    /// <param name="route">Segment list to fly</param>
    public GAModel( CmdList route )
      : base( route )
    {
      // set VFR max and consts
      c_maxGS_kt = 180;
      c_maxVRate_ftPmin = 1500; // +-
      c_maxAlt_ftMSL = 10_000;
      c_accel_ktPsec = 2.0;     // default acceleration kt/s
      c_tRate_degPsec = 3.0;    // turnrate deg/sec
    }

  }
}

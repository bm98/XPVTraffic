using System;
using System.Collections.Generic;
using System.Text;
using libXPVTgen.coordlib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A simulation for a Virtual Airliner, Jet type aircrafts
  /// Used for IFR flights
  /// </summary>
  internal class JetModel : AcftModel
  {
    /// <summary>
    /// cTor: Init a new IFR flight model
    /// </summary>
    /// <param name="route">Segment list to fly</param>
    public JetModel( CmdList route )
      : base( route )
    {
      // set Jet max and consts
      c_maxGS_kt = 500;         // on the high side for this purpose
      c_maxVRate_ftPmin = 2500; // +-
      c_maxAlt_ftMSL = 45_000;
      c_accel_ktPsec = 5.0;     // default acceleration kt/s
      c_tRate_degPsec = 3.0;    // turnrate deg/sec
    }

  }
}


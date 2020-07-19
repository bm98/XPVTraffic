using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.acftSim;
using libXPVTgen.coordlib;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// An IFR virtual Aircraft for Airline and Jet Operations
  /// </summary>
  internal class IFRvAcft : VAcft
  {
    // CLASS

    /// <summary>
    /// cTor: Create a new Airliner, Jet aircraft type
    /// </summary>
    /// <param name="route">Segment list to fly</param>
    public IFRvAcft( CmdList route )
        : base( route )
    {
      m_model = new JetModel( route );
    }

  }
}

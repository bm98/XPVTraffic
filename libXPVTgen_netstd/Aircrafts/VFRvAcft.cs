using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.acftSim;
using libXPVTgen.coordlib;
using libXPVTgen.my_rwylib;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// A VFR virtual Aircraft for General Aviation 
  /// </summary>
  internal class VFRvAcft : VAcft
  {


    // CLASS

    /// <summary>
    /// cTor: Create a new aircraft model
    /// Creates a GA model for the smaller AC types, a Jet model for all others
    /// Jet model allows for extended operating ranges
    /// </summary>
    /// <param name="route">A commandlist of segments to fly</param>
    public VFRvAcft( CmdList route )
      : base( route )
    {
      // select the Flight model according to acft type (use the types above for GA)
      if ( AircraftSelection.GA_AircraftTypes.Contains( route.Descriptor.AircraftType ) ) {
        m_model = new GAModel( route );
      }
      else {
        m_model = new JetModel( route );
      }
    }

  }
}

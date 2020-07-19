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
    // Selection of GA Aircraft types avilable from the Bluebird CSL library (may be extend for heuristic GAs in LiveTraffic)
    public static List<string> GA_AircraftTypes = new List<string>( ) { "C150", "C172", "C421", "BE20", "PC9" };


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
      if ( GA_AircraftTypes.Contains( route.Descriptor.AircraftType ) ) {
        m_model = new GAModel( route );
      }
      else {
        m_model = new JetModel( route );
      }
    }

  }
}

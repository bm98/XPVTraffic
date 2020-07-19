using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.acftSim;
using libXPVTgen.coordlib;
using libXPVTgen.my_awlib;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// A Virtual Aircraft
  /// </summary>
  internal abstract class VAcft : Acft

  {
    // the aircraft model
    protected AcftModel m_model = null; 

    // vars not maintained in the model
    protected string m_hex = "";
    protected string m_type = "";
    protected string m_tailReg = "";
    protected string m_callsign = "";
    protected string m_from = "";
    protected string m_to = "";

    // Public properties for the handler
    public double VSI => m_model.VRate_ftPmin; // from model
    public double TRK => m_model.Trk_degm; // from model
    public double GS => m_model.GS_kt; // from model
    public long TStamp => m_model.TimeStamp_sec; // from model
    public bool Airborne => m_model.Airborne; // from model

    public string AcftType => m_type;
    public string AcftCallsign => m_callsign;
    public string AcftTailReg => m_tailReg;
    public string AcftFrom => m_from;
    public string AcftTo => m_to;
    public string AcftHex => m_hex;


    /// <summary>
    /// cTor: Create a new aircraft
    /// </summary>
    /// <param name="route">Segment list to fly</param>
    public VAcft( CmdList route )
    {
      base.ID = route.Descriptor.AircraftTailReg;
      m_hex = Convert.ToInt32( route.Descriptor.AircraftRegNumber.ToString( ), 16 ).ToString( ); // hex-> same literal as number
      m_tailReg = route.Descriptor.AircraftTailReg;
      m_callsign = route.Descriptor.AircraftCallsign;
      m_type = route.Descriptor.AircraftType;
      m_from = route.Descriptor.Start_IcaoID;
      m_to = route.Descriptor.End_IcaoID;

      m_model = null;
    }

    /// <summary>
    /// True if the acft is no longer active (end of route)
    /// </summary>
    public bool Out => m_model.Out; // from model

    /// <summary>
    /// Increment Model
    /// either at realtime pace or simulated pace
    /// </summary>
    /// <param name="stepSec">sim step seconds (default = -1 to use the realtime clock)</param>
    public void StepModel( int stepSec )
    {
      m_model?.PaceModel( stepSec );
      // copy to generic - should be done better..
      LatLon = m_model.Pos; // from model
      Alt_ft = (int)m_model.Alt_ftMsl; // from model
    }

    /// <summary>
    /// Increment Model at realtime pace
    /// </summary>
    public void StepModel()
    {
      this.StepModel( -1 );
    }

  }
}

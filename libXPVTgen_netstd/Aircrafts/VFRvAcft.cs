using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;
using libXPVTgen.acftSim;
using libXPVTgen.my_rwylib;

namespace libXPVTgen.Aircrafts
{
  internal class VFRvAcft : VAcft
  {
    private static readonly Random m_random = new Random( );

    private VFRModel m_vFR = null;
    private rwyRec m_rwy = null;

    private string m_hex = "";
    private string m_type = "";
    private string m_reg = "";
    private string m_from = "";
    private string m_to = "";

    public override double VSI => m_vFR.VSI; // from model
    public override double HDG => m_vFR.Trk; // from model
    public override double TAS => m_vFR.TAS; // from model

    public override string AcftHex => m_hex;
    public override string AcftType => m_type;
    public override string AcftReg => m_reg;

    public override string AcftFrom => m_from;
    public override string AcftTo => m_to;

    public override long TStamp => m_vFR.TimeStamp; // from model

    public override bool Out => m_vFR.Out; // from model

    /// <summary>
    /// Increment Model at realtime pace
    /// </summary>
    public override void StepModel( )
    {
      StepModel( -1 );
    }

    /// <summary>
    /// Increment Model
    /// either at realtime pace or simulated pace
    /// </summary>
    /// <param name="stepSec">sim step seconds (default = -1 to use the realtime clock)</param>
    public void StepModel( int stepSec )
    {
      m_vFR?.PaceModel( stepSec );
      // copy to generic - should be done better..
      LatLon = m_vFR.Pos; // from model
      Alt_ft = (int)m_vFR.AltMsl; // from model
    }

    public VFRvAcft( string ID, rwyRec rwyRef, CmdList route, string aType, string aHex )
    {
      base.ID = ID;
      m_type = aType;
      m_hex = aHex;
      m_rwy = rwyRef.DeepCopy( );
      m_from = m_rwy.icao_id;
      m_to = "VFR->";

      m_vFR = new VFRModel( ID, m_rwy.start_latlon, m_rwy.elevation, m_rwy.brg, route );

    }

  }
}

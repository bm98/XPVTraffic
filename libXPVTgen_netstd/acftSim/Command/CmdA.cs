using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;
using libXPVTgen.my_awlib;
using libXPVTgen.my_rwylib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Aircraft Command
  /// e.g. A=C172[;LSZH_RW14]  # use a Cessna 172 and optionally prefer LSZH Runway 14 to start from
  /// </summary>
  class CmdA : CmdBase
  {
    private static string AcftTailPrefix = "VX-";    // Virtual ICAO registration country prefix (VX seems not in use)

    // CLASS

    public int AircraftRegNumber { get; private set; } = 0;       // (4 digit) unique number - created in cTor

    public string AircraftType { get; private set; } = "C172";    // aircraft type ICAO type (defaults to a Cessna)
    public string AircraftOperator { get; private set; } = "YYY"; // 7 letter ICAO operator code or YYYnnnn for GA (default)
    public string AircraftCallsign=> $"{AircraftOperator}{AircraftRegNumber:0000}"; // 7 letter ICAO operator code or YYYnnnn for GA (default)
    public string AircraftTailReg => $"{AcftTailPrefix}{AircraftRegNumber:0000}";  // Virtual ICAO registration

    public string RunwayPreference { get; set; } = "";            // start icao id name (fix or nav or rwy_id)
    public bool RunwayPrefStrict { get; set; } = false;           // do we have to obey the Runway (true)

    // need to fill these when completing the route
    public string Start_IcaoID { get; private set; } = "";         // start icao id name (fix or nav or rwy_id)
    public string End_IcaoID { get; private set; } = "";           // end icao id name (fix or nav)
    public LatLon StartPos_latlon { get; private set; } = new LatLon( );  // origin of this route
    public double StartAlt_ftMsl { get; private set; } = 0;        // Alt ft msl of the origin
    public double StartBrg_degm { get; private set; } = 0;         // Flight direction degm at origin
    public double RwyAlt_ftMsl { get; private set; } = 0;          // Alt ft msl of the runway (if there was no runway let it 0)

    /// <summary>
    /// cTor: with an Aircraft Type
    /// assigns a YYYnnnn callsign for GA flights
    /// </summary>
    /// <param name="acftType">An aircraft type name</param>
    public CmdA( string acftType )
    {
      Cmd = Cmd.A;
      AircraftType = acftType;
    }

    /// <summary>
    /// cTor: with Aircraft Type and Airline Operator Code (3 letter ICAO code)
    /// assigns the operator code callsign (CODnnnn)
    /// </summary>
    /// <param name="acftType">The aircraft type name</param>
    /// <param name="airline">The ICAO operator name of the airline</param>
    public CmdA( string acftType, string airline )
    {
      Cmd = Cmd.A;
      AircraftType = acftType;
      AircraftOperator = airline;
    }

    /// <summary>
    /// Completes the Cmd from a Runway record
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="rwy">A Runway to start from</param>
    public void InitFromRunway( int acftNo, rwyRec rwy )
    {
      AircraftRegNumber = acftNo;
      StartAlt_ftMsl = rwy.elevation; // base for relative alt commands is the runway elevation
      RwyAlt_ftMsl = rwy.elevation;   // need to know if we are airborne or not
      StartBrg_degm = rwy.brg;        // start direction
      StartPos_latlon = new LatLon( rwy.start_latlon );
      Start_IcaoID = rwy.icao_id;     // final start location ID
      End_IcaoID = "VFR->";           // final end location ID - actually not known..
    }

    /// <summary>
    /// Initializes the Cmd from an Airway record
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="awy">A Airway to start from</param>
    /// <param name="alt">Initial altitude</param>
    public void InitFromAirway( int acftNo, awyRec awy, double alt )
    {
      AircraftRegNumber = acftNo;
      StartAlt_ftMsl = alt;             // all Alt commands are absolute above SL (use M command to switch)
      RwyAlt_ftMsl = 0;                 // we are always airborne..
      StartBrg_degm = awy.brg;          // initial track direction
      StartPos_latlon = new LatLon( awy.start_latlon );
      Start_IcaoID = awy.start_icao_id; // final start location ID
      End_IcaoID = "IFR->";             // temp end location ID, to be completed with FinishFromAirway(last segment)
    }

    /// <summary>
    /// Completes the Cmd from an Airway record
    /// </summary>
    /// <param name="awy">An Airway to finish with</param>
    public void FinishFromAirway( awyRec awy )
    {
      End_IcaoID = awy.end_icao_id;       // final end location ID
    }


  }
}

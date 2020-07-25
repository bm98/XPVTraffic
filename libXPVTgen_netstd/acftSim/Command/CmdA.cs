using System;
using System.Collections.Generic;
using System.IO;
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
    public enum FlightT
    {
      Invalid = 0,
      Runway = 1,
      Airway = 2,
      MsgRelative = 3,
      MsgAbsolute = 4,
    }
    public static string FlightTypeS( FlightT ft )
    {
      return ft.ToString( );
    }

    public static FlightT AsFlightType( string ft )
    {
      if ( Enum.TryParse( ft, out FlightT flightType ) ) {
        return flightType;
      }
      return FlightT.Invalid;
    }

    /// <summary>
    /// Create a CmdA from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdA CreateFromStrings( string[] script )
    {
      /*
        0     1      2          3 ..   
        A=AcftType;Runway[;Pref.Runway[;Strict]]	# Type 1 – default Alt Mode => AGL based
        A=AcftType;Airway			# Type 2 – default Alt Mode => MSL based
        A=AcftType;MsgRelative;Alt;GS		# Type 3 – default Alt Mode => MSL based; StartPos random; 
        A=AcftType;MsgAbsolute;Alt;GS;Lat;Lon;Hdg	# Type 4 – default Alt Mode => MSL based; StartPos defined;        
       */
      // must be 3 at least
      if ( script.Length < 3 ) return null; // ERROR exit

      var acType = script[1];
      var fType = AsFlightType( script[2] );
      if ( fType == FlightT.Invalid ) return null; // ERROR exit

      var cmd = new CmdA( acType, fType );
      switch ( fType ) {
        case FlightT.Runway: {// this is usually not used but to have it complete
            if ( script.Length > 3 ) {
              // pref runway
              cmd.RunwayPreference = script[3].Trim( ).ToUpperInvariant( ); // optional, preferred RWY
              if ( script.Length > 4 ) {
                // pref runway strict
                cmd.RunwayPrefStrict = script[4].Trim( ).ToUpperInvariant( ) == "S"; // optional, preferred RWY is strict
              }
            }
            return cmd;
          }
        case FlightT.Airway:
          return cmd; // no further parameters

        case FlightT.MsgRelative: {
            if ( script.Length < 5 ) return null; // ERROR exit
            bool pass = true;
            pass &= double.TryParse( script[3], out double alt );
            pass &= double.TryParse( script[4], out double gs );
            if ( !pass ) return null; // ERROR exit - number conversion error
            cmd.StartAlt_ftMsl = alt;
            cmd.StartGS_kn = gs;
            return cmd;
          }
        case FlightT.MsgAbsolute: {
            if ( script.Length < 8 ) return null; // ERROR exit
            bool pass = true;
            pass &= double.TryParse( script[3], out double alt );
            pass &= double.TryParse( script[4], out double gs );
            pass &= double.TryParse( script[5], out double lat );
            pass &= double.TryParse( script[6], out double lon );
            pass &= double.TryParse( script[7], out double hdg );
            if ( !pass ) return null; // ERROR exit - number conversion error
            cmd.StartAlt_ftMsl = alt;
            cmd.StartGS_kn = gs;
            cmd.StartPos_latlon = new LatLon( lat, lon );
            cmd.StartBrg_degm = hdg;
            return cmd;
          }
        default:
          return null;
      }
    }


    private static string AcftTailPrefix = "VX-";    // Virtual ICAO registration country prefix (VX seems not in use)

    // CLASS

    public int AircraftRegNumber { get; private set; } = 0;       // (4 digit) unique number - created in cTor

    public string AircraftType { get; private set; } = "C172";    // aircraft type ICAO type (defaults to a Cessna)
    public FlightT FlightType { get; private set; } = FlightT.Invalid; // The type of this flight
    public string AircraftOperator { get; private set; } = "YYY"; // 7 letter ICAO operator code or YYYnnnn for GA (default)
    public string AircraftCallsign { get; private set; } = "";    // 7 letter ICAO operator code or YYYnnnn for GA (default)
    public string AircraftTailReg { get; private set; } = "";     // Virtual ICAO registration

    public string RunwayPreference { get; set; } = "";            // start icao id name (fix or nav or rwy_id)
    public bool RunwayPrefStrict { get; set; } = false;           // do we have to obey the Runway (true)

    // need to fill these when completing the route
    public string Start_IcaoID { get; private set; } = "";         // start icao id name (fix or nav or rwy_id)
    public string End_IcaoID { get; private set; } = "";           // end icao id name (fix or nav)
    public LatLon StartPos_latlon { get; private set; } = new LatLon( );  // origin of this route
    public double StartAlt_ftMsl { get; private set; } = 0;        // Alt ft msl of the origin
    public double StartBrg_degm { get; private set; } = 0;         // Flight direction degm at origin
    public double StartGS_kn { get; private set; } = 0;            // Initial speed kn at origin
    public double RwyAlt_ftMsl { get; private set; } = 0;          // Alt ft msl of the runway (if there was no runway let it 0)

    /// <summary>
    /// cTor: with an Aircraft Type
    /// assigns a YYYnnnn callsign for GA flights
    /// </summary>
    /// <param name="acftType">An aircraft type name</param>
    public CmdA( string acftType, FlightT ft )
    {
      Cmd = Cmd.A;
      AircraftType = acftType;
      FlightType = ft;
    }

    /// <summary>
    /// cTor: with Aircraft Type and Airline Operator Code (3 letter ICAO code)
    /// assigns the operator code callsign (CODnnnn)
    /// </summary>
    /// <param name="acftType">The aircraft type name</param>
    /// <param name="airline">The ICAO operator name of the airline</param>
    public CmdA( string acftType, FlightT ft, string airline )
    {
      Cmd = Cmd.A;
      AircraftType = acftType;
      FlightType = ft;
      AircraftOperator = airline;
      AircraftCallsign = $"{AircraftOperator}{AircraftRegNumber:0000}";
    }

    /// <summary>
    /// Allows to update the aircraft type
    /// </summary>
    /// <param name="act">A new aircraft type</param>
    public void UpdateAcftType(string act )
    {
      AircraftType = act.Trim().ToUpperInvariant( );
    }


    /// <summary>
    /// Flight Type Runway
    /// Initializes the Cmd for flying - 
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="rwy">A Runway to start from</param>
    public void InitFromRunway( int acftNo, rwyRec rwy )
    {
      AircraftRegNumber = acftNo;
      AircraftTailReg = $"{AcftTailPrefix}{AircraftRegNumber:0000}";
      AircraftCallsign = $"{AircraftOperator}{AircraftRegNumber:0000}";
      StartPos_latlon = new LatLon( rwy.start_latlon );
      StartAlt_ftMsl = rwy.elevation; // base for relative alt commands is the runway elevation
      StartBrg_degm = rwy.brg;        // start direction
      StartGS_kn = 0;                 // start speed
      RwyAlt_ftMsl = rwy.elevation;   // need to know if we are airborne or not
      Start_IcaoID = rwy.icao_id;     // final start location ID
      End_IcaoID = "VFR->";           // final end location ID - actually not known..
    }

    /// <summary>
    /// Flight Type Airway
    /// Initializes the Cmd for flying - 
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="awy">A Airway to start from</param>
    /// <param name="alt">Initial altitude</param>
    public void InitFromAirway( int acftNo, awyRec awy, double alt, double gs )
    {
      AircraftRegNumber = acftNo;
      AircraftTailReg = $"{AcftTailPrefix}{AircraftRegNumber:0000}";
      AircraftCallsign = $"{AircraftOperator}{AircraftRegNumber:0000}";
      StartPos_latlon = new LatLon( awy.start_latlon );
      StartAlt_ftMsl = alt;             // all Alt commands are absolute above SL (use M command to switch)
      StartBrg_degm = awy.brg;          // initial track direction
      StartGS_kn = gs;                  // we have an initial speed
      RwyAlt_ftMsl = 0;                 // we are always airborne..
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

    /// <summary>
    /// Flight Type MsgRelative
    /// Initializes the Cmd for flying - 
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="rwy">A Runway to start from</param>
    /// <param name="callSign">A callsign</param>
    public void InitFromMsgRelative( int acftNo, rwyRec rwy, string callSign )
    {
      AircraftRegNumber = acftNo;
      StartPos_latlon = new LatLon( rwy.start_latlon );
      StartBrg_degm = rwy.brg;        // start direction
      Start_IcaoID = rwy.icao_id;     // final start location ID
      End_IcaoID = "MSG_EPOS";        // artificial, usually not known
      AircraftTailReg = $"{AcftTailPrefix}{AircraftRegNumber:0000}";
      if ( !string.IsNullOrEmpty( callSign ) ) {
        AircraftOperator = callSign.Trim( ).ToUpperInvariant( );
      }
      AircraftCallsign = $"{AircraftOperator}{AircraftRegNumber:0000}";
    }

    /// <summary>
    /// Flight Type MsgAbsolute
    /// Initializes the Cmd for flying - 
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="callSign">A callsign</param>
    public void InitFromMsgAbsolute( int acftNo, string callSign )
    {
      AircraftRegNumber = acftNo;
      Start_IcaoID = "MSG_SPOS"; // artificial, usually not known
      End_IcaoID = "MSG_EPOS";   // artificial, usually not known
      AircraftTailReg = $"{AcftTailPrefix}{AircraftRegNumber:0000}";
      if ( !string.IsNullOrEmpty( callSign ) ) {
        AircraftOperator = callSign.Trim( ).ToUpperInvariant( );
      }
      AircraftCallsign = $"{AircraftOperator}{AircraftRegNumber:0000}";
    }

    /// <summary>
    /// Flight Type MsgRelative
    /// Create a complete Cmd for scripting - 
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="rwy">A Runway to start from</param>
    /// <param name="callSign">A callsign</param>
    public void CreateForMsgRelative( double startAlt, double startGS )
    {
      StartAlt_ftMsl = startAlt;
      StartGS_kn = startGS;
    }

    /// <summary>
    /// Flight Type MsgAbsolute
    /// Create a complete Cmd for scripting - 
    /// </summary>
    /// <param name="acftNo">The aircraft reg number</param>
    /// <param name="callSign">A callsign</param>
    public void CreateForMsgAbsolute(LatLon startPos, double startBrg, double startAlt, double startGS )
    {
      StartPos_latlon = new LatLon( startPos );
      StartBrg_degm = startBrg;
      StartAlt_ftMsl = startAlt;
      StartGS_kn = startGS;
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      string r = $"A={AircraftType};{FlightTypeS( FlightType )}"; // for all
      switch ( FlightType ) {
        case FlightT.Runway: // this is usually not used but to have it complete
          if ( !string.IsNullOrEmpty( RunwayPreference.ToUpperInvariant( ) ) ) {
            r += $";{RunwayPreference}";
            if ( RunwayPrefStrict ) {
              r += $";S";
            }
          }
          stream.WriteLine( r );
          return;
        case FlightT.Airway:
          stream.WriteLine( r );
          return;
        case FlightT.MsgRelative:
          r += $";{StartAlt_ftMsl:#0};{StartGS_kn:#0.0000}";
          stream.WriteLine( r );
          return;
        case FlightT.MsgAbsolute:
          r += $";{StartAlt_ftMsl:#0};{StartGS_kn:#0.0000};{StartPos_latlon.Lat:#0.000000};{StartPos_latlon.Lon:#0.000000};{StartBrg_degm:#0.0}";
          stream.WriteLine( r );
          return;
        default:
          return;
      }
    }


  }
}

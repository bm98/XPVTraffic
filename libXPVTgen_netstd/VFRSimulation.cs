using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

using libXPVTgen.Aircrafts;
using libXPVTgen.LiveTraffic;
using libXPVTgen.my_awlib;
using libXPVTgen.my_rwylib;
using libXPVTgen.acftSim;
using libXPVTgen.kmllib;
using libXPVTgen.coordlib;

namespace libXPVTgen
{
  /// <summary>
  /// Used to simulate one VFR script or a random IFR flight and create a KML file for it
  /// </summary>
  public class VFRSimulation
  {
    // Traffic 
    private awyDatabase AWYDB = new awyDatabase( );
    private rwyDatabase RWYDB = new rwyDatabase( );
    private List<CmdList> CMDS = null;

    // configs
    private int m_stepLen_sec = 2;     // update pace for LiveTraffic (use 1..3 for VFR modelling)

    // my Aircraft
    private UserAcft m_userAcft = null;
    private DateTime m_lastUpdate = DateTime.Now;

    /// <summary>
    /// True when ready
    /// </summary>
    public bool Valid { get; private set; } = false;
    /// <summary>
    /// Error indication if not Valid
    /// </summary>
    public string Error { get; private set; } = "";

    /// <summary>
    /// cTor: Create all infrastructure
    ///       Proceed with EstablishLink
    /// </summary>
    /// <param name="dat_Path">The path to my_awy.dat</param>
    public VFRSimulation( string dat_Path, int stepLen_sec, bool logging )
    {
      Logger.Instance.Reset( );
      Logger.Instance.Logging = logging; // user

#if DEBUG
      Logger.Instance.Logging = true;
#endif

      Logger.Instance.Log( $"VFRSimulation-Create @: {DateTime.Now.ToString( )}" );
      string eawy = Path.Combine( dat_Path, DBCreator.MyAwyDbName );
      if ( !File.Exists( eawy ) ) {
        Error = $"Error: my_awy.dat does not exist: {eawy}";
        Logger.Instance.Log( $"VFRSimulation: {Error}" );
        return; // ERROR exit
      }

      string erwy = Path.Combine( dat_Path, DBCreator.MyRwyDbName );
      if ( !File.Exists( erwy ) ) {
        Error = $"Error: my_rwy.dat does not exist: {erwy}";
        Logger.Instance.Log( $"VFRSimulation: {Error}" );
        return; // ERROR exit
      }

      string ret = awyReader.ReadDb( ref AWYDB, eawy );
      if ( !string.IsNullOrEmpty( ret ) ) {
        Error = $"Error: my_awy.dat read failed: {ret}";
        Logger.Instance.Log( $"VFRSimulation: {Error}" );
        return; // ERROR exit
      }

      ret = rwyReader.ReadDb( ref RWYDB, erwy );
      if ( !string.IsNullOrEmpty( ret ) ) {
        Error = $"Error: my_rwy.dat read failed: {ret}";
        Logger.Instance.Log( $"VFRSimulation: {Error}" );
        return; // ERROR exit
      }

      string escript = Path.Combine( dat_Path, DBCreator.MyVfrScriptPath );
      if ( !Directory.Exists( escript ) ) {
        Error = $"Warning: script folder does not exist: {escript}";
        Logger.Instance.Log( $"VFRSimulation: {Error}" );
      }
      else {
        // Load Scripts
        CMDS = CmdReader.ReadScripts( escript );
        if ( CMDS.Count <= 0 ) {
          Error = $"Warning: script folder does not contain valid scripts";
          Logger.Instance.Log( $"VFRSimulation: {Error}" );
        }
      }

      m_stepLen_sec = stepLen_sec;

      Valid = true;
    }

    /// <summary>
    /// Dump the first 100 random IFR script start,end names
    /// </summary>
    public void DumpIFR()
    {
      var awys = AWYDB.GetSubtable( 100, 47.17, 8.5 ); // just around here..
      using ( var sw = new StreamWriter( "IFRscrips.log", false ) ) {
        for ( int i = 0; i < 100; i++ ) {
          var route = IFRroute.GetRandomFlight( awys, 1, "A333", "SIM" );
          sw.WriteLine( $"{i:000}\t{route.Descriptor.Start_IcaoID}\t{route.Descriptor.End_IcaoID}" );
        }
      }
    }

    /// <summary>
    /// Create and run one random IFR Flight Sim and write a KML of it
    /// - just for testing..
    /// </summary>
    /// <returns>True if OK</returns>
    public bool RunIFRSim()
    {
      var awys = AWYDB.GetSubtable( 100, 47.17, 8.5 ); // just around here..
      var route = IFRroute.GetRandomFlight( awys, 1, "A333", "SIM" );
      return RunSimFromScript( route );
    }

    /// <summary>
    /// Run one route script with an IFR model aircraft
    /// </summary>
    /// <param name="route">A complete route script to run</param>
    /// <returns>True if OK</returns>
    private bool RunSimFromScript( CmdList route )
    {
      string routeName = route.Descriptor.Start_IcaoID + "_" + route.Descriptor.End_IcaoID;

      var virtAcft = new IFRvAcft( route ); // use the Jet model
      var kmlFile = new KmlFile( );
      var kmlLine = new line {
        Name = routeName,
        LineColor = LineStyle.LT_Blue
      };
      do {
        virtAcft.StepModel( m_stepLen_sec ); // step the model at 2 sec until finished

        kmlLine.Add( new point {
          Position = new LatLon( virtAcft.LatLon ),
          Altitude_ft = virtAcft.Alt_ft,
          Heading = (int)virtAcft.TRK
        } );

      } while ( !virtAcft.Out );
      // setup Comm
      kmlFile.Lines.Add( kmlLine );
      kmlFile.WriteKML( routeName + ".kml" );
      return Valid;
    }


    /// <summary>
    /// Simulate the Model file given and write a kmlfile of the path
    /// same folder as the model file with the extension .kml
    /// </summary>
    /// <param name="vfrModelFile">The VFR Model File</param>
    /// <returns>True if successfull (else see Error content)</returns>
    public bool RunSimulation( string vfrModelFile, string fallBackRwy )
    {
      Error = "";
      Logger.Instance.Log( $"VFRSimulation-SetupSimulation for: {vfrModelFile}" );
      if ( !Valid ) return false;

      var route = CmdReader.ReadCmdScript( vfrModelFile );
      if ( !route.IsValid ) {
        Valid = false;
        Error = "File not found or invalid content";
        Logger.Instance.Log( Error );
        return false;
      }

      if ( route.Descriptor.FlightType == CmdA.FlightT.Runway ) {
        string rwID = route.Descriptor.RunwayPreference; // preferred one
        if ( string.IsNullOrEmpty( rwID ) ) rwID = fallBackRwy;
        var rwy = RWYDB.GetSubtable( rwID ); // search RWY
        if ( rwy.Count < 1 ) {
          Valid = false;
          Error = $"Runway: {route.Descriptor.Start_IcaoID} not found in Runway database, cannot continue";
          Logger.Instance.Log( Error );
          return false;
        }
        // actually my position
        m_userAcft = new UserAcft( );
        m_userAcft.NewPos( rwy.ElementAt( 0 ).Value.start_latlon );
        // the simulated aircraft
        route.Descriptor.InitFromRunway( 1, rwy.ElementAt( 0 ).Value ); // Complete the script
      }
      else if ( route.Descriptor.FlightType == CmdA.FlightT.Airway ) {
        return false; // not supported - use the one above...
      }
      else if ( route.Descriptor.FlightType == CmdA.FlightT.MsgRelative ) {
        string rwID = route.Descriptor.RunwayPreference; // preferred one
        if ( string.IsNullOrEmpty( rwID ) ) rwID = fallBackRwy;
        var rwy = RWYDB.GetSubtable( rwID ); // search RWY
        if ( rwy.Count < 1 ) {
          Valid = false;
          Error = $"Runway: {route.Descriptor.Start_IcaoID} not found in Runway database, cannot continue";
          Logger.Instance.Log( Error );
          return false;
        }
        // actually my position
        m_userAcft = new UserAcft( );
        m_userAcft.NewPos( rwy.ElementAt( 0 ).Value.start_latlon );
        // the simulated aircraft
        route.Descriptor.InitFromMsgRelative( 1, rwy.ElementAt( 0 ).Value, "SIM" ); // Complete the script
      }

      else if ( route.Descriptor.FlightType == CmdA.FlightT.MsgAbsolute ) {
        // actually my position
        m_userAcft = new UserAcft( );
        m_userAcft.NewPos( route.Descriptor.StartPos_latlon );
        // the simulated aircraft
        route.Descriptor.InitFromMsgAbsolute( 1, "SIM" ); // Complete the script
      }

      var virtAcft = new VFRvAcft( route ); // use the GA model
      var kmlFile = new KmlFile( );
      var kmlLine = new line {
        Name = Path.GetFileNameWithoutExtension( vfrModelFile ),
        LineColor = LineStyle.LT_Yellow
      };
      do {
        virtAcft.StepModel( m_stepLen_sec ); // step the model at 2 sec until finished

        kmlLine.Add( new point {
          Position = new LatLon( virtAcft.LatLon ),
          Altitude_ft = virtAcft.Alt_ft,
          Heading = (int)virtAcft.TRK
        } );

      } while ( !virtAcft.Out );
      // setup Comm
      kmlFile.Lines.Add( kmlLine );
      kmlFile.WriteKML( vfrModelFile + ".kml" );
      return Valid;
    }

  }
}

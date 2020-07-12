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

namespace libXPVTgen
{
  /// <summary>
  /// Used to simulate one VFR Flight and create a KML file for it
  /// </summary>
  public class VFRSimulation
  {
    // Traffic 
    private awyDatabase AWYDB = new awyDatabase( );
    private rwyDatabase RWYDB = new rwyDatabase( );
    private List<CmdList> CMDS = null;

    // configs
    private uint m_stepLen_sec = 2;     // update pace for LiveTraffic (use 1..3 for VFR modelling)

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
    public VFRSimulation( string dat_Path, uint stepLen_sec, bool logging )
    {
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
    /// Simulate the Model file given and write a kmlfile of the path
    /// same folder as the model file with the extension .kml
    /// </summary>
    /// <param name="vfrModelFile">The VFR Model File</param>
    /// <returns>True if successfull (else see Error content)</returns>
    public bool RunSimulation( string vfrModelFile )
    {
      Error = "";
      Logger.Instance.Log( $"VFRSimulation-SetupSimulation for: {vfrModelFile}" );
      if ( !Valid ) return false;

      var cmd = CmdReader.ReadCmdScript( vfrModelFile );
      if ( cmd.IsEmpty ) {
        Valid = false;
        Error = "File not found or invalid content";
        Logger.Instance.Log( Error );
        return false;
      }

      var rwy = RWYDB.GetSubtable( cmd.Runway_ID );
      if ( rwy.Count < 1 ) {
        Valid = false;
        Error = $"Runway: {cmd.Runway_ID} not found in Runway database, cannot continue";
        Logger.Instance.Log( Error );
        return false;
      }
      // actually my position
      m_userAcft = new UserAcft( );
      m_userAcft.NewPos( rwy.ElementAt( 0 ).Value.start_latlon );

      // the simulated aircraft
      var virtAcft = new VFRvAcft( Path.GetFileNameWithoutExtension( vfrModelFile ), rwy.ElementAt( 0 ).Value, cmd, cmd.AircraftType, "000001" );
      var kmlFile = new KmlFile( );
      var kmlLine = new line {
        Name = Path.GetFileNameWithoutExtension( vfrModelFile )
      };
      do {
        virtAcft.StepModel( 2 ); // step the model at 2 sec until finished

        kmlLine.Add( new point {
          Position = new coordlib.LatLon( virtAcft.LatLon ),
          Altitude_ft = virtAcft.Alt_ft,
          Heading = (int)rwy.ElementAt( 0 ).Value.brg
        } );

      } while ( !virtAcft.Out );
      // setup Comm
      kmlFile.Lines.Add( kmlLine );
      kmlFile.WriteKML( vfrModelFile + ".kml" );
      return Valid;
    }

  }
}

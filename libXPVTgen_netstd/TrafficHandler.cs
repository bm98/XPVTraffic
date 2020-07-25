using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using libXPVTgen.Aircrafts;
using libXPVTgen.LiveTraffic;
using libXPVTgen.my_awlib;
using libXPVTgen.my_rwylib;
using libXPVTgen.acftSim;

namespace libXPVTgen
{
  /// <summary>
  /// Master obj to handle the traffic interface
  /// </summary>
  public class TrafficHandler
  {

    // Comms
    private string m_host = "127.0.0.1";
    private UDPsender LT_Traffic = null;
    private UDPsender LT_Weather = null;
    private TCPclient LTLink = null;

    // Traffic 
    private awyDatabase AWYDB = new awyDatabase( );
    private rwyDatabase RWYDB = new rwyDatabase( );
    private List<CmdList> CMDS = null;

    private VAcftPool POOL = null;

    // config defaults
    private uint m_stepLen_sec = 2;     // update pace for LiveTraffic (use 1..3 for VFR modelling)
    private double m_radius_nm = 100.0; // Airway selection radius
    private uint m_numAcft = 100;
    private uint m_numVFR = 20;

    // my Aircraft
    private UserAcft m_userAcft = null;
    private DateTime m_lastUpdate = DateTime.Now;


    public event EventHandler<TrafficEventArgs> TrafficEvent;
    private void OnTraffic( long pingSeconds )
    {
      TrafficEvent?.Invoke( this, new TrafficEventArgs( pingSeconds ) );
    }

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
    public TrafficHandler( string dat_Path, uint stepLen_sec, bool logging )
    {
      Logger.Instance.Reset( );
      Logger.Instance.Logging = logging; // user

#if DEBUG
      // always log in DEBUG mode
      Logger.Instance.Logging = true;
#endif
      // Load Database files if possible, else exit with Error
      Logger.Instance.Log( $"TrafficHandler-Create @: {DateTime.Now.ToString( )}" );
      string eawy = Path.Combine( dat_Path, DBCreator.MyAwyDbName );
      if ( !File.Exists( eawy ) ) {
        Error = $"Error: my_awy.dat does not exist: {eawy}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
        return; // ERROR exit
      }

      string erwy = Path.Combine( dat_Path, DBCreator.MyRwyDbName );
      if ( !File.Exists( erwy ) ) {
        Error = $"Error: my_rwy.dat does not exist: {erwy}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
        return; // ERROR exit
      }

      string ret = awyReader.ReadDb( ref AWYDB, eawy );
      if ( !string.IsNullOrEmpty( ret ) ) {
        Error = $"Error: my_awy.dat read failed: {ret}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
        return; // ERROR exit
      }

      ret = rwyReader.ReadDb( ref RWYDB, erwy );
      if ( !string.IsNullOrEmpty( ret ) ) {
        Error = $"Error: my_rwy.dat read failed: {ret}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
        return; // ERROR exit
      }

      string escript = Path.Combine( dat_Path, DBCreator.MyVfrScriptPath );
      if ( !Directory.Exists( escript ) ) {
        Error = $"Warning: script folder does not exist: {escript}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
      }
      else {
        // Load Scripts
        CMDS = CmdReader.ReadScripts( escript );
        if ( CMDS.Count <= 0 ) {
          Error = $"Warning: script folder does not contain valid scripts";
          Logger.Instance.Log( $"TrafficHandler: {Error}" );
        }
      }

      m_stepLen_sec = stepLen_sec;

      Valid = true;
    }

    /// <summary>
    /// Initialize the Link with LiveTraffic
    ///  this will init the receiver and pace outbound messages every 10 sec
    ///  Connects to default ports
    /// </summary>
    /// <param name="hostIP">The LiveTraffic plugin host</param>
    /// <returns>True if successfull (else see Error content)</returns>
    public bool EstablishLink( string hostIP, uint numAcft, uint numVFR )
    {
      Logger.Instance.Log( $"TrafficHandler-EstablishLink to: {hostIP}" );
      if ( !Valid ) return false;

      m_numAcft = numAcft;
      m_numVFR = numVFR;

      m_userAcft = new UserAcft( );
      // setup Comm
      m_host = hostIP;
      try {
        LT_Traffic = new UDPsender( m_host, RealTraffic.PortTrafficUDP );
        LT_Weather = new UDPsender( m_host, RealTraffic.PortWeatherUDP );
        LTLink = new TCPclient( m_host, RealTraffic.PortLinkTCP );
        LTLink.LTEvent += LTLink_LTEvent;
      }
      catch ( Exception e ) {
        Error = $"Error: {e.Message}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
        return false;
      }
      // 
      Valid = LTLink.Connect( ); // if successful this triggers the reception of messages; 
      if ( !Valid )
        Error = $"Error: {LTLink.Error}";

      Logger.Instance.Log( $"TrafficHandler: {Error}" );
      return Valid;
    }

    /// <summary>
    /// Shut Link to LiveTraffic down
    /// </summary>
    public void RemoveLink()
    {
      Logger.Instance.Log( $"TrafficHandler-RemoveLink" );
      Valid = false;
      Error = "";
      try {
        LTLink.LTEvent -= LTLink_LTEvent;
        LT_Traffic = null;
        LT_Weather = null;
        LTLink.Disconnect( );
        m_userAcft = null;
        POOL = null;
      }
      catch ( Exception e ) {
        Error = $"Error: {e.Message}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
      }
    }


    // Asynch processing from receiving actual user position (Triggered by TCPclient)
    private void LTLink_LTEvent( object sender, LTEventArgs e )
    {
      long secSinceLastPing = (long)( DateTime.Now - m_lastUpdate ).TotalSeconds;
      OnTraffic( secSinceLastPing );

      if ( !Valid ) return; // catch out of bounds messages

      m_userAcft.NewPos( e.LatLon );
      if ( POOL == null ) {
        // create the first subset with our AcftPos
        POOL = new VAcftPool( m_radius_nm, m_stepLen_sec ) {
          NumAcft = m_numAcft,
          NumVFRcraft = m_numVFR
        };
        POOL.CreateAwySelection( AWYDB.GetTable( ), RWYDB.GetTable( ), m_userAcft.LatLon );
        POOL.UpdateVFRscripts( CMDS );
        Logger.Instance.Log( $"TrafficHandler: Create POOL with {m_numAcft} aircrafts where {m_numVFR} are VFR, one sim step is >= {m_stepLen_sec} seconds" );
      }
      // push an update only after 'StepLen_sec' secs
      if ( secSinceLastPing >= POOL.StepLen_sec ) {
        m_lastUpdate = DateTime.Now; // reset timer

        LT_Weather.SendMsg( RealTraffic.WeatherString( ) ); // send a 'generic' weather string
        // do POOL maintenance
        POOL.ReGenerate( );
        POOL.Update( );
        // send updates to LiveTraffic - TODO need to pace it ??
        foreach ( var vac in POOL.AircraftPoolRef ) {
          string msg = RealTraffic.AITrafficString( vac );
          LT_Traffic.SendMsg( msg );
        }
        // recreates the airway selection if needed
        POOL.UpdateAwySelection( AWYDB.GetTable( ), RWYDB.GetTable( ), m_userAcft.LatLon );
      }
    }


  }
}

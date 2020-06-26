using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libXPVTgen.LiveTraffic;
using libXPVTgen.my_awlib;
using System.IO;

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
    private VAcftPool POOL = null;
    // configs
    private uint m_stepLen_sec = 10;    // update pace for LiveTraffic
    private double m_radius_nm = 100.0; // Airway selection radius
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
    public TrafficHandler( string dat_Path, uint stepLen_sec )
    {
#if DEBUG
      Logger.Instance.Logging = true;
#endif

      Logger.Instance.Log( $"TrafficHandler-Create @: {DateTime.Now.ToString()}" );
      string eawy = Path.Combine( dat_Path, DBCreator.MyDbName );
      if ( !File.Exists( eawy ) ) {
        Error = $"Error: my_awy.dat does not exist: {eawy}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
        return; // ERROR exit
      }

      string ret = awyReader.ReadDb( ref AWYDB, eawy );
      if ( !string.IsNullOrEmpty( ret ) ) {
        Error = $"Error: my_awy.dat read failed: {ret}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
        return; // ERROR exit
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
    public bool EstablishLink( string hostIP )
    {
      Logger.Instance.Log( $"TrafficHandler-EstablishLink to: {hostIP}" );
      if ( !Valid ) return false;

      m_userAcft = new UserAcft( );
      // setup Comm
      m_host = hostIP;
      try {
        LT_Traffic = new UDPsender( m_host, RealTraffic.PortTrafficUDP );
        LT_Weather = new UDPsender( m_host, RealTraffic.PortWeatherUDP );
        LTLink = new TCPclient( m_host, RealTraffic.PortLinkTCP );
        LTLink.LTEvent += LTLink_LTEvent;
      }
      catch (Exception e){
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
      catch (Exception e)
      {
        Error = $"Error: {e.Message}";
        Logger.Instance.Log( $"TrafficHandler: {Error}" );
      }
    }


    // Asynch processing from receiving actual user position (Triggered by TCPclient)
    private void LTLink_LTEvent( object sender, LTEventArgs e )
    {
      if ( !Valid ) return; // catch out of bounds messages

      m_userAcft.NewPos( e.LatLon );
      if ( POOL == null ) {
        // create the first subset with our AcftPos
        POOL = new VAcftPool( m_stepLen_sec );
        POOL.CreateAwySelection( AWYDB.GetTable( ), m_radius_nm, m_userAcft.LatLon );
      }
      // push an update only after 'StepLen_sec' secs
      if ( ( DateTime.Now - m_lastUpdate ).TotalSeconds >= POOL.StepLen_sec ) {
        m_lastUpdate = DateTime.Now; // reset timer

        LT_Weather.SendMsg( RealTraffic.WeatherString( ) ); // send a 'generic' weather string
        // do POOL maintenance
        POOL.ReGenerate( );
        POOL.Update( );
        // send updates to LiveTraffic - TODO need to pace it ??
        foreach ( var vac in POOL.AircraftPool ) {
          string msg = RealTraffic.AITrafficString( vac );
          LT_Traffic.SendMsg( msg );
        }
        // recreates the airway selection if needed
        POOL.UpdateAwySelection( AWYDB.GetTable( ), m_radius_nm, m_userAcft.LatLon );
      }
    }


  }
}

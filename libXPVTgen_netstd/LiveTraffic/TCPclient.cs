using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using libXPVTgen.coordlib;

namespace libXPVTgen.LiveTraffic
{
  /// <summary>
  /// Implements a client, connecting to a server and then waiting for telegrams
  /// </summary>
  public class TCPclient
  {
    private string m_host = "";
    private IPAddress m_ipAddress = null;
    private int m_port = 0;
    private TcpClient m_tcpClient = null;
    private IPEndPoint m_endpoint = null;
    private NetworkStream m_nStream = null;

    internal PosQueue m_positions = new PosQueue( );

    public string Error { get; private set; } = "";

    /// <summary>
    /// Exposes a command callback derived from COMMAND "Ext3" 
    /// </summary>
    public event EventHandler<LTEventArgs> LTEvent;
    void On_LTEvent( LatLon latlon )
    {
      // just send it to the Owner Form - no local processing in the UC
      LTEvent?.Invoke( this, new LTEventArgs( latlon ) );
    }


    /// <summary>
    /// cTor: Create a TCP client
    /// </summary>
    /// <param name="remoteHost">host IP</param>
    /// <param name="remotePort">Port number</param>
    public TCPclient( string remoteHost, int remotePort )
    {
      m_host = remoteHost;
      m_port = remotePort;

      if ( IPAddress.TryParse( remoteHost, out IPAddress lAddr ) ) {
        m_ipAddress = lAddr;
        try {
          m_endpoint = new IPEndPoint( m_ipAddress, m_port );
          m_tcpClient = new TcpClient( );
        }
        catch ( Exception e ) {
          Error = $"TCP Client setup: {e.Message}";
        }
      }
      else {
        Error = $"TCP Client setup: malformed IP";
      }
    }

    /// <summary>
    /// Try to connect the server and start a receiver
    /// </summary>
    /// <returns>True if connected</returns>
    public bool Connect()
    {
      Error = $"TCP Client connect: no client available";
      // have no infrastructure ??
      if ( m_endpoint == null ) return false;
      if ( m_tcpClient == null ) return false;
      Error = ""; // clear

      try {
        // anybody there ?
        m_tcpClient.Connect( m_endpoint );
        if ( !m_tcpClient.Connected ) {
          Error = "Cannot connect";
          Disconnect( );
          return false;
        }
        m_nStream = m_tcpClient.GetStream( );
        if ( !m_nStream.CanRead ) {
          Error = "Cannot read from stream";
          Disconnect( );
          return false;
        }
        m_nStream.WriteTimeout = 5_000; // we don't actually write..
        m_nStream.ReadTimeout = 5_000; // 5 sec, should receive 1..5/sec
        m_byteBuffer = new byte[m_tcpClient.ReceiveBufferSize];
      }
      catch ( SocketException se ) {
        Error = se.Message;
        Disconnect( );
        return false;
      }
      catch ( Exception e ) {
        Error = e.Message;
        Disconnect( );
        return false;
      }

      return StartReceiveTask( );
    }


    // Task stuff
    private bool m_abort = false;

    private Task m_processor;
    AutoResetEvent m_are = new AutoResetEvent( false );

    private Task m_receiver;
    private string m_RecvBuffer = "";  // there we hold whatever needs to be stored for processing
    private byte[] m_byteBuffer = null;

    /// <summary>
    /// Asynch receiver
    /// </summary>
    private bool StartReceiveTask()
    {
      m_abort = true;
      Error = $"TCP Client connect: no client available or not connected";
      if ( m_endpoint == null ) return false;
      if ( m_tcpClient == null ) return false;
      if ( !m_tcpClient.Connected ) return false;
      if ( m_nStream == null ) return false;
      if ( !m_nStream.CanRead ) return false;

      // clear
      Error = ""; 
      m_abort = false;

      // start processor
      m_positions.Clear( );
      m_are = new AutoResetEvent( false );
      m_processor = Task.Run( () => ProcessPosQueue( ) );
      // start receiver
      m_receiver = Task.Run( () => ProcessNetworkStream( ) );

      return (m_receiver != null) && (m_processor!=null);
    }

    /// <summary>
    /// Check the Receiver state
    /// </summary>
    public bool Receiving { get => !m_abort; }

    /// <summary>
    /// End the receiver 
    /// </summary>
    public void EndProcessTask()
    {
      m_positions.Clear( );
      m_are.Set( ); // allow to abort
      m_processor?.Wait( ); // for the task to finish
    }

    /// <summary>
    /// End the receiver 
    /// </summary>
    public void EndReceiveTask()
    {
      m_receiver?.Wait( ); // for a read timeout
    }

    /// <summary>
    /// Final disconnect (should end first..)
    /// </summary>
    public void Disconnect()
    {
      if ( !m_abort ) {
        m_abort = true; // signal end
        EndProcessTask( );
        EndReceiveTask( );
      }

      m_nStream?.Close( );
      m_tcpClient?.Close( );
    }

    /// <summary>
    /// Processes the Positions received
    /// Asynch - don't delay the receiver task 
    /// </summary>
    private void ProcessPosQueue()
    {
      // task loop
      while ( !m_abort ) {
        m_are.WaitOne( ); // until signaled
        if ( m_positions.Count > 0 ) {
          On_LTEvent( m_positions.Dequeue( ) ); // signal to owner, could be lengthy
        }
        // still work to do ?
        if ( m_positions.Count > 0 ) {
          m_are.Set( ); // signal
        }
      }
    }


    /// <summary>
    /// Task Routine: Processes the clients requests (TCP stream)
    /// This just makes one receive round and returns to the clientdispatcher
    /// </summary>
    private void ProcessNetworkStream()
    {
      var lastTick = DateTime.Now;
      // task loop
      while ( !m_abort ) {
        try {
          // read from client and handle a telegram - it will care about invalid ones
          var latlon = RecvMessage( m_nStream );
          if ( latlon != null ) {
            var newTick = DateTime.Now;
            // LiveTraffic paces pretty hard for no better use
            // so pace it down to 1/sec already here
            if ( ( newTick - lastTick ).TotalSeconds > 1 ) {
              m_positions.Enqueue( latlon );
              m_are.Set( ); // work to do
              lastTick = newTick; 
            }
          }
        }
        catch ( SocketException e ) {
          Error = e.Message;
          Disconnect( );
        }
        catch ( Exception e ) {
          Error = e.Message;
          Disconnect( );
        }
      }
    }


    /// <summary>
    /// Try to get a valid message
    /// </summary>
    /// <remarks>
    /// Received data is stored in the instance buffer and therefore
    /// the first thing is to see if there is already a complete message available
    /// from a former read - if not it will attempt to read from the stream
    /// The dispatcher call us every 100ms or so - this pace is high enough to process
    /// already received messages before reading more data from the client
    /// Read is with timeout - we can then shutdown if needed
    /// </remarks>
    /// <param name="stream">The network stream</param>
    /// <returns>A message, either valid or invalid</returns>
    private LatLon RecvMessage( NetworkStream stream )
    {
      string recvBuffer = m_RecvBuffer; // load from last run

      var latLon = new LatLon( );
      // see if we have a complete message from a previous receive
      if ( RealTraffic.GetFromLinkString( ref recvBuffer, out latLon ) ) {
        m_RecvBuffer = recvBuffer; // save for next run
        return latLon;
      }

      // if we did not found a complete message in the receiving buffer, try to get a new one
      try {
        var ascii = new ASCIIEncoding( );
        int i = stream.Read( m_byteBuffer, 0, m_byteBuffer.Length ); // timeout raises the IOException
        if ( ( i == 0 ) && ( m_tcpClient.Available == 0 ) ) {
          // this is usually true when the client disapeared 
          // - there is no practical way to detect such a case other than trying to send something
          //   but there is no request/reply protocol implemented so far
          throw new SocketException( ); // handle outside
        }

        if ( i != 0 ) {
          recvBuffer += ascii.GetString( m_byteBuffer, 0, i );
          // translate complete messages into commands (if there is one available)
          if ( RealTraffic.GetFromLinkString( ref recvBuffer, out latLon ) ) {
            m_RecvBuffer = recvBuffer; // save for next run
            return latLon;
          }
        }
      }
      catch ( IOException ) {
        // client did not answer in time - this is OK
      }

      return null; // the client did not complete the message - try next time

    }



  }
}

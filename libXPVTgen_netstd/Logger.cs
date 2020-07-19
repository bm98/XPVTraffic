using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libXPVTgen
{
  /// <summary>
  /// A simple debug logger
  /// </summary>
  internal class Logger
  {
    /// <summary>
    /// Simple singleton
    /// </summary>
    public static Logger Instance
    {
      get {
        if ( m_logger == null ) {
          m_logger = new Logger( );
        }
        return m_logger;
      }
    }

    private static Logger m_logger = null;


    // CLASS

    /// <summary>
    /// Turn Logging on/off
    /// </summary>
    public bool Logging { get; set; } = false;

    /// <summary>
    /// Reset the logfile, backup the current one
    /// </summary>
    public void Reset()
    {
      try {
        if ( File.Exists( "libXPVTgen.bak" ) ) File.Delete( "libXPVTgen.bak" );
        File.Move( "libXPVTgen.log", "libXPVTgen.bak" );
      }
      catch { }
    }

    /// <summary>
    /// Log  a line
    /// </summary>
    /// <param name="entry"></param>
    public void Log( string entry )
    {
      if ( !Logging ) return;

      using ( var sw = new StreamWriter( "libXPVTgen.log", true ) ) {
        sw.WriteLine( $"{DateTime.Now.ToLongTimeString( )}: {entry}" );
      }
    }


  }
}

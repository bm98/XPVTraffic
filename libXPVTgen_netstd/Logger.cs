using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libXPVTgen
{
  internal class Logger
  {

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

    public bool Logging { get; set; } = false;

    public void Log( string entry )
    {
      if ( !Logging ) return;

      using ( var sw = new StreamWriter( "libXPVTgen.log", true ) ) {
        sw.WriteLine( entry );
      }
    }


  }
}

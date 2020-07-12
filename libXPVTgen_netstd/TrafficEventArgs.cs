using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen
{
  public class TrafficEventArgs : EventArgs
  {
    /// <summary>
    /// TrafficHandler feedback
    /// </summary>
    public TrafficEventArgs( long pingSeconds )
    {
      PingSeconds = pingSeconds;
    }

    public readonly long PingSeconds = 0;

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libXPVTgen.LiveTraffic
{
  public class LTEventArgs
  {
    /// <summary>
    /// LiveTraffic received Event
    /// </summary>
    public LTEventArgs( coordlib.LatLon latlon )
    {
      LatLon = latlon;
    }

    public coordlib.LatLon LatLon { get; private set; }
  }
}

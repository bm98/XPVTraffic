using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.coordlib;

namespace libXPVTgen
{
  /// <summary>
  /// An Aircraft
  /// </summary>
  public abstract class Acft
  {
    public string ID = "";
    public LatLon LatLon = new LatLon();
    public int Alt_ft;
    
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.coordlib;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// The User Aircraft
  /// </summary>
  public class UserAcft: Acft
  {
    public LatLon InitPos = new LatLon( ); // we could track the distance here
    private bool m_initNeeded = true;

    public void NewPos(LatLon latlon )
    {
      // copy scalars not the class ref...
      if ( m_initNeeded ) {
        InitPos.Lat = latlon.Lat;
        InitPos.Lon = latlon.Lon;
        m_initNeeded = false;
      }

      base.LatLon.Lat = latlon.Lat;
      base.LatLon.Lon = latlon.Lon;
    }


  }
}

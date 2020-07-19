using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;

namespace libXPVTgen.kmllib
{
  /// <summary>
  /// A KML Point Lat/Lon/Alt AGL
  /// </summary>
  class point
  {
    public LatLon Position { get; set; } = new LatLon( );
    public int Altitude_ft { get; set; } = 0;
    public int Altitude_M { get => (int)ConvConsts.FtToM( Altitude_ft ); }
    public int Heading { get; set; } = 0;


    /// <summary>
    /// Returns the point as list of KML lines
    /// </summary>
    /// <returns>A list of KML lines</returns>
    public List<string> AsLookAtList()
    {
      var ret = new List<string> {
        $"  <LookAt>",
        $"    <longitude>{Position.Lon.ToString( )}</longitude>",
        $"    <latitude>{Position.Lat.ToString( )}</latitude>",
        $"    <altitude>{Altitude_M.ToString( )}</altitude>",
        $"    <heading>{Heading}</heading>",
        $"    <tilt>55</tilt>",
        $"    <range>10000</range>",
        $"  </LookAt>"
      };

      return ret;

    }

    /// <summary>
    /// Returns the point as KML coordinate string (Lon,Lat,Alt)
    /// </summary>
    /// <returns>A KML coordinate string</returns>
    public string AsCoordString()
    {
      return $"{Position.Lon.ToString()},{Position.Lat.ToString()},{Altitude_M.ToString()}"; // !! Lon, Lat, Alt (meter)
    }
  }
}

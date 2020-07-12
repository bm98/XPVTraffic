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
    public LatLon Position = new LatLon( );
    public int Altitude_ft = 0;
    public int Altitude_M { get => (int)ConvConsts.FtToM( Altitude_ft ); }
    public int Heading = 0;

    public List<string> AsLookAtList()
    {
      var ret = new List<string>( );
      ret.Add( $"  <LookAt>" );
      ret.Add( $"    <longitude>{Position.Lon.ToString( )}</longitude>" );
      ret.Add( $"    <latitude>{Position.Lat.ToString( )}</latitude>" );
      ret.Add( $"    <altitude>{Altitude_M.ToString( )}</altitude>" );
      ret.Add( $"    <heading>{Heading}</heading>" );
      ret.Add( $"    <tilt>75</tilt>" );
      ret.Add( $"    <range>1000</range>" );
      ret.Add( $"  </LookAt>" );

      return ret;

    }
    public string AsCoordString()
    {
      return $"{Position.Lon.ToString()},{Position.Lat.ToString()},{Altitude_M.ToString()}"; // !! Lon, Lat, Alt (meter)
    }
  }
}

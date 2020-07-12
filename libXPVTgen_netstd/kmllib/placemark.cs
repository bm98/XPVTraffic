using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;

namespace libXPVTgen.kmllib
{
  /// <summary>
  /// A simple KML placemark,
  /// extruded
  /// </summary>
  class placemark
  {
    public point Point = new point( );
    public string Name = "";
    public string Description = "";

    public List<string> AsStringList()
    {
      var ret = new List<string>( );
      ret.Add( $"<Placemark>" );
      ret.Add( $"  <name>{Name}</name>" );
      ret.Add( $"  <visibility>0</visibility>" );
      ret.Add( $"  <description>{Description}</ description > " );
      ret.AddRange( Point.AsLookAtList( ) );
      ret.Add( $"  <styleUrl>#globeIcon</styleUrl>" );
      ret.Add( $"  <Point>" );
      ret.Add( $"    <extrude>1</extrude>" );
      ret.Add( $"    <altitudeMode>relativeToGround</altitudeMode>" );
      ret.Add( $"    <coordinates>" );
      ret.Add( $"      {Point.AsCoordString( )}" );
      ret.Add( $"    </coordinates>" );
      ret.Add( $"  </Point>" );
      ret.Add( $"</Placemark>" );

      return ret;
    }
  }
}

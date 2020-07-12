using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;

namespace libXPVTgen.kmllib
{
  /// <summary>
  /// A line segment
  /// Relative to ground 
  /// </summary>
  class line: List<point>
  {
    public string Name = "unnamed";

    public List<string> AsStringList()
    {
      var ret = new List<string>( );
      ret.Add( $"<Placemark>" );
      ret.Add( $"  <name>{Name}</name>" );
      ret.Add( $"  <visibility>1</visibility>" );
      if ( this.Count > 0 ) {
        ret.AddRange( this[0].AsLookAtList( ) );
        ret.Add( $"  <styleUrl>#yellowLineGreenPoly</styleUrl>" );
        // line string part
        ret.Add( $"  <LineString>" );
        ret.Add( $"    <extrude>1</extrude>" );
        ret.Add( $"    <tessellate>1</tessellate>" );
        ret.Add( $"    <altitudeMode>absolute</altitudeMode>" );
        // coords
        ret.Add( $"    <coordinates>" );
        foreach(var p in this ) {
          ret.Add( $"      {p.AsCoordString( )}" );
        }
        ret.Add( $"    </coordinates>" );
        ret.Add( $"  </LineString>" );
      }
      ret.Add( $"</Placemark>" );

      return ret;
    }
  }
}

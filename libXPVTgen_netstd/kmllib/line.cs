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
  class line : List<point>
  {
    public string Name { get; set; } = "unnamed";
    public LineStyle LineColor { get; set; } = LineStyle.LT_Yellow; // default color

    /// <summary>
    /// Returns the line as KML lines
    /// </summary>
    /// <returns>A list of KML lines</returns>
    public List<string> AsStringList()
    {
      var ret = new List<string> {
        $"<Placemark>",
        $"  <name>{Name}</name>",
        $"  <visibility>1</visibility>"
      };
      if ( this.Count > 0 ) {
        ret.AddRange( this[0].AsLookAtList( ) );
        ret.Add( $"  <styleUrl>{Styles.LTStyleName( LineColor )}</styleUrl>" );
        // line string part
        ret.Add( $"  <LineString>" );
        ret.Add( $"    <extrude>1</extrude>" );
        ret.Add( $"    <tessellate>1</tessellate>" );
        ret.Add( $"    <altitudeMode>absolute</altitudeMode>" );
        // coords
        ret.Add( $"    <coordinates>" );
        foreach ( var p in this ) {
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

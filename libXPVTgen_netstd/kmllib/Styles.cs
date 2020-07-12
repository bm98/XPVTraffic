using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.kmllib
{
  /// <summary>
  /// A number of Styles used in the kmlLib
  /// </summary>
  class Styles
  {

    private static string globeIcon = 
   @"<Style id=""globeIcon""><IconStyle>
     <Icon><href>http://maps.google.com/mapfiles/kml/pal3/icon19.png</href></Icon>
     </IconStyle><LineStyle><width>2</width></LineStyle>
     </Style>";

    private static string yellowLineGreenPoly =
   @"<Style id=""yellowLineGreenPoly""><LineStyle><color>7f00ffff</color><width>4</width></LineStyle>
     <PolyStyle><color>7f00ff00</color></PolyStyle></Style>";

    private static string redLineBluePoly =
   @"<Style id=""redLineBluePoly""><LineStyle><color>ff0000ff</color></LineStyle>
     <PolyStyle><color>ffff0000</color></PolyStyle></Style>";

    private static string blueLineRedPoly =
   @"<Style id=""blueLineRedPoly""><LineStyle><color>ffff0000</color></LineStyle>
     <PolyStyle><color>ff0000ff</color></PolyStyle></Style>";

    public static List<string> AsString()
    {
      var ret = new List<string>( );
      ret.Add($"{globeIcon}");
      ret.Add( $"{yellowLineGreenPoly}");
      ret.Add( $"{redLineBluePoly}");
      ret.Add( $"{blueLineRedPoly}");
      return ret;
    }

  }
}

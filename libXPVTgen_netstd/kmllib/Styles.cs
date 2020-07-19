using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.kmllib
{
  /// <summary>
  /// Supported line styles
  /// </summary>
  public enum LineStyle
  {
    LT_Yellow = 0,
    LT_Red,
    LT_Blue,
  }

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

    public static string LTattr_Yellow = "#yellowLineGreenPoly";
    private static string yellowLineGreenPoly =
   @"<Style id=""yellowLineGreenPoly""><LineStyle><color>7f00ffff</color><width>4</width></LineStyle>
     <PolyStyle><color>7f00ff00</color></PolyStyle></Style>";

    public static string LTattr_Red = "#redLineGrayPoly";
    private static string redLineGrayPoly =
   @"<Style id=""redLineGrayPoly""><LineStyle><color>af0000ff</color></LineStyle>
     <PolyStyle><color>7f333333</color></PolyStyle></Style>";

    public static string LTattr_Blue = "#blueLineGrayPoly";
    private static string blueLineGrayPoly =
   @"<Style id=""blueLineGrayPoly""><LineStyle><color>afff7700</color></LineStyle>
     <PolyStyle><color>7f333333</color></PolyStyle></Style>";

    /// <summary>
    /// Returns a linestyle name
    /// </summary>
    /// <param name="lineStyle">Style</param>
    /// <returns>KML attribute</returns>
    public static string LTStyleName( LineStyle lineStyle )
    {
      switch ( lineStyle ) {
        case LineStyle.LT_Yellow: return LTattr_Yellow;
        case LineStyle.LT_Red: return LTattr_Red;
        case LineStyle.LT_Blue: return LTattr_Blue;
        default: return LTattr_Yellow;
      }
    }

    /// <summary>
    /// Returns the Style settings as list of KML lines
    /// </summary>
    /// <returns>A list of KML lines</returns>
    public static List<string> AsString()
    {
      var ret = new List<string> {
        $"{globeIcon}",
        $"{yellowLineGreenPoly}",
        $"{redLineGrayPoly}",
        $"{blueLineGrayPoly}"
      };
      return ret;
    }

  }
}

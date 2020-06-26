using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libXPVTgen.coordlib
{
  /// <summary>
  /// Geodesy representation conversion functions  
  /// Latitude/longitude points may be represented as double degrees, or subdivided into sexagesimal
  /// minutes and seconds.
  /// 1:1 C# translation from:
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /* Geodesy representation conversion functions                        (c) Chris Veness 2002-2017  */
  /*                                                                                   MIT Licence  */
  /* www.movable-type.co.uk/scripts/latlong.html                                                    */
  /* www.movable-type.co.uk/scripts/geodesy/docs/module-dms.html                                    */
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  // note Unicode Degree = U+00B0. Prime = U+2032, Double prime = U+2033
  public class Dms
  {

    /// <summary>
    /// Separator character to be used to separate degrees, minutes, seconds, and cardinal directions.
    /// 
    /// Set to '\u202f' (narrow no-break space) for improved formatting.
    ///      * @example
    ///      * var p = new LatLon(51.2, 0.33);  // 51°12′00.0″N, 000°19′48.0″E
    ///      *   Dms.separator = '\u202f';        // narrow no-break space
    ///      *   var pʹ = new LatLon( 51.2, 0.33 ); // 51° 12′ 00.0″ N, 000° 19′ 48.0″ E
    /// </summary>
    public static string Separator = "";


    /// <summary>
    /// Parses string representing degrees/minutes/seconds into numeric degrees.
    /// 
    /// This is very flexible on formats, allowing signed double degrees, or deg-min-sec optionally
    /// suffixed by compass direction( NSEW). A variety of separators are accepted( eg 3° 37′ 09″W).
    /// Seconds and minutes may be omitted.
    /// 
    ///      * @example
    ///      * var lat = Dms.parseDMS('51° 28′ 40.12″ N');
    ///      *     var lon = Dms.parseDMS( '000° 00′ 05.31″ W' );
    ///      *     var p1 = new LatLon( lat, lon ); // 51.4778°N, 000.0015°W
    /// </summary>
    /// <param name="dmsStr">{string|number} dmsStr - Degrees or deg/min/sec in variety of formats.</param>
    /// <returns>{number} Degrees as double number.</returns>
    public static double ParseDMS( string dmsStr )
    {
      // check for signed double degrees without NSEW, if so return it directly
      if ( double.TryParse( dmsStr, out double dValue ) ) return dValue;

      // strip off any sign or compass dir'n & split out separate d/m/s
      // var dms = String( dmsStr ).trim( ).replace(/^-/, '' ).replace(/[NSEW]$/i, '' ).split(/[^0-9.,]+/);
      var dms = Regex.Replace( dmsStr.Trim( ), @"^-", "" );
      dms = Regex.Replace( dms, @"[NSEWnsew]$", "" );
      string[] dmsA = Regex.Split( dms, @"[^0-9.,]+" );
      //if ( dms[dms.length - 1] == '' ) dms.splice( dms.length - 1 );  // from trailing symbol
      if ( dmsA[dmsA.Length - 1] == "" ) {
        Array.Resize( ref dmsA, dmsA.Length - 1 ); //dms.splice( dms.length - 1 );  // from trailing symbol
      }
      if ( dmsA.Length == 0 ) return double.NaN; // NaN;

      // and convert to double degrees...
      double deg = 0;
      switch ( dmsA.Length ) {
        case 3:  // interpret 3-part result as d/m/s
          deg = double.Parse( dmsA[0] ) / 1 + double.Parse( dmsA[1] ) / 60 + double.Parse( dmsA[2] ) / 3600;
          break;
        case 2:  // interpret 2-part result as d/m
          deg = double.Parse( dmsA[0] ) / 1 + double.Parse( dmsA[1] ) / 60;
          break;
        case 1:  // just d (possibly double) or non-separated dddmmss
          deg = double.Parse( dmsA[0] );
          // check for fixed-width unseparated format eg 0033709W
          //if (/[NS]/i.test(dmsStr)) deg = '0' + deg;  // - normalise N/S to 3-digit degrees
          //if (/[0-9]{7}/.test(deg)) deg = deg.slice(0,3)/1 + deg.slice(3,5)/60 + deg.slice(5)/3600;
          break;
        default:
          return double.NaN;
      }
      if ( Regex.IsMatch( dmsStr.Trim( ), "^-|[WSws]$" ) )
        deg = -deg; // take '-', west and south as -ve

      return deg;
    }


    /// <summary>
    /// Converts double degrees to deg/min/sec format
    ///  - degree, prime, double-prime symbols are added, but sign is discarded, though no compass
    /// direction is added.
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToDMS( double deg, string format = "dms", int dPlaces = -1 )
    {
      if ( deg == double.NaN ) return ""; // give up here if we can't make a number from deg

      // default values
      if ( dPlaces == -1 ) {
        switch ( format ) {
          case "d": case "deg": dPlaces = 4; break;
          case "dm": case "deg+min": dPlaces = 2; break;
          case "dms": case "deg+min+sec": dPlaces = 0; break;
          default: format = "dms"; dPlaces = 0; break; // be forgiving on invalid format
        }
      }

      deg = Math.Abs( deg );  // (unsigned result ready for appending compass dir'n)

      string dms = "", d = "", m = "", s = "";
      double dN = 0, mN = 0, sN = 0;
      switch ( format ) {
        case "d":
        case "deg":
          // round/right-pad degrees, left-pad with leading zeros (note may include doubles)
          if ( dPlaces > 0 )
            d = Math.Round( deg, dPlaces ).ToString( "000." + "000000".Substring( 0, dPlaces ) );
          else
            d = Math.Round( deg, dPlaces ).ToString( "000" );
          dms = d + '°';
          break;

        case "dm":
        case "deg+min":
          dN = Math.Floor( deg );
          mN = Math.Round( ( ( deg * 60 ) % 60 ), dPlaces );// get component min & round/right-pad
          if ( mN == 60 ) { mN = 0; dN++; }               // check for rounding up
          d = dN.ToString( "000" );                   // left-pad with leading zeros
          if ( dPlaces > 0 )
            m = mN.ToString( "00." + "000000".Substring( 0, dPlaces ) );   // left-pad with leading zeros (note may include doubles)
          else
            m = mN.ToString( "00" );                     // left-pad with leading zeros (note may include doubles)
          dms = d + '°' + Separator + m + '′';
          break;

        case "dms":
        case "deg+min+sec":
          dN = Math.Floor( deg );
          mN = Math.Floor( ( deg * 60 ) % 60 );// get component min & round/right-pad
          sN = Math.Round( ( deg * 3600 % 60 ), dPlaces );  // get component sec & round/right-pad
          if ( sN == 60 ) { sN = 0; mN++; } // check for rounding up
          if ( mN == 60 ) { mN = 0; dN++; }               // check for rounding up
          d = dN.ToString( "000" );                   // left-pad with leading zeros
          m = mN.ToString( "00" );                     // left-pad with leading zeros (note may include doubles)
          if ( dPlaces > 0 )
            s = sN.ToString( "00." + "000000".Substring( 0, dPlaces ) );  // left-pad with leading zeros (note may include doubles)
          else
            s = sN.ToString( "00" );                     // left-pad with leading zeros (note may include doubles)
          dms = d + '°' + Separator + m + '′' + Separator + s + '″';
          break;
        default: break; // invalid format spec!
      }

      return dms;
    }


    /// <summary>
    /// Converts numeric degrees to deg/min/sec latitude (2-digit degrees, suffixed with N/S).
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToLat( double deg, string format = "dms", int dPlaces = -1 )
    {
      var lat = ToDMS( deg, format, dPlaces );
      return ( lat == "" ) ? "–" : ( lat.Substring( 1 ) + Separator + ( deg < 0 ? 'S' : 'N' ) );  // knock off initial '0' for lat!
    }


    /// <summary>
    /// Convert numeric degrees to deg/min/sec longitude (3-digit degrees, suffixed with E/W)
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToLon( double deg, string format = "dms", int dPlaces = -1 )
    {
      var lon = ToDMS( deg, format, dPlaces );
      return ( lon == "" ) ? "–" : lon + Separator + ( deg < 0 ? 'W' : 'E' );  // knock off initial '0' for lat!
    }


    /// <summary>
    /// Converts numeric degrees to deg/min/sec as a bearing (0°..360°)
    /// </summary>
    /// <param name="deg">{number} deg - Degrees to be formatted as specified.</param>
    /// <param name="format">{string} [format=dms] - Return value as 'd', 'dm', 'dms' for deg, deg+min, deg+min+sec.</param>
    /// <param name="dPlaces">{number} [dp=0|2|4] - Number of double places to use – default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Degrees formatted as deg/min/secs according to specified format.</returns>
    public static string ToBrng( double deg, string format = "dms", int dPlaces = -1 )
    {
      deg = ( deg + 360 ) % 360;  // normalise -ve values to 180°..360°
      var brng = ToDMS( deg, format, dPlaces );
      return ( brng == "" ) ? "–" : brng.Replace( "360", "0" );  // just in case rounding took us up to 360°!
    }


    /// <summary>
    /// Returns compass point (to given precision) for supplied bearing.
    ///      * @example
    ///      * var point = Dms.compassPoint(24);    // point = 'NNE'
    ///      * var point = Dms.compassPoint( 24, 1 ); // point = 'N'

    /// </summary>
    /// <param name="bearing">{number} bearing - Bearing in degrees from north.</param>
    /// <param name="precision">{number} [precision=3] - Precision (1:cardinal / 2:intercardinal / 3:secondary-intercardinal).</param>
    /// <returns>{string} Compass point for supplied bearing.</returns>
    public static string CompassPoint( double bearing, int precision = 3 )
    {
      // note precision could be extended to 4 for quarter-winds (eg NbNW), but I think they are little used
      bearing = ( ( bearing % 360 ) + 360 ) % 360; // normalise to range 0..360°

      var cardinals = new string[] {
          "N", "NNE", "NE", "ENE",
          "E", "ESE", "SE", "SSE",
          "S", "SSW", "SW", "WSW",
          "W", "WNW", "NW", "NNW" };
      int n = (int)( 4 * Math.Pow( 2, precision - 1 ) ); // no of compass points at req’d precision (1=>4, 2=>8, 3=>16)
      var cardinal = cardinals[(int)Math.Round( bearing * n / 360 ) % n * 16 / n];

      return cardinal;
    }


  }
}

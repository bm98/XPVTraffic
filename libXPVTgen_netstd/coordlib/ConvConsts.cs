using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.coordlib
{
  /// <summary>
  /// Useful conversions and constants
  /// </summary>
  public class ConvConsts
  {

    public const double KmPerNm = 1.852;
    public const double MPerFt = 0.3048;

    public const double EarthRadiusM = 6371.0E3;
    public const double EarthRadiusKm = EarthRadiusM / 1000;
    public const double EarthRadiusNm = EarthRadiusKm / KmPerNm;

    public static double KmToNm( double km )
    {
      if ( km == double.NaN ) return double.NaN;  // sanity
      return ( km / KmPerNm );
    }

    public static double NmToKm( double nm )
    {
      if ( nm == double.NaN ) return double.NaN;  // sanity
      return ( nm * KmPerNm );
    }


    public static double MToFt( double m )
    {
      if ( m == double.NaN ) return double.NaN;  // sanity
      return ( m / MPerFt );
    }

    public static double FtToM( double ft )
    {
      if ( ft == double.NaN ) return double.NaN;  // sanity
      return ( ft * MPerFt );
    }



  }
}

using System;

namespace libXPVTgen.coordlib
{
  /// <summary>
  /// Implements a Lat, Lon point
  /// 1:1 C# translation from:
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /* Latitude/longitude spherical geodesy tools                         (c) Chris Veness 2002-2017  */
  /*                                                                                   MIT Licence  */
  /* www.movable-type.co.uk/scripts/latlong.html                                                    */
  /* www.movable-type.co.uk/scripts/geodesy/docs/module-latlon-spherical.html                       */
  /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
  /// </summary>
  public class LatLon
  {
    private double m_lat = 0;
    private double m_lon = 0;

    public double Lat { get => m_lat; set => m_lat = value; }
    public double Lon { get => m_lon; set => m_lon = value; }

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude.
    ///  initialized to 0/0
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    public LatLon()
    {
    }

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude.
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    public LatLon( double lat, double lon )
    {
      m_lat = lat;
      m_lon = lon;
    }

    /// <summary>
    /// Creates a LatLon point on the earth's surface at the specified latitude / longitude.
    ///   as copy of the given LatLon
    /// </summary>
    /// <param name="other">LatLon to copy from</param>
    public LatLon( LatLon other )
    {
      m_lat = other.Lat;
      m_lon = other.Lon;
    }


    /// <summary>
    /// Returns the distance from ‘this’ point to destination point (using haversine formula).
    ///      * @example
    ///      *     var p1 = new LatLon( 52.205, 0.119 );
    ///      *     var p2 = new LatLon( 48.857, 2.351 );
    ///      *     var d = p1.distanceTo( p2 ); // 404.3 km
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance between this point and destination point, in same units as radius.</returns>
    public double DistanceTo( LatLon point, double radius = 6371.0E3 )
    {
      // a = sin²(Δφ/2) + cos(φ1)⋅cos(φ2)⋅sin²(Δλ/2)
      // tanδ = √(a) / √(1−a)
      // see mathforum.org/library/drmath/view/51879.html for derivation
      var R = radius;
      double φ1 = m_lat.ToRadians( );
      double λ1 = m_lon.ToRadians( );
      double φ2 = point.Lat.ToRadians( );
      double λ2 = point.Lon.ToRadians( );
      double Δφ = φ2 - φ1;
      double Δλ = λ2 - λ1;

      var a = Math.Sin( Δφ / 2 ) * Math.Sin( Δφ / 2 )
            + Math.Cos( φ1 ) * Math.Cos( φ2 )
            * Math.Sin( Δλ / 2 ) * Math.Sin( Δλ / 2 );
      var c = 2 * Math.Atan2( Math.Sqrt( a ), Math.Sqrt( 1 - a ) );
      var d = R * c;

      return d;
    }


    /// <summary>
    /// Returns the (initial) bearing from ‘this’ point to destination point.
    /// 
    ///      * @example
    ///      *     var p1 = new LatLon( 52.205, 0.119 );
    ///      *     var p2 = new LatLon( 48.857, 2.351 );
    ///      *     var b1 = p1.bearingTo( p2 ); // 156.2°
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <returns>{number} Initial bearing in degrees from north.</returns>
    public double BearingTo( LatLon point )
    {
      // tanθ = sinΔλ⋅cosφ2 / cosφ1⋅sinφ2 − sinφ1⋅cosφ2⋅cosΔλ
      // see mathforum.org/library/drmath/view/55417.html for derivation
      double φ1 = m_lat.ToRadians( );
      double φ2 = point.Lat.ToRadians( );
      double Δλ = ( point.Lon - m_lon ).ToRadians( );
      var y = Math.Sin( Δλ ) * Math.Cos( φ2 );
      var x = Math.Cos( φ1 ) * Math.Sin( φ2 ) -
              Math.Sin( φ1 ) * Math.Cos( φ2 ) * Math.Cos( Δλ );
      var θ = Math.Atan2( y, x );

      return ( θ.ToDegrees( ) + 360.0 ) % 360;
    }

    /// <summary>
    /// Returns final bearing arriving at destination destination point from ‘this’ point; the final bearing
    /// will differ from the initial bearing by varying degrees according to distance and latitude.
    /// 
    ///      * @example
    ///      *     var p1 = new LatLon( 52.205, 0.119 );
    ///      *     var p2 = new LatLon( 48.857, 2.351 );
    ///      *     var b2 = p1.finalBearingTo( p2 ); // 157.9°
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <returns>{number} Final bearing in degrees from north.</returns>
    public double FinalBearingTo( LatLon point )
    {
      // get initial bearing from destination point to this point & reverse it by adding 180°
      return ( point.BearingTo( this ) + 180 ) % 360;
    }


    /// <summary>
    /// Returns the midpoint between ‘this’ point and the supplied point.
    /// 
    ///  * @example
    ///  *     var p1 = new LatLon( 52.205, 0.119 );
    ///  *     var p2 = new LatLon( 48.857, 2.351 );
    ///  *     var pMid = p1.midpointTo( p2 ); // 50.5363°N, 001.2746°E
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <returns>{LatLon} Midpoint between this point and the supplied point.</returns>
    public LatLon MidpointTo( LatLon point )
    {
      // φm = atan2( sinφ1 + sinφ2, √( (cosφ1 + cosφ2⋅cosΔλ) ⋅ (cosφ1 + cosφ2⋅cosΔλ) ) + cos²φ2⋅sin²Δλ )
      // λm = λ1 + atan2(cosφ2⋅sinΔλ, cosφ1 + cosφ2⋅cosΔλ)
      // see mathforum.org/library/drmath/view/51822.html for derivation
      double φ1 = m_lat.ToRadians( );
      double λ1 = m_lon.ToRadians( );
      double φ2 = point.Lat.ToRadians( );
      double Δλ = (double)( point.Lon - m_lon ).ToRadians( );

      var Bx = Math.Cos( φ2 ) * Math.Cos( Δλ );
      var By = Math.Cos( φ2 ) * Math.Sin( Δλ );

      var x = Math.Sqrt( ( Math.Cos( φ1 ) + Bx ) * ( Math.Cos( φ1 ) + Bx ) + By * By );
      var y = Math.Sin( φ1 ) + Math.Sin( φ2 );
      var φ3 = Math.Atan2( y, x );

      var λ3 = λ1 + Math.Atan2( By, Math.Cos( φ1 ) + Bx );

      return new LatLon( φ3.ToDegrees( ), ( λ3.ToDegrees( ) + 540 ) % 360 - 180 ); // normalise to −180..+180°
    }


    /// <summary>
    /// Returns the point at given fraction between ‘this’ point and specified point.
    /// 
    ///      * @example
    ///      *   let p1 = new LatLon( 52.205, 0.119 );
    ///      *   let p2 = new LatLon( 48.857, 2.351 );
    ///      *   let pMid = p1.intermediatePointTo( p2, 0.25 ); // 51.3721°N, 000.7073°E
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point.</param>
    /// <param name="fraction">{number} fraction - Fraction between the two points (0 = this point, 1 = specified point).</param>
    /// <returns>{LatLon} Intermediate point between this point and destination point.</returns>
    public LatLon IntermediatePointTo( LatLon point, double fraction )
    {
      double φ1 = m_lat.ToRadians( );
      double λ1 = m_lon.ToRadians( );
      double φ2 = point.Lat.ToRadians( );
      double λ2 = point.Lon.ToRadians( );

      double sinφ1 = Math.Sin( φ1 ), cosφ1 = Math.Cos( φ1 ), sinλ1 = Math.Sin( λ1 ), cosλ1 = Math.Cos( λ1 );
      double sinφ2 = Math.Sin( φ2 ), cosφ2 = Math.Cos( φ2 ), sinλ2 = Math.Sin( λ2 ), cosλ2 = Math.Cos( λ2 );

      // distance between points
      var Δφ = φ2 - φ1;
      var Δλ = λ2 - λ1;
      var a = Math.Sin( Δφ / 2 ) * Math.Sin( Δφ / 2 )
          + Math.Cos( φ1 ) * Math.Cos( φ2 ) * Math.Sin( Δλ / 2 ) * Math.Sin( Δλ / 2 );
      var δ = 2 * Math.Atan2( Math.Sqrt( a ), Math.Sqrt( 1 - a ) );

      var A = Math.Sin( ( 1 - fraction ) * δ ) / Math.Sin( δ );
      var B = Math.Sin( fraction * δ ) / Math.Sin( δ );

      var x = A * cosφ1 * cosλ1 + B * cosφ2 * cosλ2;
      var y = A * cosφ1 * sinλ1 + B * cosφ2 * sinλ2;
      var z = A * sinφ1 + B * sinφ2;

      var φ3 = Math.Atan2( z, Math.Sqrt( x * x + y * y ) );
      var λ3 = Math.Atan2( y, x );

      return new LatLon( φ3.ToDegrees( ), ( λ3.ToDegrees( ) + 540 ) % 360 - 180 ); // normalise lon to −180..+180°
    }


    /// <summary>
    /// Returns the destination point from ‘this’ point having travelled the given distance on the
    /// given initial bearing( bearing normally varies around path followed ).
    /// 
    ///      * @example
    ///      *     var p1 = new LatLon( 51.4778, -0.0015 );
    ///      *     var p2 = p1.destinationPoint( 7794, 300.7 ); // 51.5135°N, 000.0983°W
    /// </summary>
    /// <param name="distance">{number} distance - Distance travelled, in same units as earth radius (default: metres).</param>
    /// <param name="bearing">{number} bearing - Initial bearing in degrees from north.</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{LatLon} Destination point.</returns>
    public LatLon DestinationPoint( double distance, double bearing, double radius = 6371.0E3 )
    {
      // sinφ2 = sinφ1⋅cosδ + cosφ1⋅sinδ⋅cosθ
      // tanΔλ = sinθ⋅sinδ⋅cosφ1 / cosδ−sinφ1⋅sinφ2
      // see mathforum.org/library/drmath/view/52049.html for derivation
      double δ = distance / radius; // angular distance in radians
      double θ = bearing.ToRadians( );

      double φ1 = m_lat.ToRadians( );
      double λ1 = m_lon.ToRadians( );

      double sinφ1 = Math.Sin( φ1 ), cosφ1 = Math.Cos( φ1 );
      double sinδ = Math.Sin( δ ), cosδ = Math.Cos( δ );
      double sinθ = Math.Sin( θ ), cosθ = Math.Cos( θ );

      var sinφ2 = sinφ1 * cosδ + cosφ1 * sinδ * cosθ;
      var φ2 = Math.Asin( sinφ2 );
      var y = sinθ * sinδ * cosφ1;
      var x = cosδ - sinφ1 * sinφ2;
      var λ2 = λ1 + Math.Atan2( y, x );

      return new LatLon( φ2.ToDegrees( ), ( λ2.ToDegrees( ) + 540 ) % 360 - 180 ); // normalise to −180..+180°
    }


    /// <summary>
    /// Returns the point of intersection of two paths defined by point and bearing.
    /// 
    ///      * @example
    ///      *     var p1 = LatLon( 51.8853, 0.2545 ), brng1 = 108.547;
    ///      *     var p2 = LatLon( 49.0034, 2.5735 ), brng2 = 32.435;
    ///      *     var pInt = LatLon.intersection( p1, brng1, p2, brng2 ); // 50.9078°N, 004.5084°E
    /// </summary>
    /// <param name="p1">{LatLon} p1 - First point.</param>
    /// <param name="brng1">{number} brng1 - Initial bearing from first point.</param>
    /// <param name="p2">{LatLon} p2 - Second point.</param>
    /// <param name="brng2">{number} brng2 - Initial bearing from second point.</param>
    /// <returns>{LatLon|null} Destination point (null if no unique intersection defined).</returns>
    public LatLon Intersection( LatLon p1, double brng1, LatLon p2, double brng2 )
    {
      // see www.edwilliams.org/avform.htm#Intersection
      double φ1 = p1.Lat.ToRadians( );
      double λ1 = p1.Lon.ToRadians( );
      double φ2 = p2.Lat.ToRadians( );
      double λ2 = p2.Lon.ToRadians( );
      double θ13 = brng1.ToRadians( ), θ23 = brng2.ToRadians( );
      double Δφ = φ2 - φ1;
      double Δλ = λ2 - λ1;

      // angular distance p1-p2
      var δ12 = 2 * Math.Asin( Math.Sqrt( Math.Sin( Δφ / 2 ) * Math.Sin( Δφ / 2 )
          + Math.Cos( φ1 ) * Math.Cos( φ2 ) * Math.Sin( Δλ / 2 ) * Math.Sin( Δλ / 2 ) ) );
      if ( δ12 == 0 ) return null;

      // initial/final bearings between points
      var θa = Math.Acos( ( Math.Sin( φ2 ) - Math.Sin( φ1 ) * Math.Cos( δ12 ) ) / ( Math.Sin( δ12 ) * Math.Cos( φ1 ) ) );
      if ( double.IsNaN( θa ) ) θa = 0; // protect against rounding
      var θb = Math.Acos( ( Math.Sin( φ1 ) - Math.Sin( φ2 ) * Math.Cos( δ12 ) ) / ( Math.Sin( δ12 ) * Math.Cos( φ2 ) ) );

      var θ12 = Math.Sin( λ2 - λ1 ) > 0 ? θa : 2 * Math.PI - θa;
      var θ21 = Math.Sin( λ2 - λ1 ) > 0 ? 2 * Math.PI - θb : θb;

      var α1 = θ13 - θ12; // angle 2-1-3
      var α2 = θ21 - θ23; // angle 1-2-3

      if ( Math.Sin( α1 ) == 0 && Math.Sin( α2 ) == 0 ) return null; // infinite intersections
      if ( Math.Sin( α1 ) * Math.Sin( α2 ) < 0 ) return null;      // ambiguous intersection

      var α3 = Math.Acos( -Math.Cos( α1 ) * Math.Cos( α2 ) + Math.Sin( α1 ) * Math.Sin( α2 ) * Math.Cos( δ12 ) );
      var δ13 = Math.Atan2( Math.Sin( δ12 ) * Math.Sin( α1 ) * Math.Sin( α2 ), Math.Cos( α2 ) + Math.Cos( α1 ) * Math.Cos( α3 ) );
      var φ3 = Math.Asin( Math.Sin( φ1 ) * Math.Cos( δ13 ) + Math.Cos( φ1 ) * Math.Sin( δ13 ) * Math.Cos( θ13 ) );
      var Δλ13 = Math.Atan2( Math.Sin( θ13 ) * Math.Sin( δ13 ) * Math.Cos( φ1 ), Math.Cos( δ13 ) - Math.Sin( φ1 ) * Math.Sin( φ3 ) );
      var λ3 = λ1 + Δλ13;

      return new LatLon( φ3.ToDegrees( ), ( λ3.ToDegrees( ) + 540 ) % 360 - 180 ); // normalise to −180..+180°
    }


    /// <summary>
    /// Returns (signed) distance from ‘this’ point to great circle defined by start-point and end-point.
    /// 
    ///      * @example
    ///      *   var pCurrent = new LatLon( 53.2611, -0.7972 );
    ///      *   var p1 = new LatLon( 53.3206, -1.7297 );
    ///      *   var p2 = new LatLon( 53.1887, 0.1334 );
    ///      *   var d = pCurrent.crossTrackDistanceTo( p1, p2 );  // -307.5 m
    /// </summary>
    /// <param name="pathStart">{LatLon} pathStart - Start point of great circle path.</param>
    /// <param name="pathEnd">{LatLon} pathEnd - End point of great circle path.</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance to great circle (-ve if to left, +ve if to right of path).</returns>
    public double DoubleCrossTrackDistanceTo( LatLon pathStart, LatLon pathEnd, double radius = 6371.0E3 )
    {
      var δ13 = pathStart.DistanceTo( this, radius ) / radius;
      var θ13 = pathStart.BearingTo( this ).ToRadians( );
      var θ12 = pathStart.BearingTo( pathEnd ).ToRadians( );

      var δxt = Math.Asin( Math.Sin( δ13 ) * Math.Sin( θ13 - θ12 ) );

      return δxt * radius;
    }


    /// <summary>
    /// Returns how far ‘this’ point is along a path from from start-point, heading towards end-point.
    /// That is, if a perpendicular is drawn from ‘this’ point to the( great circle ) path, the along-track
    /// distance is the distance from the start point to where the perpendicular crosses the path.
    /// 
    ///      * @example
    ///      *   var pCurrent = new LatLon( 53.2611, -0.7972 );
    ///      *   var p1 = new LatLon( 53.3206, -1.7297 );
    ///      *   var p2 = new LatLon( 53.1887, 0.1334 );
    ///      *   var d = pCurrent.alongTrackDistanceTo( p1, p2 );  // 62.331 km
    /// </summary>
    /// <param name="pathStart">{LatLon} pathStart - Start point of great circle path.</param>
    /// <param name="pathEnd">{LatLon} pathEnd - End point of great circle path.</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance along great circle to point nearest ‘this’ point.</returns>
    public double AlongTrackDistanceTo( LatLon pathStart, LatLon pathEnd, double radius = 6371.0E3 )
    {
      var δ13 = pathStart.DistanceTo( this, radius ) / radius;
      var θ13 = pathStart.BearingTo( this ).ToRadians( );
      var θ12 = pathStart.BearingTo( pathEnd ).ToRadians( );

      var δxt = Math.Asin( Math.Sin( δ13 ) * Math.Sin( θ13 - θ12 ) );

      var δat = Math.Acos( Math.Cos( δ13 ) / Math.Abs( Math.Cos( δxt ) ) );

      return δat * Math.Sign( Math.Cos( θ12 - θ13 ) ) * radius;
    }


    /// <summary>
    /// Returns maximum latitude reached when travelling on a great circle on given bearing from this
    /// point('Clairaut's formula'). Negate the result for the minimum latitude (in the Southern
    /// hemisphere).
    /// 
    /// The maximum latitude is independent of longitude; it will be the same for all points on a given
    /// latitude.
    /// </summary>
    /// <param name="bearing">{number} bearing - Initial bearing.</param>
    /// <returns>maximum latitude reached</returns>
    public double MaxLatitude( double bearing )
    {
      var θ = bearing.ToRadians( );
      var φ = m_lat.ToRadians( );
      var φMax = Math.Acos( Math.Abs( Math.Sin( θ ) * Math.Cos( φ ) ) );

      return φMax.ToDegrees( );
    }


    /// <summary>
    /// Returns the pair of meridians at which a great circle defined by two points crosses the given
    /// latitude.If the great circle doesn't reach the given latitude, null is returned.
    /// </summary>
    /// <param name="point1">{LatLon} point1 - First point defining great circle.</param>
    /// <param name="point2">{LatLon} point2 - Second point defining great circle.</param>
    /// <param name="latitude">{number} latitude - Latitude crossings are to be determined for.</param>
    /// <returns>{Object|null} Object containing { lon1, lon2 } or null if given latitude not reached.</returns>
    double[] CrossingParallels( LatLon point1, LatLon point2, double latitude )
    {
      var φ = latitude.ToRadians( );

      double φ1 = point1.Lat.ToRadians( );
      double λ1 = point1.Lon.ToRadians( );
      double φ2 = point2.Lat.ToRadians( );
      double λ2 = point2.Lon.ToRadians( );

      var Δλ = λ2 - λ1;

      var x = Math.Sin( φ1 ) * Math.Cos( φ2 ) * Math.Cos( φ ) * Math.Sin( Δλ );
      var y = Math.Sin( φ1 ) * Math.Cos( φ2 ) * Math.Cos( φ ) * Math.Cos( Δλ ) - Math.Cos( φ1 ) * Math.Sin( φ2 ) * Math.Cos( φ );
      var z = Math.Cos( φ1 ) * Math.Cos( φ2 ) * Math.Sin( φ ) * Math.Sin( Δλ );

      if ( z * z > x * x + y * y ) return null; // great circle doesn't reach latitude

      var λm = Math.Atan2( -y, x );                  // longitude at max latitude
      var Δλi = Math.Acos( z / Math.Sqrt( x * x + y * y ) ); // Δλ from λm to intersection points

      var λi1 = λ1 + λm - Δλi;
      var λi2 = λ1 + λm + Δλi;

      return new double[] { ( λi1.ToDegrees( ) + 540 ) % 360 - 180, ( λi2.ToDegrees( ) + 540 ) % 360 - 180 }; // normalise to −180..+180°
    }


    /// <summary>
    /// Checks if another point is equal to ‘this’ point.
    /// 
    ///      * @example
    ///      *   var p1 = new LatLon( 52.205, 0.119 );
    ///      *   var p2 = new LatLon( 52.205, 0.119 );
    ///      *   var equal = p1.equals( p2 ); // true
    /// </summary>
    /// <param name="point">{LatLon} point - Point to be compared against this point.</param>
    /// <returns>{bool}   True if points are identical.</returns>
    public bool Equals( LatLon point )
    {
      if ( m_lat != point.Lat ) return false;
      if ( m_lon != point.Lon ) return false;

      return true;
    }

    /// <summary>
    /// Returns a string representation of ‘this’ point, formatted as degrees, degrees+minutes, or
    /// degrees+minutes+seconds.
    /// </summary>
    /// <param name="format">{string} [format=dms] - Format point as 'd', 'dm', 'dms'.</param>
    /// <param name="dp">{number} [dp=0|2|4] - Number of decimal places to use - default 0 for dms, 2 for dm, 4 for d.</param>
    /// <returns>{string} Comma-separated latitude/longitude.</returns>
    public string ToString( string format = "dms", int dp = 0 )
    {
      return Dms.ToLat( m_lat, format, dp ) + ", " + Dms.ToLon( m_lon, format, dp );
    }

  }


  #region Extension

  /// <summary>
  /// Extension ToRadians / ToDegrees for double type
  /// </summary>
  internal static class Foo
  {
    public static double ToRadians( this double angleInDegree )
    {
      return ( angleInDegree * Math.PI ) / 180.0;
    }

    public static double ToDegrees( this double angleInRadians )
    {
      return angleInRadians * ( 180.0 / Math.PI );
    }

    /// <summary>
    /// Returns the sum of angles reduced to a 0..360° circle
    /// </summary>
    public static double AddDegrees( this double angleInDegree, double otherAngle )
    {
      double s = angleInDegree + otherAngle;
      while ( s > 360.0 ) { s -= 360.0; }
      while ( s < 0.0 ) { s += 360.0; }

      return s;
    }

    /// <summary>
    /// Returns the differenc of angles reduced to a 0..360° circle
    /// </summary>
    public static double SubDegrees( this double angleInDegree, double otherAngle )
    {
      double s = angleInDegree - otherAngle;
      while ( s > 360.0 ) { s -= 360.0; }
      while ( s < 0.0 ) { s += 360.0; }

      return s;
    }


  }
  #endregion

}



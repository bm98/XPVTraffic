using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libXPVTgen.acftSim;
using libXPVTgen.coordlib;
using libXPVTgen.my_awlib;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// Creates a random IFR Route
  /// </summary>
  internal class IFRroute
  {
    private static Random m_random = new Random( (int)DateTime.Now.Ticks );

    // maintain IFR flights within certain levels
    private const int c_maxLevel = 45_000; // ft AMSL
    private const int c_minLevel = 3_000;  // ft AMSL

    private static int MaxLevel( int alt ) { return ( alt > c_maxLevel ) ? c_maxLevel : alt; }
    private static int MinLevel( int alt ) { return ( alt < c_minLevel ) ? c_minLevel : alt; }

    // Get one airway as random selection or null 
    private static awyRec GetRandomAwy( awyTable awyTableRef )
    {
      if ( awyTableRef.Count <= 0 ) return null; // no Awy in table 

      int item = m_random.Next( awyTableRef.Count );
      return awyTableRef.ElementAt( item ).Value;
    }

    // Get a random speed
    private static double GetSpeed( int layer )
    {
      double gs = 180;
      if ( layer == 1 ) { // low
        gs = m_random.Next( 160, 210 ); // some variations is gs for low enroutes
      }
      else if ( layer == 2 ) { // high
        gs = m_random.Next( 250, 420 ); // some variations is gs for high enroutes
      }
      return gs;
    }

    // Get a new altitude from the given altitude within min and max airway level
    private static int GetNewAlt( int alt, int lvlMin, int lvlMax )
    {
      // Sanity checks for level limits
      lvlMax = MaxLevel( lvlMax );
      lvlMin = MinLevel( lvlMin );
      if ( alt < lvlMin ) return lvlMin + 100; // we are too low
      if ( alt > lvlMax ) return lvlMax - 100; // we are too high

      // now we are within the levels
      int lc = m_random.Next( -5, 5 ); // select between +- 500ft
      int newAlt = alt + lc * 100;
      if ( ( newAlt < lvlMin ) || ( newAlt > lvlMax ) ) {
        newAlt = alt - lc * 100; // use the other direction if we are out of limits
      }
      return newAlt;
    }

    /// <summary>
    /// Create a random IFR flight
    /// </summary>
    /// <param name="awyTableRef">The airway table to chose from</param>
    /// <param name="acftNo">The aircraft regNo</param>
    /// <param name="acftType">The aircraft type</param>
    /// <param name="airline">The airline operator ICAO code</param>
    /// <returns></returns>
    public static CmdList GetRandomFlight( awyTable awyTableRef, int acftNo, string acftType, string airline )
    {
      var route = new CmdList( );
      var visited = new List<string>( );  // visited Navs and Fixes, we don't want to run circles

      var awy = GetRandomAwy( awyTableRef );
      if ( awy == null ) return route; // no airways available ??

      // add Aircraft Descriptor first
      route.Enqueue( new CmdA( acftType, CmdA.FlightT.Airway, airline ) );
      // some random altitude and complete start of the route
      var altMsl = m_random.Next( MinLevel( awy.baseFt ), MaxLevel( awy.topFt ) );
      altMsl = (int)Math.Round( altMsl / 100.0 ) * 100; // get 100 ft increments
      route.Descriptor.InitFromAirway( acftNo, awy, altMsl, GetSpeed( awy.layer ) );   // set start conditions (assumes MslBase=0)
      // add segment length command
      route.Enqueue( new CmdD( awy.Distance_nm ) );
      visited.Add( awy.startID ); // we add all startIDs

      // do we have an airway to go from here?
      var newleg = awyTableRef.GetNextSegment( awy );
      while ( newleg != null ) {
        if ( visited.Contains( newleg.startID ) ) {
          break; // this would create a circle (endless loop)
        }

        awy = newleg;
        // random speed change
        if ( m_random.Next( 10 ) == 0 ) { // one out of 10 
          // add S command
          route.Enqueue( new CmdS( GetSpeed( awy.layer ) ) );
        }
        // random alt change
        if ( m_random.Next( 20 ) == 0 ) { // one out of 20 
          // add V command
          altMsl = GetNewAlt( altMsl, awy.baseFt, awy.topFt );
          route.Enqueue( new CmdV( 1200, altMsl ) );
        }
        // add Goto command
        route.Enqueue( new CmdG( awy.end_latlon ) );
        visited.Add( awy.startID ); // we add all startIDs to avoid loops above

        // try next one
        newleg = awyTableRef.GetNextSegment( awy );
      }
      route.Descriptor.FinishFromAirway( awy ); // set end location ID from last segment
      // add mandatory end segment
      route.Enqueue( new CmdE( ) );

      return route;
    }


  }
}

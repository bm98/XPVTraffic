using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using libXPVTgen.coordlib;

namespace libXPVTgen.my_awlib
{
  /// <summary>
  ///  key: start_reg_end_reg
  /// </summary>
  public class awyTable : Dictionary<string, awyRec>
  {
    private static Random m_random = new Random( );


    public awyTable()
    {
    }

    /// <summary>
    /// Create an ICAO table from the given table
    /// </summary>
    /// <param name="prefix">The prefix of the table</param>
    /// <param name="table">The source to fill from</param>
    public awyTable( awyTable table )
    {
      this.AddSubtable( table );
    }


    /// <summary>
    /// Add one record to the table
    /// </summary>
    /// <param name="rec"></param>
    public string Add( awyRec rec )
    {
      string ret = "";
      if ( rec != null ) {
        if ( rec.ident.Length >= 5 ) {
          if ( !this.ContainsKey( rec.ident ) ) {
            this.Add( rec.ident, rec );
          }
          else {
            // overwite ?? NO 
          }
        }
      }
      return ret;
    }


    /// <summary>
    /// Adds a table to this table (omitting key dupes)
    /// </summary>
    /// <param name="subtable">A table to add to this table</param>
    public string AddSubtable( awyTable subtable )
    {
      string ret = "";
      foreach ( var rec in subtable ) {
        try {
          ret += this.Add( rec.Value );
        }
        catch { }
      }
      return ret;
    }


    /// <summary>
    /// Adds a table to this table (omitting key dupes)
    /// </summary>
    /// <param name="selection">Enumerated Key Value pairs to add to this table</param>
    private string AddSubtable( IEnumerable<KeyValuePair<string, awyRec>> selection )
    {
      string ret = "";
      foreach ( var rec in selection ) {
        try {
          ret += this.Add( rec.Value );
        }
        catch { }
      }
      return ret;
    }

    /// <summary>
    /// Return an Airway subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>An awyTable</returns>
    public awyTable GetSubtable( string icao_key )
    {
      var nT = new awyTable( );
      foreach ( var rec in this ) {
        // key = ident => "icao_region_icao_region"  (so find is Contains(icao), which is expensive...)
        if ( rec.Key.Contains( icao_key ) ) {
          nT.Add( rec.Value );
        }
      }
      return nT;
    }

    /// <summary>
    /// Return an Airway subtable where start ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>An awyTable</returns>
    public awyTable GetSubtableStartWith( string icao_key )
    {
      var nT = new awyTable( );
      foreach ( var rec in this ) {
        // key = ident => "icao_region_icao_region"  (so find is Contains(icao), which is expensive...)
        if ( rec.Key.StartsWith( icao_key ) ) {
          nT.Add( rec.Value );
        }
      }
      return nT;
    }

    /// <summary>
    /// Return an Airway subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>An awyTable</returns>
    public awyTable GetSubtable( double rangeLimitNm, double Lat, double Lon )
    {
      var nT = new awyTable( );

      var myLoc = new LatLon( Lat, Lon );
      foreach ( var rec in this ) {
        var dist = myLoc.DistanceTo( rec.Value.mid_latlon, ConvConsts.EarthRadiusNm );
        if ( dist <= rangeLimitNm ) {
          nT.Add( rec.Value );
        }
      }
      return nT;
    }


    /// <summary>
    /// Return a sorted Airway subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>A sorted awyTable</returns>
    public awyTable GetSortedSubtable( string icao_key )
    {
      var l = GetSubtable( icao_key ).OrderBy( key => key.Key );
      return ( l.ToDictionary( ( keyItem ) => keyItem.Key, ( valueItem ) => valueItem.Value ) as awyTable );
    }

    /// <summary>
    /// Returns true if tgt contains one of the src names
    /// </summary>
    /// <param name="src">Segment name list</param>
    /// <param name="tgt">Segment name list</param>
    /// <returns>True for a match</returns>
    private bool SegNameMatch( string src, string tgt )
    {
      if ( src.Contains( "-" ) ) {
        string[] srcE = src.Split( new char[] { '-' } );
        for ( int i = 0; i < srcE.Length; i++ ) {
          if ( tgt.Contains( srcE[i] ) ) return true;
        }
      }
      else {
        // source is a single name
        return tgt.Contains( src );
      }
      return false;
    }

    /// <summary>
    /// Get a continuation segment for the given segment
    /// </summary>
    /// <param name="seg">The current segment</param>
    /// <returns>A continuation segment or null</returns>
    public awyRec GetNextSegment( awyRec seg )
    {
      // do we have an airway to go from here?
      var newlegs = GetSubtableStartWith( seg.endID ); // get the ones that start here
      var leglist = new List<awyRec>( );
      foreach ( var leg in newlegs ) {
        // don't just go back..
        if ( leg.Value.endID != seg.startID ) {
          if ( SegNameMatch( seg.segments, leg.Value.segments ) ) {
            // this is the continuation segment
            Logger.Instance.Log( $"awyTable: Routing: ContSeg: {leg.Value.ident}" );
            return leg.Value;
          }
          // not a continuation, chance to change the airway then but only at some angle
          float diversion = Math.Abs(seg.brg - leg.Value.brg);
          if ( diversion < 150.0f ) {
            // this is a decent continuation (max brg diff is 150°)
            leglist.Add( leg.Value );
          }
        }
      }
      // did not found the continuation; from a new list take one
      if ( leglist.Count > 0 ) {
        int index = m_random.Next( leglist.Count );
        Logger.Instance.Log( $"awyTable: Routing: RndSeg: {leglist.ElementAt( index ).ident}" );
        return leglist.ElementAt( index );
      }
      Logger.Instance.Log( $"awyTable: Routing: No segment found" );
      return null; // nothing found..
    }

  }
}


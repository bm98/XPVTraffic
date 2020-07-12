using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using libXPVTgen.coordlib;

namespace libXPVTgen.my_rwylib
{
  /// <summary>
  ///  key: start_reg_end_reg
  /// </summary>
  public class rwyTable : Dictionary<string, rwyRec>
  {
    private static Random m_random = new Random( );


    public rwyTable()
    {
    }

    /// <summary>
    /// Create an ICAO table from the given table
    /// </summary>
    /// <param name="prefix">The prefix of the table</param>
    /// <param name="table">The source to fill from</param>
    public rwyTable( rwyTable table )
    {
      this.AddSubtable( table );
    }


    /// <summary>
    /// Add one record to the table
    /// </summary>
    /// <param name="rec"></param>
    public string Add( rwyRec rec )
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
    public string AddSubtable( rwyTable subtable )
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
    private string AddSubtable( IEnumerable<KeyValuePair<string, rwyRec>> selection )
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
    /// Return an Runway subtable where airport_runwy key matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>A rwyTable</returns>
    public rwyTable GetSubtable( string icao_key )
    {
      var l = this.Where( x => x.Key == icao_key );

      var t = new rwyTable( );
      foreach ( var kv in l ) t.Add( kv.Key, kv.Value );
      return t;
    }

    /// <summary>
    /// Return an Runway subtable for Runways in range
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>A rwyTable</returns>
    public rwyTable GetSubtable( double rangeLimitNm, double Lat, double Lon )
    {
      var nT = new rwyTable( );

      var myLoc = new LatLon( Lat, Lon );
      foreach ( var rec in this ) {
        var dist = myLoc.DistanceTo( rec.Value.start_latlon, ConvConsts.EarthRadiusNm );
        if ( dist <= rangeLimitNm ) {
          nT.Add( rec.Value );
        }
      }
      return nT;
    }


    /// <summary>
    /// Return a sorted Runway subtable where airport_runwy key matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>A sorted rwyTable</returns>
    public rwyTable GetSortedSubtable( string icao_key )
    {
      var l = GetSubtable( icao_key ).OrderBy( key => key.Key );
      return ( l.ToDictionary( ( keyItem ) => keyItem.Key, ( valueItem ) => valueItem.Value ) as rwyTable );
    }


  }
}


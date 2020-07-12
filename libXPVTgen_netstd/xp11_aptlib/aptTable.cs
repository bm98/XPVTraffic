using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using libXPVTgen.xp11_navlib;

namespace libXPVTgen.xp11_aptlib
{
  /// <summary>
  ///  key: start_reg_end_reg
  /// </summary>
  public class aptTable : Dictionary<string, aptRec>
  {

    public aptTable()
    {
    }

    /// <summary>
    /// Create an ICAO table from the given table
    /// </summary>
    /// <param name="prefix">The prefix of the table</param>
    /// <param name="table">The source to fill from</param>
    public aptTable( aptTable table )
    {
      this.AddSubtable( table );
    }


    /// <summary>
    /// Add one record to the table
    /// </summary>
    /// <param name="rec"></param>
    public string Add( aptRec rec )
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
    public string AddSubtable( aptTable subtable )
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
    private string AddSubtable( IEnumerable<KeyValuePair<string, aptRec>> selection )
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
    /// Return an Airport subtable where airport_runwy key matches
    /// /// </summary>
    /// <param name="icao_key">The ICAO name of runways to select</param>
    /// <returns>A table with selected records</returns>
    public aptTable GetSubtable( string icao_key )
    {
      var l = this.Where( x => x.Key == icao_key );

      var t = new aptTable( );
      foreach ( var kv in l ) t.Add( kv.Key, kv.Value );
      return t;
    }


    /// <summary>
    /// Return a sorted Airport subtable where airport_runwy key matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>A sorted aptTable</returns>
    public aptTable GetSortedSubtable( string icao_key )
    {
      var l = GetSubtable( icao_key ).OrderBy( key => key.Key );
      return ( l.ToDictionary( ( keyItem ) => keyItem.Key, ( valueItem ) => valueItem.Value ) as aptTable );
    }


  }
}


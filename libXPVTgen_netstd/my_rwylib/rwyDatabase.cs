using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace libXPVTgen.my_rwylib
{
  /// <summary>
  /// Maintains the XPlane Runway Items database
  /// </summary>
  public class rwyDatabase
  {

    private rwyTable m_db = null;

    /// <summary>
    /// cTor: init the database
    /// </summary>
    public rwyDatabase()
    {
      m_db = new rwyTable( );
    }

    /// <summary>
    /// Add one record to the table
    /// </summary>
    /// <param name="rec">The awyRec to add</param>
    public string Add( rwyRec rec )
    {
      if ( rec != null ) {
        return m_db.Add( rec );
      }
      return "";
    }

    /// <summary>
    /// Returns the number of records in the database
    /// </summary>
    public int Count
    {
      get {
        return m_db.Count;
      }
    }

    /// <summary>
    /// Return the complete table
    /// </summary>
    /// <returns></returns>
    public rwyTable GetTable()
    {
      return m_db;
    }

    /// <summary>
    /// Return an Airport subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The ICAO start or end name of airways to select</param>
    /// <returns>A table with selected records</returns>
    public rwyTable GetSubtable( string icao_key )
    {
      return m_db.GetSubtable( icao_key );
    }

    /// <summary>
    /// Return an Airport subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>An aptTable</returns>
    public rwyTable GetSubtable( double rangeLimitNm, double Lat, double Lon )
    {
      return m_db.GetSubtable( rangeLimitNm, Lat, Lon );
    }


    /// <summary>
    /// Load from a Runway DB from the X11 Airport db (only runway records are collected)
    /// </summary>
    /// <param name="APTDB">An Airport DB</param>
    public void LoadFromX11DB( xp11_aptlib.aptDatabase APTDB )
    {
      foreach ( var aptrec in APTDB.GetTable( ) ) {
        // process all runways
        var r = aptrec.Value;
        // find the reverse runway
        var rev = APTDB.GetSubtable( r.rev_rwy_ident );
        if ( rev.Count == 1 ) {
          var rwy = new rwyRec( r.icao_id, r.rwy_num, r.rwy_side, r.elevation, r.lat, r.lon, rev.ElementAt( 0 ).Value.lat, rev.ElementAt( 0 ).Value.lon );
          this.Add( rwy );
        }
        else {
          ; // DEBUG STOP
        }
      }
    }



  }
}

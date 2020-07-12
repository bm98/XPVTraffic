using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace libXPVTgen.xp11_aptlib
{
  /// <summary>
  /// Maintains the XPlane Airport Items database
  /// </summary>
  public class aptDatabase
  {

    private aptTable m_db = null;

    /// <summary>
    /// cTor: init the database
    /// </summary>
    public aptDatabase()
    {
      m_db = new aptTable( );
    }

    /// <summary>
    /// Add one record to the table
    /// </summary>
    /// <param name="rec">The awyRec to add</param>
    public string Add( aptRec rec )
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
    public aptTable GetTable()
    {
      return m_db;
    }

    /// <summary>
    /// Return an Airport subtable where airport_runwy key matches
    /// /// </summary>
    /// <param name="icao_key">The ICAO name of runways to select</param>
    /// <returns>A table with selected records</returns>
    public aptTable GetSubtable( string icao_key )
    {
      return m_db.GetSubtable( icao_key );
    }

    /// <summary>
    /// Return an Airport subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The ICAO start or end name of airways to select</param>
    /// <returns>A sorted table with selected records</returns>
    public aptTable GetSortedSubtable( string icao_key )
    {
      return m_db.GetSortedSubtable( icao_key );
    }

  }
}

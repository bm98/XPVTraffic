using System;
using System.Collections.Generic;
using System.Text;
using static libXPVTgen.xp11_navlib.navRec;

namespace libXPVTgen.xp11_navlib
{
  /// <summary>
  /// Maintains the XPlane Nav Items database
  /// </summary>
  public class navDatabase
  {

    private navTable m_db = null;

    /// <summary>
    /// cTor: init the database
    /// </summary>
    public navDatabase()
    {
      m_db = new navTable( );
    }

    /// <summary>
    /// Add one record to the table
    /// </summary>
    /// <param name="rec"></param>
    public string Add( navRec rec )
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
    /// <returns>The complete table</returns>
    public navTable GetTable()
    {
      return m_db;
    }

    /// <summary>
    /// Returns a subtable with items that match the given criteria
    /// </summary>
    /// <param name="rangeLimitNm">Range Limit in nm</param>
    /// <param name="Lat">Center Lat (decimal)</param>
    /// <param name="Lon">Center Lon (decimal)</param>
    /// <param name="navTypes">Type of nav items to include</param>
    /// <returns>A table with selected records</returns>
    public navTable GetSubtable( double rangeLimitNm, double Lat, double Lon, NavTypes[] navTypes = null )
    {
      return m_db.GetSubtable( rangeLimitNm, Lat, Lon, navTypes );
    }


  }
}

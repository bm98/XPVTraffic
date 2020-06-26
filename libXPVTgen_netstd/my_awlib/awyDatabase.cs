using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace libXPVTgen.my_awlib
{
  /// <summary>
  /// Maintains the XPlane Airway Items database
  /// </summary>
  public class awyDatabase
  {

    private awyTable m_db = null;

    /// <summary>
    /// cTor: init the database
    /// </summary>
    public awyDatabase()
    {
      m_db = new awyTable( );
    }

    /// <summary>
    /// Add one record to the table
    /// </summary>
    /// <param name="rec">The awyRec to add</param>
    public string Add( awyRec rec )
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
    public awyTable GetTable()
    {
      return m_db;
    }

    /// <summary>
    /// Return an Airway subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The ICAO start or end name of airways to select</param>
    /// <returns>A table with selected records</returns>
    public awyTable GetSubtable( string icao_key )
    {
      return m_db.GetSubtable( icao_key );
    }

    /// <summary>
    /// Return an Airway subtable where either start or end ICAO designator matches
    /// </summary>
    /// <param name="icao_key">The icao to match</param>
    /// <returns>An awyTable</returns>
    public awyTable GetSubtable( double rangeLimitNm, double Lat, double Lon )
    {
      return m_db.GetSubtable( rangeLimitNm, Lat, Lon );
    }

  }
  }

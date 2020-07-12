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


    /// <summary>
    /// Load from XP11 Nav and Airway DBs
    /// </summary>
    /// <param name="NAVDB">An XP11 NavDB</param>
    /// <param name="AWYDB">An XP11 AirwayDB</param>
    public void LoadFromX11DB ( xp11_navlib.navDatabase NAVDB, xp11_awylib.awyDatabase AWYDB )
    {
      foreach ( var awyrec in AWYDB.GetTable( ) ) {
        // process all airways
        if ( NAVDB.GetTable( ).ContainsKey( awyrec.Value.startID )
          && NAVDB.GetTable( ).ContainsKey( awyrec.Value.endID )
          && awyrec.Value.baselevel > 0
          && awyrec.Value.toplevel > 0
          && ( awyrec.Value.startID != awyrec.Value.endID ) ) {
          // endpoints must be known and not the same, levels above 000 
          var startFix = NAVDB.GetTable( )[awyrec.Value.startID];
          var endFix = NAVDB.GetTable( )[awyrec.Value.endID];

          if ( awyrec.Value.restriction == "F" || awyrec.Value.restriction == "N" ) {
            // get forward path
            var ar = new my_awlib.awyRec(
              startFix.icao_id, startFix.icao_region, startFix.lat.ToString( ), startFix.lon.ToString( ),
              endFix.icao_id, endFix.icao_region, endFix.lat.ToString( ), endFix.lon.ToString( ),
              awyrec.Value.layer.ToString( ), awyrec.Value.Base_ft.ToString( ), awyrec.Value.Top_ft.ToString( ), awyrec.Value.name );
            this.Add( ar );
          }
          if ( awyrec.Value.restriction == "B" || awyrec.Value.restriction == "N" ) {
            // get backward path
            var ar = new my_awlib.awyRec(
              endFix.icao_id, endFix.icao_region, endFix.lat.ToString( ), endFix.lon.ToString( ),
              startFix.icao_id, startFix.icao_region, startFix.lat.ToString( ), startFix.lon.ToString( ),
              awyrec.Value.layer.ToString( ), awyrec.Value.Base_ft.ToString( ), awyrec.Value.Top_ft.ToString( ), awyrec.Value.name );
            this.Add( ar );
          }
        }
      }


    }


  }
}

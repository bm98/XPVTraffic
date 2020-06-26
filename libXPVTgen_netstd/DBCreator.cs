using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.xp11_awylib;
using libXPVTgen.xp11_navlib;

namespace libXPVTgen
{
  /// <summary>
  /// Creates the application Airways file (my_awy.dat) in the application folder
  /// NOTE: does NOT overwrite any existing file..
  /// </summary>
  public class DBCreator
  {
    public const string MyDbName = "my_awy.dat";
    // Traffic 
    private navDatabase NAVDB = new navDatabase( );
    private awyDatabase AWYDB = new awyDatabase( );

    public bool Valid { get; private set; } = false;
    public string Error { get; private set; } = "";

    public DBCreator(  )
    {
#if DEBUG
      Logger.Instance.Logging = true;
#endif

      Error = fixReader.ReadDb( ref NAVDB, XP11.FixDatFile );
      Logger.Instance.Log( $"DBCreator-FixFile: {Error}" );
      if ( !string.IsNullOrEmpty( Error ) )  return; // ERROR exit

      Error = fixReader.ReadDb( ref NAVDB, XP11.NavDatFile );
      Logger.Instance.Log( $"DBCreator-NavFile: {Error}" );
      if ( !string.IsNullOrEmpty( Error ) ) return; // ERROR exit

      Error = awyReader.ReadDb( ref AWYDB, XP11.AwyDatFile );
      Logger.Instance.Log( $"DBCreator-AwyFile: {Error}" );
      if ( !string.IsNullOrEmpty( Error ) ) return; // ERROR exit

      Valid = true;
      Logger.Instance.Log( $"DBCreator-end: Valid" );
    }

    /// <summary>
    /// Creates the dbfile for our internal use
    /// </summary>
    /// <param name="myDataPath"></param>
    /// <returns></returns>
    public string CreateDbFile( string myDataPath )
    {
      Logger.Instance.Log( $"DBCreator-CreateDbFile" );
      if ( !Valid ) {
        Error = "$Error: Init failed, check XP11 base path and earth_nav, earth_fix, earth_awy files";
        Logger.Instance.Log( $"DBCreator: {Error}" );
        return Error; // ERROR exit
      }

      string eawy = Path.Combine( myDataPath, MyDbName );
      if ( File.Exists( eawy ) ) {
        Error = "$Error: DB File already exists"; // ERROR exit
        Logger.Instance.Log( $"DBCreator: {Error}" );
        return Error;
      }

      var MYAWDB = new my_awlib.awyDatabase( );
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
            MYAWDB.Add( ar );
          }
          if ( awyrec.Value.restriction == "B" || awyrec.Value.restriction == "N" ) {
            // get backward path
            var ar = new my_awlib.awyRec(
              endFix.icao_id, endFix.icao_region, endFix.lat.ToString( ), endFix.lon.ToString( ),
              startFix.icao_id, startFix.icao_region, startFix.lat.ToString( ), startFix.lon.ToString( ),
              awyrec.Value.layer.ToString( ), awyrec.Value.Base_ft.ToString( ), awyrec.Value.Top_ft.ToString( ), awyrec.Value.name );
            MYAWDB.Add( ar );
          }
        }
      }

      Error = my_awlib.awyWriter.WriteDb( MYAWDB, eawy );
      Logger.Instance.Log( $"DBCreator-CreateDbFile ended: {Error}" );
      return Error;
    }


  }
}

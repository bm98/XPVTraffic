using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libXPVTgen.xp11_aptlib;
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
    public const string MyAwyDbName = "my_awy.dat";
    public const string MyRwyDbName = "my_rwy.dat";
    public const string MyVfrScriptPath = "vfrScripts";

    // Traffic 
    private navDatabase NAVDB = new navDatabase( );
    private awyDatabase AWYDB = new awyDatabase( );
    private aptDatabase APTDB = new aptDatabase( );

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

      Error = aptReader.ReadDb( ref APTDB, XP11.AptDatPath );
      Logger.Instance.Log( $"DBCreator-AptPath: {Error}" );
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

      // delete existing files
      string eawy = Path.Combine( myDataPath, MyAwyDbName );
      if ( File.Exists( eawy ) )  File.Delete( eawy );

      string erwy = Path.Combine( myDataPath, MyRwyDbName );
      if ( File.Exists( erwy ) ) File.Delete( erwy );

      var MYAWDB = new my_awlib.awyDatabase( );
      MYAWDB.LoadFromX11DB( NAVDB, AWYDB );
      Error = my_awlib.awyWriter.WriteDb( MYAWDB, eawy );
      Logger.Instance.Log( $"DBCreator-CreateMyAWYDB ended: {Error}" );

      var MYRWYDB = new my_rwylib.rwyDatabase( );
      MYRWYDB.LoadFromX11DB( APTDB );
      Error = my_rwylib.rwyWriter.WriteDb( MYRWYDB, erwy );
      Logger.Instance.Log( $"DBCreator-CreateMyRWYDB ended: {Error}" );

      return Error;
    }


  }
}

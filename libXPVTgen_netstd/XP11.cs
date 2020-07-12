using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libXPVTgen
{
  /// <summary>
  /// Various X-Plane 11 related items
  /// Ref: https://developer.x-plane.com/?article=navdata-in-x-plane-11
  /// 
  /// Note: does only support the XPNAV1100/XPNAV1150 /XPFIX1101 /XPAWY1100 formats
  /// does not support earth_424.dat
  /// </summary>
  public class XP11
  {
    /// <summary>
    /// Base Path to X-Plane 11 (MUST BE SET)
    /// </summary>
    public static string BasePath { get; set; } = "";

    public static string CustomDataPath { get => Path.Combine( BasePath, "Custom Data" ); }
    public static string DefaultDatPath { get => Path.Combine( BasePath, "Resources", "default data" ); }

    /// <summary>
    /// The Fix File (fully qualified)
    /// </summary>
    public static string FixDatFile
    {
      get {
        string f = Path.Combine( CustomDataPath, "earth_fix.dat" );
        if ( File.Exists( f ) ) {
          return f;
        }
        f = Path.Combine( DefaultDatPath, "earth_fix.dat" );
        if ( File.Exists( f ) ) {
          return f;
        }
        else {
          return "";
        }
      }
    }

    /// <summary>
    /// The Nav File (fully qualified)
    /// </summary>
    public static string NavDatFile
    {
      get {
        string f = Path.Combine( CustomDataPath, "earth_nav.dat" );
        if ( File.Exists( f ) ) {
          return f;
        }
        f = Path.Combine( DefaultDatPath, "earth_nav.dat" );
        if ( File.Exists( f ) ) {
          return f;
        }
        else {
          return "";
        }
      }
    }

    /// <summary>
    /// The Awy File (fully qualified)
    /// </summary>
    public static string AwyDatFile
    {
      get {
        string f = Path.Combine( CustomDataPath, "earth_awy.dat" );
        if ( File.Exists( f ) ) {
          return f;
        }
        f = Path.Combine( DefaultDatPath, "earth_awy.dat" );
        if ( File.Exists( f ) ) {
          return f;
        }
        else {
          return "";
        }
      }
    }

    /// <summary>
    /// The Apt File (fully qualified)
    /// </summary>
    public static string AptDatPath
    {
      get {
        string f = Path.Combine( CustomDataPath, "CIFP" );
        if ( Directory.Exists( f ) ) {
          return f;
        }
        f = Path.Combine( DefaultDatPath, "CIFP" );
        if ( Directory.Exists( f ) ) {
          return f;
        }
        else {
          return "";
        }
      }
    }


  }
}

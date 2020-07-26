using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPTsim
{
  sealed class AppSettings : ApplicationSettingsBase
  {

    // Singleton
    private static readonly Lazy<AppSettings> m_lazy = new Lazy<AppSettings>( () => new AppSettings( ) );
    public static AppSettings Instance { get => m_lazy.Value; }

    private AppSettings()
    {
      if ( this.FirstRun ) {
        // migrate the settings to the new version if the app runs the first time
        try {
          this.Upgrade( );
        }
        catch { }
        this.FirstRun = false;
        this.Save( );
      }
    }

    #region Setting Properties

    // manages Upgrade
    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool FirstRun
    {
      get { return (bool)this["FirstRun"]; }
      set { this["FirstRun"] = value; }
    }


    // Control bound settings
    [UserScopedSetting( )]
    [DefaultSettingValue( "10, 10" )]
    public Point FormLocation
    {
      get { return (Point)this["FormLocation"]; }
      set { this["FormLocation"] = value; }
    }

    // User Config Settings

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Logging
    {
      get { return (bool)this["Logging"]; }
      set { this["Logging"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "127.0.0.1" )]
    public string LiveTrafficIP
    {
      get { return (string)this["LiveTrafficIP"]; }
      set { this["LiveTrafficIP"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( @"E:\G\Steam\SteamApps\common\X-Plane 11" )]
    public string XP11BasePath
    {
      get { return (string)this["XP11BasePath"]; }
      set { this["XP11BasePath"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "LSZH_RW28" )]
    public string FallbackRwy
    {
      get { return (string)this["FallbackRwy"]; }
      set { this["FallbackRwy"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "50" )]
    public decimal NumAircrafts
    {
      get { return (decimal)this["NumAircrafts"]; }
      set { this["NumAircrafts"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "25" )]
    public decimal NumVFRAircrafts
    {
      get { return (decimal)this["NumVFRAircrafts"]; }
      set { this["NumVFRAircrafts"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool ConvertFile
    {
      get { return (bool)this["ConvertFile"]; }
      set { this["ConvertFile"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool AbsolutePos
    {
      get { return (bool)this["AbsolutePos"]; }
      set { this["AbsolutePos"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool IgnoreAirborne
    {
      get { return (bool)this["IgnoreAirborne"]; }
      set { this["IgnoreAirborne"] = value; }
    }

    #endregion


  }
}

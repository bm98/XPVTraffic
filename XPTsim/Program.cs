using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XPTsim
{
  static class Program
  {

    // Thanks.. http://blog.rastating.com/setting-default-currentculture-in-all-versions-of-net/
    static void SetDefaultCulture( CultureInfo culture )
    {
      // The CultureInfo class has two private static members named m_userDefaultCulture 
      // and m_userDefaultUICulture in versions prior to .NET 4.0; 
      // in 4.0 they are named s_userDefaultCulture and s_userDefaultUICulture.

      Type type = typeof(CultureInfo);

      try {
        type.InvokeMember( "s_userDefaultCulture",
                            BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                            null,
                            culture,
                            new object[] { culture } );

        type.InvokeMember( "s_userDefaultUICulture",
                            BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                            null,
                            culture,
                            new object[] { culture } );
      }
      catch { }

      try {
        type.InvokeMember( "m_userDefaultCulture",
                            BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                            null,
                            culture,
                            new object[] { culture } );

        type.InvokeMember( "m_userDefaultUICulture",
                            BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                            null,
                            culture,
                            new object[] { culture } );
      }
      catch { }
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // 20210402BM - fix culture related decimal - force decimal point throughout the application
      CultureInfo current = CultureInfo.CurrentCulture;
      var modded = new CultureInfo( current.Name ); // that is the users locale
      Console.WriteLine( "XPTsim - current culture : {0}", modded.Name );
      Console.WriteLine( "XPTsim - current decimal separator : {0}", modded.NumberFormat.NumberDecimalSeparator );
      var us = new CultureInfo( "en-US" );
      modded.NumberFormat = us.NumberFormat;  // change the whole number format to US - should be safe ...
      Console.WriteLine( "XPTsim - newly used decimal separator : {0}", modded.NumberFormat.NumberDecimalSeparator );
      // change the applications formatting to US (the dec point is essential here)
      SetDefaultCulture( modded ); // have to maintain number formats without tracking every piece of code with locales

      // regular startup proc
      Application.EnableVisualStyles( );
      Application.SetCompatibleTextRenderingDefault( false );
      Application.Run( new Form1( ) );
    }
  }
}

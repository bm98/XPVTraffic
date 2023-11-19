using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using libXPVTgen;

namespace XPTsim
{
  public partial class Form1 : Form
  {
    private const string AppName = "X-Plane 11 - Virtual LiveTraffic";

    // config
    private string m_dataLocation = ""; // localtion of my_awy.dat file - "" => app dir
    private uint m_stepLength_Sec = 5;  // defines the pace to update traffic in LiveTraffic (2..10 sec should do it)

    private string IP = "127.0.0.1"; // localhost default to init the GUI
    private TrafficHandler TH = null;

    private long m_lastPing;

    /// <summary>
    /// Checks if a rectangle is visible on any screen
    /// </summary>
    /// <param name="formRect"></param>
    /// <returns>True if visible</returns>
    private static bool IsOnScreen( Rectangle formRect )
    {
      Screen[] screens = Screen.AllScreens;
      foreach ( Screen screen in screens ) {
        if ( screen.WorkingArea.Contains( formRect ) ) {
          return true;
        }
      }
      return false;
    }

    public Form1()
    {
      InitializeComponent( );
      txIP.Text = IP;
    }

    private void Form1_Load( object sender, EventArgs e )
    {
      AppSettings.Instance.Reload( );
      // Assign Size property - check if on screen, else use defaults
      if ( IsOnScreen( new Rectangle( AppSettings.Instance.FormLocation, this.Size ) ) ) {
        this.Location = AppSettings.Instance.FormLocation;
      }

      string version = Application.ProductVersion;  // get the version information
      // BETA VERSION; TODO -  comment out if not longer
      //lblTitle.Text += " - V " + version.Substring( 0, version.IndexOf( ".", version.IndexOf( "." ) + 1 ) ); // PRODUCTION
      lblVersion.Text = "Version: " + version + " beta"; // BETA

#if DEBUG
      btSimIFR.Visible = true; // show IFR Sim only in debug mode, creates random IFR scripts
      btDumpIFR.Visible = true;// show IFR Dump only in debug mode, find and dump all IFRs
#endif

      cbxLogging.Checked = AppSettings.Instance.Logging;
      txIP.Text = AppSettings.Instance.LiveTrafficIP;
      txBasePath.Text = AppSettings.Instance.XP11BasePath;
      txRwy.Text = AppSettings.Instance.FallbackRwy;
      numTotalAC.Value = AppSettings.Instance.NumAircrafts;
      numVFR.Value = AppSettings.Instance.NumVFRAircrafts;
      cbxConvertFile.Checked = AppSettings.Instance.ConvertFile;
      cbxAbsolutePos.Checked = AppSettings.Instance.AbsolutePos;
      cbxIgnoreAirborne.Checked = AppSettings.Instance.IgnoreAirborne;
    }

    private void Form1_FormClosing( object sender, FormClosingEventArgs e )
    {
      // don't record minimized, maximized forms
      if ( this.WindowState == FormWindowState.Normal ) {
        AppSettings.Instance.FormLocation = this.Location;
      }
      AppSettings.Instance.Logging = cbxLogging.Checked;
      AppSettings.Instance.LiveTrafficIP = txIP.Text;
      AppSettings.Instance.XP11BasePath = txBasePath.Text;
      AppSettings.Instance.FallbackRwy = txRwy.Text;
      AppSettings.Instance.NumAircrafts = numTotalAC.Value;
      AppSettings.Instance.NumVFRAircrafts = numVFR.Value;
      AppSettings.Instance.ConvertFile = cbxConvertFile.Checked;
      AppSettings.Instance.AbsolutePos = cbxAbsolutePos.Checked;
      AppSettings.Instance.IgnoreAirborne = cbxIgnoreAirborne.Checked;
      AppSettings.Instance.Save( );
    }



    private void btCreateLink_Click( object sender, EventArgs e )
    {
      lblLink.Text = "...";
      btCreateLink.Enabled = false;
      TH = new TrafficHandler( m_dataLocation, m_stepLength_Sec, cbxLogging.Enabled );

      if ( TH.Valid ) {
        bool ret = TH.EstablishLink( IP, (uint)numTotalAC.Value, (uint)numVFR.Value );
        if ( ret ) {
          TH.TrafficEvent += TH_TrafficEvent;
          btCreateLink.BackColor = Color.ForestGreen;
          timer1.Interval = 1000; // 1 sec only
          timer1.Enabled = true;
        }
        else {
          // error on establish link
          lblLink.Text = TH.Error;
          btCreateLink.Enabled = true;
          btCreateLink.BackColor = Color.IndianRed;
        }
      }
      else {
        // invalid datafile..
        lblLink.Text = TH.Error;
        btCreateLink.Enabled = true;
      }
    }

    private void TH_TrafficEvent( object sender, TrafficEventArgs e )
    {
      m_lastPing = e.PingSeconds;
      m_pingTog = !m_pingTog;
    }

    private void btDropLink_Click( object sender, EventArgs e )
    {
      timer1.Enabled = false;
      if ( TH != null ) {
        TH.TrafficEvent -= TH_TrafficEvent;
        TH.RemoveLink( );
        lblLink.Text = TH.Error;
      }
      btCreateLink.BackColor = btDropLink.BackColor; // reset
      TH = null;
      btCreateLink.Enabled = true;
    }

    private void btSimVFR_Click( object sender, EventArgs e )
    {
      lblLink.Text = "...";
      btSimVFR.BackColor = btDropLink.BackColor; // this is the native button color...

      OFD.Filter = "Script Files|*.vsc|All Files|*.*";
      OFD.Title = "Load VFR Model File";
      if ( OFD.ShowDialog( this ) == DialogResult.OK ) {
        lblLink.Text = $"Simulation of: {OFD.FileName}\n";
        var sim = new VFRSimulation( m_dataLocation, 2, cbxLogging.Enabled );
        if ( sim.Valid ) {
          btSimVFR.BackColor = Color.ForestGreen;

          if ( !sim.RunSimulation( OFD.FileName, txRwy.Text ) ) {
            lblLink.Text += sim.Error;
            btSimVFR.BackColor = Color.IndianRed;
          }
          else {
            lblLink.Text += "KML File created in script folder";
          }
        }
        else {
          // error 
          lblLink.Text += sim.Error;
          btSimVFR.BackColor = Color.IndianRed;
        }
        lblLink.Text += $"DONE";
      }
    }

    private void btSimIFR_Click( object sender, EventArgs e )
    {
      lblLink.Text = "...";

      var sim = new VFRSimulation( m_dataLocation, 2, cbxLogging.Enabled );
      if ( !sim.RunIFRSim( ) ) {
        lblLink.Text = sim.Error;
        btSimVFR.BackColor = Color.IndianRed;
      }
      else {
        lblLink.Text = "KML File created in appDir";
      }
    }

    private void btDumpIFR_Click( object sender, EventArgs e )
    {
      lblLink.Text = "...";

      var sim = new VFRSimulation( m_dataLocation, 2, cbxLogging.Enabled );
      sim.DumpIFR( );
    }

    private void btConvertAIT_Click( object sender, EventArgs e )
    {
      lblLink.Text = "";
      lblConvert.Text = "";
      btConvertAIT.BackColor = btDropLink.BackColor; // this is the native button color...

      int numFiles = 0;
      OFD.Title = "Convert RTTraffic Files";
      OFD.Multiselect = false;
      OFD.Filter = "RTT Files|*.csv";

      var fList = new List<string>( );
      if ( OFD.ShowDialog( this ) == DialogResult.OK ) {
        if ( cbxConvertFile.Checked ) {
          lblLink.Text = $"Converting {OFD.FileName}\n";
          fList.Add( OFD.FileName );
          numFiles = 1;
        }
        else {
          lblLink.Text = $"Converting Folder {Path.GetDirectoryName( OFD.FileName )}\n";
          fList = Directory.EnumerateFiles( Path.GetDirectoryName( OFD.FileName ), "*.csv", SearchOption.AllDirectories ).ToList( );
          numFiles = fList.Count( );
        }
        int fconv = 0;
        foreach ( var file in fList ) {
          var AIC = new RTTConverter( );
          if ( !AIC.CreateRouteScript( file, cbxAbsolutePos.Checked, cbxIgnoreAirborne.Checked ) ) {
            lblLink.Text += $"Conv error: {file}\n\t{AIC.Error}\n";
          }
          lblConvert.Text = $"{++fconv} of {numFiles} converted";
        }
        lblLink.Text += $"DONE";
      }
    }

    private void btCreateDB_Click( object sender, EventArgs e )
    {
      lblCreate.Text = "...";

      btCreateDB.Enabled = false;
      if ( MessageBox.Show( $"We are about to create new database files\nThis may take a while!\nShall we continue?", "Create Database Files", MessageBoxButtons.YesNo ) == DialogResult.Yes ) {
        XP11.BasePath = txBasePath.Text; // MUST be set before using DBCreator
        var DBC = new DBCreator( );
        if ( DBC.Valid ) {
          string ret = DBC.CreateDbFile( m_dataLocation );
          if ( string.IsNullOrEmpty( ret ) )
            lblCreate.Text = "DONE";
          else
            lblCreate.Text = ret;
        }
        else {
          lblCreate.Text = DBC.Error;
        }

      }
      btCreateDB.Enabled = true;
    }

    private void btBasePath_Click( object sender, EventArgs e )
    {
      FLD.SelectedPath = txBasePath.Text;
      if ( FLD.ShowDialog( this ) == DialogResult.OK ) {
        txBasePath.Text = FLD.SelectedPath;
      }
    }


    private bool m_pingTog = true;
    private void timer1_Tick( object sender, EventArgs e )
    {
      lblPing.Text = ( m_pingTog ) ? @"/" + m_lastPing.ToString( ) : @"\" + m_lastPing.ToString( );
      lblPing.Text += $" of {m_stepLength_Sec} sec";
    }

    private void txRwy_TextChanged( object sender, EventArgs e )
    {

    }
  }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using libXPVTgen;

namespace XPTsim
{
  public partial class Form1 : Form
  {
    // config
    private string m_dataLocation = ""; // localtion of my_awy.dat file - "" => app dir
    private uint m_stepLength_Sec = 5;  // defines the pace to update traffic in LiveTraffic (2..10 sec should do it)

    private string IP = "127.0.0.1"; // localhost default to init the GUI
    private TrafficHandler TH = null;

    private long m_lastPing;

    public Form1()
    {
      InitializeComponent( );
      txIP.Text = IP;
    }

    private void btBasePath_Click( object sender, EventArgs e )
    {
      FLD.SelectedPath = txBasePath.Text;
      if ( FLD.ShowDialog( this ) == DialogResult.OK ) {
        txBasePath.Text = FLD.SelectedPath;
      }
    }


    private void btSimVFR_Click( object sender, EventArgs e )
    {
      OFD.Title = "Load VFR Model File";
      if ( OFD.ShowDialog( this ) == DialogResult.OK ) {
        var sim = new VFRSimulation( m_dataLocation, 2, cbxLogging.Enabled );
        if ( sim.Valid ) {
          btSimVFR.BackColor = Color.ForestGreen;

          if ( !sim.RunSimulation( OFD.FileName ) ) {
            lblLink.Text = sim.Error;
            btSimVFR.BackColor = Color.IndianRed;
          }
          else {
            lblLink.Text = "KML File created in script folder";
          }
        }
        else {
          // error 
          lblLink.Text = sim.Error;
          btSimVFR.BackColor = Color.IndianRed;
        }
      }
    }

    private void btCreateLink_Click( object sender, EventArgs e )
    {
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

    private void btCreateDB_Click( object sender, EventArgs e )
    {
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

    private bool m_pingTog = true;
    private void timer1_Tick( object sender, EventArgs e )
    {
      lblPing.Text = ( m_pingTog ) ? @"/" + m_lastPing.ToString( ) : @"\" + m_lastPing.ToString( );
    }


  }
}

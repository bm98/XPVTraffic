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
    private void btCreateLink_Click( object sender, EventArgs e )
    {
      btCreateLink.Enabled = false;
      TH = new TrafficHandler( m_dataLocation, m_stepLength_Sec );
      if ( TH.Valid ) {
        bool ret = TH.EstablishLink( IP );
        if ( ret ) {
          btCreateLink.BackColor = Color.ForestGreen;
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


    private void btDropLink_Click( object sender, EventArgs e )
    {
      TH?.RemoveLink( );
      if ( TH != null ) lblLink.Text = TH.Error;
      btCreateLink.BackColor = btDropLink.BackColor; // reset
      TH = null;
      btCreateLink.Enabled = true;
    }

    private void btCreateDB_Click( object sender, EventArgs e )
    {
      btCreateDB.Enabled = false;
      XP11.BasePath = txBasePath.Text; // MUST be set before using DBCreator
      var DBC = new DBCreator(  );
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
      btCreateDB.Enabled = true;
    }

  }
}

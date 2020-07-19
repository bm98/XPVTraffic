namespace XPTsim
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if ( disposing && ( components != null ) ) {
        components.Dispose( );
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.txBasePath = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btBasePath = new System.Windows.Forms.Button();
      this.btCreateLink = new System.Windows.Forms.Button();
      this.btDropLink = new System.Windows.Forms.Button();
      this.btCreateDB = new System.Windows.Forms.Button();
      this.lblCreate = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txIP = new System.Windows.Forms.TextBox();
      this.lblLink = new System.Windows.Forms.Label();
      this.FLD = new System.Windows.Forms.FolderBrowserDialog();
      this.numTotalAC = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.numVFR = new System.Windows.Forms.NumericUpDown();
      this.cbxLogging = new System.Windows.Forms.CheckBox();
      this.btSimVFR = new System.Windows.Forms.Button();
      this.OFD = new System.Windows.Forms.OpenFileDialog();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.lblPing = new System.Windows.Forms.Label();
      this.txRwy = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.btSimIFR = new System.Windows.Forms.Button();
      this.lblVersion = new System.Windows.Forms.Label();
      this.btDumpIFR = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.numTotalAC)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numVFR)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // txBasePath
      // 
      this.txBasePath.Location = new System.Drawing.Point(101, 38);
      this.txBasePath.Name = "txBasePath";
      this.txBasePath.Size = new System.Drawing.Size(310, 22);
      this.txBasePath.TabIndex = 0;
      this.txBasePath.Text = "E:\\G\\Steam\\SteamApps\\common\\X-Plane 11";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(98, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(160, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "XP 11 Base Path for Create DB:";
      // 
      // btBasePath
      // 
      this.btBasePath.Location = new System.Drawing.Point(368, 14);
      this.btBasePath.Name = "btBasePath";
      this.btBasePath.Size = new System.Drawing.Size(43, 21);
      this.btBasePath.TabIndex = 2;
      this.btBasePath.Text = "...";
      this.btBasePath.UseVisualStyleBackColor = true;
      this.btBasePath.Click += new System.EventHandler(this.btBasePath_Click);
      // 
      // btCreateLink
      // 
      this.btCreateLink.Location = new System.Drawing.Point(15, 102);
      this.btCreateLink.Name = "btCreateLink";
      this.btCreateLink.Size = new System.Drawing.Size(86, 39);
      this.btCreateLink.TabIndex = 3;
      this.btCreateLink.Text = "Establish Link";
      this.btCreateLink.UseVisualStyleBackColor = true;
      this.btCreateLink.Click += new System.EventHandler(this.btCreateLink_Click);
      // 
      // btDropLink
      // 
      this.btDropLink.Location = new System.Drawing.Point(330, 102);
      this.btDropLink.Name = "btDropLink";
      this.btDropLink.Size = new System.Drawing.Size(86, 39);
      this.btDropLink.TabIndex = 3;
      this.btDropLink.Text = "Drop Link";
      this.btDropLink.UseVisualStyleBackColor = true;
      this.btDropLink.Click += new System.EventHandler(this.btDropLink_Click);
      // 
      // btCreateDB
      // 
      this.btCreateDB.Location = new System.Drawing.Point(9, 21);
      this.btCreateDB.Name = "btCreateDB";
      this.btCreateDB.Size = new System.Drawing.Size(86, 39);
      this.btCreateDB.TabIndex = 4;
      this.btCreateDB.Text = "Create DB";
      this.btCreateDB.UseVisualStyleBackColor = true;
      this.btCreateDB.Click += new System.EventHandler(this.btCreateDB_Click);
      // 
      // lblCreate
      // 
      this.lblCreate.Location = new System.Drawing.Point(6, 69);
      this.lblCreate.Name = "lblCreate";
      this.lblCreate.Size = new System.Drawing.Size(405, 37);
      this.lblCreate.TabIndex = 5;
      this.lblCreate.Text = "...";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 28);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(115, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "LifeTraffic IP Address:";
      // 
      // txIP
      // 
      this.txIP.Location = new System.Drawing.Point(129, 25);
      this.txIP.Name = "txIP";
      this.txIP.Size = new System.Drawing.Size(128, 22);
      this.txIP.TabIndex = 6;
      // 
      // lblLink
      // 
      this.lblLink.AutoEllipsis = true;
      this.lblLink.BackColor = System.Drawing.Color.OldLace;
      this.lblLink.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblLink.Location = new System.Drawing.Point(15, 144);
      this.lblLink.Name = "lblLink";
      this.lblLink.Size = new System.Drawing.Size(401, 71);
      this.lblLink.TabIndex = 5;
      this.lblLink.Text = "...";
      // 
      // FLD
      // 
      this.FLD.ShowNewFolderButton = false;
      // 
      // numTotalAC
      // 
      this.numTotalAC.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numTotalAC.Location = new System.Drawing.Point(15, 74);
      this.numTotalAC.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.numTotalAC.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numTotalAC.Name = "numTotalAC";
      this.numTotalAC.Size = new System.Drawing.Size(63, 22);
      this.numTotalAC.TabIndex = 7;
      this.numTotalAC.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 58);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(80, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Total Aircrafts:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(128, 58);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(88, 13);
      this.label4.TabIndex = 10;
      this.label4.Text = "Number of VFR:";
      // 
      // numVFR
      // 
      this.numVFR.Location = new System.Drawing.Point(131, 74);
      this.numVFR.Name = "numVFR";
      this.numVFR.Size = new System.Drawing.Size(63, 22);
      this.numVFR.TabIndex = 9;
      this.numVFR.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
      // 
      // cbxLogging
      // 
      this.cbxLogging.AutoSize = true;
      this.cbxLogging.Location = new System.Drawing.Point(355, 30);
      this.cbxLogging.Name = "cbxLogging";
      this.cbxLogging.Size = new System.Drawing.Size(61, 17);
      this.cbxLogging.TabIndex = 11;
      this.cbxLogging.Text = "Logfile";
      this.cbxLogging.UseVisualStyleBackColor = true;
      // 
      // btSimVFR
      // 
      this.btSimVFR.Location = new System.Drawing.Point(9, 21);
      this.btSimVFR.Name = "btSimVFR";
      this.btSimVFR.Size = new System.Drawing.Size(86, 39);
      this.btSimVFR.TabIndex = 12;
      this.btSimVFR.Text = "Simulate VFR Model";
      this.btSimVFR.UseVisualStyleBackColor = true;
      this.btSimVFR.Click += new System.EventHandler(this.btSimVFR_Click);
      // 
      // OFD
      // 
      this.OFD.Filter = "Model Files|*.vsc|All Files|*.*";
      this.OFD.SupportMultiDottedExtensions = true;
      // 
      // timer1
      // 
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // lblPing
      // 
      this.lblPing.AutoSize = true;
      this.lblPing.Location = new System.Drawing.Point(111, 128);
      this.lblPing.Name = "lblPing";
      this.lblPing.Size = new System.Drawing.Size(16, 13);
      this.lblPing.TabIndex = 13;
      this.lblPing.Text = "...";
      // 
      // txRwy
      // 
      this.txRwy.Location = new System.Drawing.Point(118, 31);
      this.txRwy.Name = "txRwy";
      this.txRwy.Size = new System.Drawing.Size(87, 22);
      this.txRwy.TabIndex = 14;
      this.txRwy.Text = "NZWN_RW16";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(211, 34);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(93, 13);
      this.label5.TabIndex = 15;
      this.label5.Text = "Fallback Runway";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.lblCreate);
      this.groupBox1.Controls.Add(this.btCreateDB);
      this.groupBox1.Controls.Add(this.txBasePath);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.btBasePath);
      this.groupBox1.Location = new System.Drawing.Point(7, 297);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(419, 118);
      this.groupBox1.TabIndex = 16;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Database Creation";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.btSimIFR);
      this.groupBox2.Controls.Add(this.btSimVFR);
      this.groupBox2.Controls.Add(this.txRwy);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Location = new System.Drawing.Point(11, 218);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(419, 73);
      this.groupBox2.TabIndex = 17;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "VFR Script Simulation";
      // 
      // btSimIFR
      // 
      this.btSimIFR.Location = new System.Drawing.Point(319, 21);
      this.btSimIFR.Name = "btSimIFR";
      this.btSimIFR.Size = new System.Drawing.Size(94, 39);
      this.btSimIFR.TabIndex = 16;
      this.btSimIFR.Text = "Simulate IFR Model";
      this.btSimIFR.UseVisualStyleBackColor = true;
      this.btSimIFR.Visible = false;
      this.btSimIFR.Click += new System.EventHandler(this.btSimIFR_Click);
      // 
      // lblVersion
      // 
      this.lblVersion.AutoSize = true;
      this.lblVersion.Location = new System.Drawing.Point(12, -1);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(16, 13);
      this.lblVersion.TabIndex = 18;
      this.lblVersion.Text = "...";
      // 
      // btDumpIFR
      // 
      this.btDumpIFR.Location = new System.Drawing.Point(330, 58);
      this.btDumpIFR.Name = "btDumpIFR";
      this.btDumpIFR.Size = new System.Drawing.Size(86, 39);
      this.btDumpIFR.TabIndex = 19;
      this.btDumpIFR.Text = "Dump IFR Scripts";
      this.btDumpIFR.UseVisualStyleBackColor = true;
      this.btDumpIFR.Visible = false;
      this.btDumpIFR.Click += new System.EventHandler(this.btDumpIFR_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(439, 424);
      this.Controls.Add(this.btDumpIFR);
      this.Controls.Add(this.lblVersion);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.lblPing);
      this.Controls.Add(this.cbxLogging);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.numVFR);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.numTotalAC);
      this.Controls.Add(this.txIP);
      this.Controls.Add(this.lblLink);
      this.Controls.Add(this.btDropLink);
      this.Controls.Add(this.btCreateLink);
      this.Controls.Add(this.label2);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "Form1";
      this.Text = "XP11 Virtual Live Traffic";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.numTotalAC)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numVFR)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txBasePath;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btBasePath;
    private System.Windows.Forms.Button btCreateLink;
    private System.Windows.Forms.Button btDropLink;
    private System.Windows.Forms.Button btCreateDB;
    private System.Windows.Forms.Label lblCreate;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txIP;
    private System.Windows.Forms.Label lblLink;
    private System.Windows.Forms.FolderBrowserDialog FLD;
    private System.Windows.Forms.NumericUpDown numTotalAC;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown numVFR;
    private System.Windows.Forms.CheckBox cbxLogging;
    private System.Windows.Forms.Button btSimVFR;
    private System.Windows.Forms.OpenFileDialog OFD;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.Label lblPing;
    private System.Windows.Forms.TextBox txRwy;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Button btSimIFR;
    private System.Windows.Forms.Button btDumpIFR;
  }
}


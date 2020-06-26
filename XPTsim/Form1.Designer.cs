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
      this.SuspendLayout();
      // 
      // txBasePath
      // 
      this.txBasePath.Location = new System.Drawing.Point(15, 88);
      this.txBasePath.Name = "txBasePath";
      this.txBasePath.Size = new System.Drawing.Size(310, 22);
      this.txBasePath.TabIndex = 0;
      this.txBasePath.Text = "E:\\G\\Steam\\SteamApps\\common\\X-Plane 11";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 72);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(89, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "XP 11 Base Path:";
      // 
      // btBasePath
      // 
      this.btBasePath.Location = new System.Drawing.Point(331, 89);
      this.btBasePath.Name = "btBasePath";
      this.btBasePath.Size = new System.Drawing.Size(43, 21);
      this.btBasePath.TabIndex = 2;
      this.btBasePath.Text = "...";
      this.btBasePath.UseVisualStyleBackColor = true;
      this.btBasePath.Click += new System.EventHandler(this.btBasePath_Click);
      // 
      // btCreateLink
      // 
      this.btCreateLink.Location = new System.Drawing.Point(16, 116);
      this.btCreateLink.Name = "btCreateLink";
      this.btCreateLink.Size = new System.Drawing.Size(86, 39);
      this.btCreateLink.TabIndex = 3;
      this.btCreateLink.Text = "Establish Link";
      this.btCreateLink.UseVisualStyleBackColor = true;
      this.btCreateLink.Click += new System.EventHandler(this.btCreateLink_Click);
      // 
      // btDropLink
      // 
      this.btDropLink.Location = new System.Drawing.Point(288, 116);
      this.btDropLink.Name = "btDropLink";
      this.btDropLink.Size = new System.Drawing.Size(86, 39);
      this.btDropLink.TabIndex = 3;
      this.btDropLink.Text = "Drop Link";
      this.btDropLink.UseVisualStyleBackColor = true;
      this.btDropLink.Click += new System.EventHandler(this.btDropLink_Click);
      // 
      // btCreateDB
      // 
      this.btCreateDB.Location = new System.Drawing.Point(15, 242);
      this.btCreateDB.Name = "btCreateDB";
      this.btCreateDB.Size = new System.Drawing.Size(86, 39);
      this.btCreateDB.TabIndex = 4;
      this.btCreateDB.Text = "Create DB";
      this.btCreateDB.UseVisualStyleBackColor = true;
      this.btCreateDB.Click += new System.EventHandler(this.btCreateDB_Click);
      // 
      // lblCreate
      // 
      this.lblCreate.AutoSize = true;
      this.lblCreate.Location = new System.Drawing.Point(14, 284);
      this.lblCreate.Name = "lblCreate";
      this.lblCreate.Size = new System.Drawing.Size(16, 13);
      this.lblCreate.TabIndex = 5;
      this.lblCreate.Text = "...";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(13, 12);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(115, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "LifeTraffic IP Address:";
      // 
      // txIP
      // 
      this.txIP.Location = new System.Drawing.Point(15, 29);
      this.txIP.Name = "txIP";
      this.txIP.Size = new System.Drawing.Size(164, 22);
      this.txIP.TabIndex = 6;
      // 
      // lblLink
      // 
      this.lblLink.AutoEllipsis = true;
      this.lblLink.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblLink.Location = new System.Drawing.Point(16, 158);
      this.lblLink.Name = "lblLink";
      this.lblLink.Size = new System.Drawing.Size(358, 71);
      this.lblLink.TabIndex = 5;
      this.lblLink.Text = "...";
      // 
      // FLD
      // 
      this.FLD.ShowNewFolderButton = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(439, 312);
      this.Controls.Add(this.txIP);
      this.Controls.Add(this.lblLink);
      this.Controls.Add(this.lblCreate);
      this.Controls.Add(this.btCreateDB);
      this.Controls.Add(this.btDropLink);
      this.Controls.Add(this.btCreateLink);
      this.Controls.Add(this.btBasePath);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txBasePath);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "Form1";
      this.Text = "Form1";
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
  }
}


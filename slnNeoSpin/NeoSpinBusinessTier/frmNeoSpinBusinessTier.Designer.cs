namespace NeoSpin.BusinessTier
{
    partial class frmNeoSpinBusinessTier
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbcSagitecServer = new System.Windows.Forms.TabControl();
            this.tbpMetaData = new System.Windows.Forms.TabPage();
            this.stsDetails = new System.Windows.Forms.StatusStrip();
            this.lblStatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.lsvMetaCacheInfo = new System.Windows.Forms.ListView();
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmReplaced = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDirectory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblMDC = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRefreshMetaData = new System.Windows.Forms.Button();
            this.tbpDbCache = new System.Windows.Forms.TabPage();
            this.lsvDbCache = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmRowCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmQuery = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblDbCacheMessage = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRefreshDB = new System.Windows.Forms.Button();
            this.tbpBusinessTier = new System.Windows.Forms.TabPage();
            this.txbError = new System.Windows.Forms.TextBox();
            this.lblErrorDetails = new System.Windows.Forms.Label();
            this.lblBTMessage = new System.Windows.Forms.Label();
            this.lblMessageCaption = new System.Windows.Forms.Label();
            this.bgwAuditLog = new System.ComponentModel.BackgroundWorker();
            this.btnRefreshRules = new System.Windows.Forms.Button();
            this.tbcSagitecServer.SuspendLayout();
            this.tbpMetaData.SuspendLayout();
            this.stsDetails.SuspendLayout();
            this.tbpDbCache.SuspendLayout();
            this.tbpBusinessTier.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcSagitecServer
            // 
            this.tbcSagitecServer.Controls.Add(this.tbpMetaData);
            this.tbcSagitecServer.Controls.Add(this.tbpDbCache);
            this.tbcSagitecServer.Controls.Add(this.tbpBusinessTier);
            this.tbcSagitecServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcSagitecServer.Location = new System.Drawing.Point(0, 0);
            this.tbcSagitecServer.Name = "tbcSagitecServer";
            this.tbcSagitecServer.SelectedIndex = 0;
            this.tbcSagitecServer.Size = new System.Drawing.Size(1018, 423);
            this.tbcSagitecServer.TabIndex = 7;
            // 
            // tbpMetaData
            // 
            this.tbpMetaData.Controls.Add(this.stsDetails);
            this.tbpMetaData.Controls.Add(this.lsvMetaCacheInfo);
            this.tbpMetaData.Controls.Add(this.lblMDC);
            this.tbpMetaData.Controls.Add(this.label2);
            this.tbpMetaData.Controls.Add(this.btnRefreshMetaData);
            this.tbpMetaData.Location = new System.Drawing.Point(4, 22);
            this.tbpMetaData.Name = "tbpMetaData";
            this.tbpMetaData.Padding = new System.Windows.Forms.Padding(3);
            this.tbpMetaData.Size = new System.Drawing.Size(1010, 397);
            this.tbpMetaData.TabIndex = 0;
            this.tbpMetaData.Text = "MetaData";
            this.tbpMetaData.UseVisualStyleBackColor = true;
            // 
            // stsDetails
            // 
            this.stsDetails.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusBar});
            this.stsDetails.Location = new System.Drawing.Point(3, 372);
            this.stsDetails.Name = "stsDetails";
            this.stsDetails.Size = new System.Drawing.Size(1004, 22);
            this.stsDetails.TabIndex = 15;
            this.stsDetails.Text = "statusStrip1";
            // 
            // lblStatusBar
            // 
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Size = new System.Drawing.Size(118, 17);
            this.lblStatusBar.Text = "toolStripStatusLabel1";
            // 
            // lsvMetaCacheInfo
            // 
            this.lsvMetaCacheInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvMetaCacheInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmReplaced,
            this.clmType,
            this.clmDirectory});
            this.lsvMetaCacheInfo.Location = new System.Drawing.Point(11, 73);
            this.lsvMetaCacheInfo.Name = "lsvMetaCacheInfo";
            this.lsvMetaCacheInfo.Size = new System.Drawing.Size(991, 316);
            this.lsvMetaCacheInfo.TabIndex = 14;
            this.lsvMetaCacheInfo.UseCompatibleStateImageBehavior = false;
            this.lsvMetaCacheInfo.View = System.Windows.Forms.View.Details;
            this.lsvMetaCacheInfo.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lsvMetaCacheInfo_ColumnClick);
            // 
            // clmName
            // 
            this.clmName.Text = "Name";
            this.clmName.Width = 200;
            // 
            // clmReplaced
            // 
            this.clmReplaced.Text = "Replaced";
            this.clmReplaced.Width = 70;
            // 
            // clmType
            // 
            this.clmType.Text = "Type";
            this.clmType.Width = 100;
            // 
            // clmDirectory
            // 
            this.clmDirectory.Text = "Directory";
            this.clmDirectory.Width = 382;
            // 
            // lblMDC
            // 
            this.lblMDC.AutoSize = true;
            this.lblMDC.Location = new System.Drawing.Point(73, 10);
            this.lblMDC.Name = "lblMDC";
            this.lblMDC.Size = new System.Drawing.Size(33, 13);
            this.lblMDC.TabIndex = 12;
            this.lblMDC.Text = "None";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Message : ";
            // 
            // btnRefreshMetaData
            // 
            this.btnRefreshMetaData.Location = new System.Drawing.Point(11, 35);
            this.btnRefreshMetaData.Name = "btnRefreshMetaData";
            this.btnRefreshMetaData.Size = new System.Drawing.Size(127, 28);
            this.btnRefreshMetaData.TabIndex = 10;
            this.btnRefreshMetaData.Text = "Refresh MetaData";
            this.btnRefreshMetaData.Click += new System.EventHandler(this.btnRefreshMetaData_Click);
            // 
            // tbpDbCache
            // 
            this.tbpDbCache.Controls.Add(this.lsvDbCache);
            this.tbpDbCache.Controls.Add(this.lblDbCacheMessage);
            this.tbpDbCache.Controls.Add(this.label4);
            this.tbpDbCache.Controls.Add(this.btnRefreshDB);
            this.tbpDbCache.Location = new System.Drawing.Point(4, 22);
            this.tbpDbCache.Name = "tbpDbCache";
            this.tbpDbCache.Padding = new System.Windows.Forms.Padding(3);
            this.tbpDbCache.Size = new System.Drawing.Size(1010, 397);
            this.tbpDbCache.TabIndex = 1;
            this.tbpDbCache.Text = "DB Cache";
            this.tbpDbCache.UseVisualStyleBackColor = true;
            // 
            // lsvDbCache
            // 
            this.lsvDbCache.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvDbCache.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.clmRowCount,
            this.clmQuery});
            this.lsvDbCache.FullRowSelect = true;
            this.lsvDbCache.Location = new System.Drawing.Point(11, 68);
            this.lsvDbCache.Name = "lsvDbCache";
            this.lsvDbCache.Size = new System.Drawing.Size(1024, 420);
            this.lsvDbCache.TabIndex = 14;
            this.lsvDbCache.UseCompatibleStateImageBehavior = false;
            this.lsvDbCache.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 123;
            // 
            // clmRowCount
            // 
            this.clmRowCount.Text = "No. Of Rows";
            this.clmRowCount.Width = 80;
            // 
            // clmQuery
            // 
            this.clmQuery.Text = "Query Details";
            this.clmQuery.Width = 836;
            // 
            // lblDbCacheMessage
            // 
            this.lblDbCacheMessage.AutoSize = true;
            this.lblDbCacheMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDbCacheMessage.Location = new System.Drawing.Point(73, 12);
            this.lblDbCacheMessage.Name = "lblDbCacheMessage";
            this.lblDbCacheMessage.Size = new System.Drawing.Size(10, 13);
            this.lblDbCacheMessage.TabIndex = 12;
            this.lblDbCacheMessage.Text = "l";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Message : ";
            // 
            // btnRefreshDB
            // 
            this.btnRefreshDB.Location = new System.Drawing.Point(11, 28);
            this.btnRefreshDB.Name = "btnRefreshDB";
            this.btnRefreshDB.Size = new System.Drawing.Size(151, 34);
            this.btnRefreshDB.TabIndex = 10;
            this.btnRefreshDB.Text = "Refresh Database Cache";
            this.btnRefreshDB.Click += new System.EventHandler(this.btnRefreshDB_Click);
            // 
            // tbpBusinessTier
            // 
            this.tbpBusinessTier.Controls.Add(this.btnRefreshRules);
            this.tbpBusinessTier.Controls.Add(this.txbError);
            this.tbpBusinessTier.Controls.Add(this.lblErrorDetails);
            this.tbpBusinessTier.Controls.Add(this.lblBTMessage);
            this.tbpBusinessTier.Controls.Add(this.lblMessageCaption);
            this.tbpBusinessTier.Location = new System.Drawing.Point(4, 22);
            this.tbpBusinessTier.Name = "tbpBusinessTier";
            this.tbpBusinessTier.Padding = new System.Windows.Forms.Padding(3);
            this.tbpBusinessTier.Size = new System.Drawing.Size(1010, 397);
            this.tbpBusinessTier.TabIndex = 2;
            this.tbpBusinessTier.Text = "Business Tier";
            this.tbpBusinessTier.UseVisualStyleBackColor = true;
            // 
            // txbError
            // 
            this.txbError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbError.Location = new System.Drawing.Point(8, 82);
            this.txbError.Multiline = true;
            this.txbError.Name = "txbError";
            this.txbError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbError.Size = new System.Drawing.Size(1032, 421);
            this.txbError.TabIndex = 15;
            // 
            // lblErrorDetails
            // 
            this.lblErrorDetails.AutoSize = true;
            this.lblErrorDetails.Location = new System.Drawing.Point(17, 45);
            this.lblErrorDetails.Name = "lblErrorDetails";
            this.lblErrorDetails.Size = new System.Drawing.Size(70, 13);
            this.lblErrorDetails.TabIndex = 14;
            this.lblErrorDetails.Text = "Error Details :";
            // 
            // lblBTMessage
            // 
            this.lblBTMessage.AutoSize = true;
            this.lblBTMessage.Location = new System.Drawing.Point(97, 18);
            this.lblBTMessage.Name = "lblBTMessage";
            this.lblBTMessage.Size = new System.Drawing.Size(33, 13);
            this.lblBTMessage.TabIndex = 12;
            this.lblBTMessage.Text = "None";
            // 
            // lblMessageCaption
            // 
            this.lblMessageCaption.AutoSize = true;
            this.lblMessageCaption.Location = new System.Drawing.Point(17, 18);
            this.lblMessageCaption.Name = "lblMessageCaption";
            this.lblMessageCaption.Size = new System.Drawing.Size(59, 13);
            this.lblMessageCaption.TabIndex = 11;
            this.lblMessageCaption.Text = "Message : ";
            // 
            // bgwAuditLog
            // 
            this.bgwAuditLog.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwAuditLog_DoWork);
            // 
            // btnRefreshRules
            // 
            this.btnRefreshRules.Location = new System.Drawing.Point(874, 0);
            this.btnRefreshRules.Name = "btnRefreshRules";
            this.btnRefreshRules.Size = new System.Drawing.Size(136, 40);
            this.btnRefreshRules.TabIndex = 20;
            this.btnRefreshRules.Text = "Refresh Rules";
            this.btnRefreshRules.UseVisualStyleBackColor = true;
            this.btnRefreshRules.Click += new System.EventHandler(this.btnRefreshRules_Click);
            // 
            // frmNeoSpinBusinessTier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 423);
            this.Controls.Add(this.tbcSagitecServer);
            this.Name = "frmNeoSpinBusinessTier";
            this.Text = "frmNeoSpinBusinessTier";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmVNAVBusinessTier_FormClosing);
            this.Load += new System.EventHandler(this.frmNeoSpinBusinessTier_Load);
            this.Shown += new System.EventHandler(this.frmNeoSpinBusinessTier_Shown);
            this.tbcSagitecServer.ResumeLayout(false);
            this.tbpMetaData.ResumeLayout(false);
            this.tbpMetaData.PerformLayout();
            this.stsDetails.ResumeLayout(false);
            this.stsDetails.PerformLayout();
            this.tbpDbCache.ResumeLayout(false);
            this.tbpDbCache.PerformLayout();
            this.tbpBusinessTier.ResumeLayout(false);
            this.tbpBusinessTier.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcSagitecServer;
        private System.Windows.Forms.TabPage tbpMetaData;
        private System.Windows.Forms.StatusStrip stsDetails;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusBar;
        private System.Windows.Forms.ListView lsvMetaCacheInfo;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.ColumnHeader clmReplaced;
        private System.Windows.Forms.ColumnHeader clmType;
        private System.Windows.Forms.ColumnHeader clmDirectory;
        private System.Windows.Forms.Label lblMDC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRefreshMetaData;
        private System.Windows.Forms.TabPage tbpDbCache;
        private System.Windows.Forms.ListView lsvDbCache;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader clmRowCount;
        private System.Windows.Forms.ColumnHeader clmQuery;
        private System.Windows.Forms.Label lblDbCacheMessage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRefreshDB;
        private System.Windows.Forms.TabPage tbpBusinessTier;
        private System.Windows.Forms.TextBox txbError;
        private System.Windows.Forms.Label lblErrorDetails;
        private System.Windows.Forms.Label lblBTMessage;
        private System.Windows.Forms.Label lblMessageCaption;
        private System.ComponentModel.BackgroundWorker bgwAuditLog;
        private System.Windows.Forms.Button btnRefreshRules;
    }
}
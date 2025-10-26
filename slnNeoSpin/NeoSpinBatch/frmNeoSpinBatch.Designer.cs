namespace NeoSpinBatch
{
    partial class frmNeoSpinBatch
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grpRunBatch = new System.Windows.Forms.GroupBox();
            this.btnRunNeoSpinBatch = new System.Windows.Forms.Button();
            this.cmbEndStep = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbStartStep = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpProgressSummary = new System.Windows.Forms.GroupBox();
            this.grvSummary = new System.Windows.Forms.DataGridView();
            this.colSumDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSumStepName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSumType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSumMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpProgressDetail = new System.Windows.Forms.GroupBox();
            this.grvDetail = new System.Windows.Forms.DataGridView();
            this.comDetDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetStepName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsrSummary = new System.Windows.Forms.BindingSource(this.components);
            this.bsrDetail = new System.Windows.Forms.BindingSource(this.components);
            this.TimerProcessMessages = new System.Windows.Forms.Timer(this.components);
            this.grpRunBatch.SuspendLayout();
            this.grpProgressSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grvSummary)).BeginInit();
            this.grpProgressDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grvDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // grpRunBatch
            // 
            this.grpRunBatch.Controls.Add(this.btnRunNeoSpinBatch);
            this.grpRunBatch.Controls.Add(this.cmbEndStep);
            this.grpRunBatch.Controls.Add(this.label2);
            this.grpRunBatch.Controls.Add(this.cmbStartStep);
            this.grpRunBatch.Controls.Add(this.label1);
            this.grpRunBatch.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpRunBatch.Location = new System.Drawing.Point(0, 0);
            this.grpRunBatch.Name = "grpRunBatch";
            this.grpRunBatch.Size = new System.Drawing.Size(913, 55);
            this.grpRunBatch.TabIndex = 0;
            this.grpRunBatch.TabStop = false;
            this.grpRunBatch.Text = "Run NeoSpin Batch";
            // 
            // btnRunNeoSpinBatch
            // 
            this.btnRunNeoSpinBatch.Location = new System.Drawing.Point(768, 24);
            this.btnRunNeoSpinBatch.Name = "btnRunNeoSpinBatch";
            this.btnRunNeoSpinBatch.Size = new System.Drawing.Size(112, 23);
            this.btnRunNeoSpinBatch.TabIndex = 4;
            this.btnRunNeoSpinBatch.Text = "Run NeoSpin Batch";
            this.btnRunNeoSpinBatch.Click += new System.EventHandler(this.btnRunNeoSpinBatch_Click);
            // 
            // cmbEndStep
            // 
            this.cmbEndStep.FormattingEnabled = true;
            this.cmbEndStep.Location = new System.Drawing.Point(431, 19);
            this.cmbEndStep.Name = "cmbEndStep";
            this.cmbEndStep.Size = new System.Drawing.Size(237, 21);
            this.cmbEndStep.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(377, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "End To :";
            // 
            // cmbStartStep
            // 
            this.cmbStartStep.FormattingEnabled = true;
            this.cmbStartStep.Location = new System.Drawing.Point(73, 15);
            this.cmbStartStep.Name = "cmbStartStep";
            this.cmbStartStep.Size = new System.Drawing.Size(290, 21);
            this.cmbStartStep.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start From :";
            // 
            // grpProgressSummary
            // 
            this.grpProgressSummary.Controls.Add(this.grvSummary);
            this.grpProgressSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpProgressSummary.Location = new System.Drawing.Point(0, 55);
            this.grpProgressSummary.Name = "grpProgressSummary";
            this.grpProgressSummary.Size = new System.Drawing.Size(913, 220);
            this.grpProgressSummary.TabIndex = 1;
            this.grpProgressSummary.TabStop = false;
            this.grpProgressSummary.Text = "Progress Summary";
            // 
            // grvSummary
            // 
            this.grvSummary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSumDateTime,
            this.colSumStepName,
            this.colSumType,
            this.colSumMessage});
            this.grvSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grvSummary.Location = new System.Drawing.Point(3, 16);
            this.grvSummary.MultiSelect = false;
            this.grvSummary.Name = "grvSummary";
            this.grvSummary.Size = new System.Drawing.Size(907, 201);
            this.grvSummary.TabIndex = 2;
            this.grvSummary.Text = "dataGridView1";
            // 
            // colSumDateTime
            // 
            this.colSumDateTime.HeaderText = "Date Time";
            this.colSumDateTime.Name = "colSumDateTime";
            this.colSumDateTime.ReadOnly = true;
            this.colSumDateTime.Width = 150;
            // 
            // colSumStepName
            // 
            this.colSumStepName.HeaderText = "Step Name";
            this.colSumStepName.Name = "colSumStepName";
            this.colSumStepName.ReadOnly = true;
            this.colSumStepName.Width = 150;
            // 
            // colSumType
            // 
            this.colSumType.HeaderText = "Type";
            this.colSumType.Name = "colSumType";
            this.colSumType.ReadOnly = true;
            this.colSumType.Width = 150;
            // 
            // colSumMessage
            // 
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colSumMessage.DefaultCellStyle = dataGridViewCellStyle1;
            this.colSumMessage.HeaderText = "Message";
            this.colSumMessage.Name = "colSumMessage";
            this.colSumMessage.ReadOnly = true;
            this.colSumMessage.Width = 400;
            // 
            // grpProgressDetail
            // 
            this.grpProgressDetail.Controls.Add(this.grvDetail);
            this.grpProgressDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProgressDetail.Location = new System.Drawing.Point(0, 275);
            this.grpProgressDetail.Name = "grpProgressDetail";
            this.grpProgressDetail.Size = new System.Drawing.Size(913, 242);
            this.grpProgressDetail.TabIndex = 2;
            this.grpProgressDetail.TabStop = false;
            this.grpProgressDetail.Text = "Progress Detail";
            // 
            // grvDetail
            // 
            this.grvDetail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.comDetDateTime,
            this.colDetStepName,
            this.colDetType,
            this.colDetMessage});
            this.grvDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grvDetail.Location = new System.Drawing.Point(3, 16);
            this.grvDetail.MultiSelect = false;
            this.grvDetail.Name = "grvDetail";
            this.grvDetail.Size = new System.Drawing.Size(907, 223);
            this.grvDetail.TabIndex = 2;
            this.grvDetail.Text = "dataGridView1";
            // 
            // comDetDateTime
            // 
            this.comDetDateTime.HeaderText = "Date Time";
            this.comDetDateTime.Name = "comDetDateTime";
            this.comDetDateTime.ReadOnly = true;
            this.comDetDateTime.Width = 150;
            // 
            // colDetStepName
            // 
            this.colDetStepName.HeaderText = "Step Name";
            this.colDetStepName.Name = "colDetStepName";
            this.colDetStepName.ReadOnly = true;
            this.colDetStepName.Width = 150;
            // 
            // colDetType
            // 
            this.colDetType.HeaderText = "Type";
            this.colDetType.Name = "colDetType";
            this.colDetType.ReadOnly = true;
            this.colDetType.Width = 150;
            // 
            // colDetMessage
            // 
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colDetMessage.DefaultCellStyle = dataGridViewCellStyle2;
            this.colDetMessage.HeaderText = "Message";
            this.colDetMessage.Name = "colDetMessage";
            this.colDetMessage.ReadOnly = true;
            this.colDetMessage.Width = 400;
            // 
            // TimerProcessMessages
            // 
            this.TimerProcessMessages.Tick += new System.EventHandler(this.TimerProcessMessages_Tick);
            // 
            // frmNeoSpinBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 517);
            this.Controls.Add(this.grpProgressDetail);
            this.Controls.Add(this.grpProgressSummary);
            this.Controls.Add(this.grpRunBatch);
            this.Name = "frmNeoSpinBatch";
            this.Text = "frmNeoSpinBatch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmNeoSpinBatch_FormClosing);
            this.Load += new System.EventHandler(this.frmNeoSpinBatch_Load);
            this.grpRunBatch.ResumeLayout(false);
            this.grpRunBatch.PerformLayout();
            this.grpProgressSummary.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grvSummary)).EndInit();
            this.grpProgressDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grvDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrDetail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpRunBatch;
        private System.Windows.Forms.Button btnRunNeoSpinBatch;
        private System.Windows.Forms.ComboBox cmbEndStep;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbStartStep;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpProgressSummary;
        private System.Windows.Forms.GroupBox grpProgressDetail;
        private System.Windows.Forms.BindingSource bsrSummary;
        private System.Windows.Forms.BindingSource bsrDetail;
        private System.Windows.Forms.DataGridView grvSummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSumDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSumStepName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSumType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSumMessage;
        private System.Windows.Forms.DataGridView grvDetail;
        private System.Windows.Forms.DataGridViewTextBoxColumn comDetDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetStepName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetMessage;
        private System.Windows.Forms.Timer TimerProcessMessages;
    }
}
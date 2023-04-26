
namespace prView
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGetPrList = new System.Windows.Forms.Button();
            this.lstPr = new System.Windows.Forms.ListBox();
            this.btnGetPrInfo = new System.Windows.Forms.Button();
            this.txtPrInfo = new System.Windows.Forms.TextBox();
            this.btnPrSearch = new System.Windows.Forms.Button();
            this.txtPrSearch = new System.Windows.Forms.TextBox();
            this.lstMd = new System.Windows.Forms.ListBox();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.btnMdDump = new System.Windows.Forms.Button();
            this.btnSeDebugPrivilege = new System.Windows.Forms.Button();
            this.txtPrContent = new System.Windows.Forms.TextBox();
            this.txtPrAddr = new System.Windows.Forms.TextBox();
            this.btnPrRead = new System.Windows.Forms.Button();
            this.txtPrReadLen = new System.Windows.Forms.TextBox();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.btnGetIcon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGetPrList
            // 
            this.btnGetPrList.Location = new System.Drawing.Point(12, 54);
            this.btnGetPrList.Name = "btnGetPrList";
            this.btnGetPrList.Size = new System.Drawing.Size(597, 36);
            this.btnGetPrList.TabIndex = 0;
            this.btnGetPrList.Text = "GetList";
            this.btnGetPrList.UseVisualStyleBackColor = true;
            this.btnGetPrList.Click += new System.EventHandler(this.btnGetPrList_Click);
            // 
            // lstPr
            // 
            this.lstPr.FormattingEnabled = true;
            this.lstPr.ItemHeight = 18;
            this.lstPr.Location = new System.Drawing.Point(12, 146);
            this.lstPr.Name = "lstPr";
            this.lstPr.Size = new System.Drawing.Size(597, 274);
            this.lstPr.TabIndex = 1;
            // 
            // btnGetPrInfo
            // 
            this.btnGetPrInfo.Location = new System.Drawing.Point(12, 426);
            this.btnGetPrInfo.Name = "btnGetPrInfo";
            this.btnGetPrInfo.Size = new System.Drawing.Size(419, 36);
            this.btnGetPrInfo.TabIndex = 2;
            this.btnGetPrInfo.Text = "GetInfo";
            this.btnGetPrInfo.UseVisualStyleBackColor = true;
            this.btnGetPrInfo.Click += new System.EventHandler(this.btnGetPrInfo_Click);
            // 
            // txtPrInfo
            // 
            this.txtPrInfo.Location = new System.Drawing.Point(12, 468);
            this.txtPrInfo.Multiline = true;
            this.txtPrInfo.Name = "txtPrInfo";
            this.txtPrInfo.ReadOnly = true;
            this.txtPrInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrInfo.Size = new System.Drawing.Size(597, 269);
            this.txtPrInfo.TabIndex = 3;
            // 
            // btnPrSearch
            // 
            this.btnPrSearch.Location = new System.Drawing.Point(12, 96);
            this.btnPrSearch.Name = "btnPrSearch";
            this.btnPrSearch.Size = new System.Drawing.Size(192, 36);
            this.btnPrSearch.TabIndex = 4;
            this.btnPrSearch.Text = "Search";
            this.btnPrSearch.UseVisualStyleBackColor = true;
            this.btnPrSearch.Click += new System.EventHandler(this.btnPrSearch_Click);
            // 
            // txtPrSearch
            // 
            this.txtPrSearch.Location = new System.Drawing.Point(211, 100);
            this.txtPrSearch.Name = "txtPrSearch";
            this.txtPrSearch.Size = new System.Drawing.Size(398, 28);
            this.txtPrSearch.TabIndex = 5;
            // 
            // lstMd
            // 
            this.lstMd.FormattingEnabled = true;
            this.lstMd.ItemHeight = 18;
            this.lstMd.Location = new System.Drawing.Point(12, 743);
            this.lstMd.Name = "lstMd";
            this.lstMd.Size = new System.Drawing.Size(597, 184);
            this.lstMd.TabIndex = 6;
            // 
            // btnMdDump
            // 
            this.btnMdDump.Location = new System.Drawing.Point(12, 933);
            this.btnMdDump.Name = "btnMdDump";
            this.btnMdDump.Size = new System.Drawing.Size(597, 36);
            this.btnMdDump.TabIndex = 7;
            this.btnMdDump.Text = "Dump";
            this.btnMdDump.UseVisualStyleBackColor = true;
            this.btnMdDump.Click += new System.EventHandler(this.btnMdDump_Click);
            // 
            // btnSeDebugPrivilege
            // 
            this.btnSeDebugPrivilege.Location = new System.Drawing.Point(12, 12);
            this.btnSeDebugPrivilege.Name = "btnSeDebugPrivilege";
            this.btnSeDebugPrivilege.Size = new System.Drawing.Size(597, 36);
            this.btnSeDebugPrivilege.TabIndex = 8;
            this.btnSeDebugPrivilege.Text = "Require SeDebugPrivilege";
            this.btnSeDebugPrivilege.UseVisualStyleBackColor = true;
            this.btnSeDebugPrivilege.Click += new System.EventHandler(this.btnSeDebugPrivilege_Click);
            // 
            // txtPrContent
            // 
            this.txtPrContent.Location = new System.Drawing.Point(616, 468);
            this.txtPrContent.Multiline = true;
            this.txtPrContent.Name = "txtPrContent";
            this.txtPrContent.ReadOnly = true;
            this.txtPrContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrContent.Size = new System.Drawing.Size(866, 499);
            this.txtPrContent.TabIndex = 9;
            // 
            // txtPrAddr
            // 
            this.txtPrAddr.Location = new System.Drawing.Point(814, 431);
            this.txtPrAddr.Name = "txtPrAddr";
            this.txtPrAddr.Size = new System.Drawing.Size(348, 28);
            this.txtPrAddr.TabIndex = 11;
            // 
            // btnPrRead
            // 
            this.btnPrRead.Location = new System.Drawing.Point(616, 425);
            this.btnPrRead.Name = "btnPrRead";
            this.btnPrRead.Size = new System.Drawing.Size(192, 36);
            this.btnPrRead.TabIndex = 10;
            this.btnPrRead.Text = "Read";
            this.btnPrRead.UseVisualStyleBackColor = true;
            this.btnPrRead.Click += new System.EventHandler(this.btnPrRead_Click);
            // 
            // txtPrReadLen
            // 
            this.txtPrReadLen.Location = new System.Drawing.Point(1168, 431);
            this.txtPrReadLen.Name = "txtPrReadLen";
            this.txtPrReadLen.Size = new System.Drawing.Size(313, 28);
            this.txtPrReadLen.TabIndex = 12;
            this.txtPrReadLen.Text = "1024";
            // 
            // picIcon
            // 
            this.picIcon.Location = new System.Drawing.Point(616, 387);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(32, 32);
            this.picIcon.TabIndex = 13;
            this.picIcon.TabStop = false;
            this.picIcon.DoubleClick += new System.EventHandler(this.picIcon_DoubleClick);
            // 
            // btnGetIcon
            // 
            this.btnGetIcon.Location = new System.Drawing.Point(437, 426);
            this.btnGetIcon.Name = "btnGetIcon";
            this.btnGetIcon.Size = new System.Drawing.Size(172, 36);
            this.btnGetIcon.TabIndex = 14;
            this.btnGetIcon.Text = "GetIcon";
            this.btnGetIcon.UseVisualStyleBackColor = true;
            this.btnGetIcon.Click += new System.EventHandler(this.btnGetIcon_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1493, 979);
            this.Controls.Add(this.btnGetIcon);
            this.Controls.Add(this.picIcon);
            this.Controls.Add(this.txtPrReadLen);
            this.Controls.Add(this.txtPrAddr);
            this.Controls.Add(this.btnPrRead);
            this.Controls.Add(this.txtPrContent);
            this.Controls.Add(this.btnSeDebugPrivilege);
            this.Controls.Add(this.btnMdDump);
            this.Controls.Add(this.lstMd);
            this.Controls.Add(this.txtPrSearch);
            this.Controls.Add(this.btnPrSearch);
            this.Controls.Add(this.txtPrInfo);
            this.Controls.Add(this.btnGetPrInfo);
            this.Controls.Add(this.lstPr);
            this.Controls.Add(this.btnGetPrList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "prView";
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetPrList;
        private System.Windows.Forms.ListBox lstPr;
        private System.Windows.Forms.Button btnGetPrInfo;
        private System.Windows.Forms.TextBox txtPrInfo;
        private System.Windows.Forms.Button btnPrSearch;
        private System.Windows.Forms.TextBox txtPrSearch;
        private System.Windows.Forms.ListBox lstMd;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.Button btnMdDump;
        private System.Windows.Forms.Button btnSeDebugPrivilege;
        private System.Windows.Forms.TextBox txtPrContent;
        private System.Windows.Forms.TextBox txtPrAddr;
        private System.Windows.Forms.Button btnPrRead;
        private System.Windows.Forms.TextBox txtPrReadLen;
        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.Button btnGetIcon;
    }
}


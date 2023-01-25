
namespace ScreenCapture
{
    partial class main
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
            this.components = new System.ComponentModel.Container();
            this.saveDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnCaptureGDI = new System.Windows.Forms.Button();
            this.picScreen = new System.Windows.Forms.PictureBox();
            this.tmCapture = new System.Windows.Forms.Timer(this.components);
            this.btnTimer = new System.Windows.Forms.Button();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.picFilter = new System.Windows.Forms.PictureBox();
            this.btnMatch = new System.Windows.Forms.Button();
            this.btnMatchTemplate = new System.Windows.Forms.Button();
            this.chkActiveWin = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).BeginInit();
            this.SuspendLayout();
            // 
            // saveDialog
            // 
            this.saveDialog.Filter = "PNG|*.png|All files|*.*";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(37, 27);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "SaveTo";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(118, 29);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(475, 28);
            this.txtFilename.TabIndex = 2;
            // 
            // btnCaptureGDI
            // 
            this.btnCaptureGDI.Location = new System.Drawing.Point(37, 86);
            this.btnCaptureGDI.Name = "btnCaptureGDI";
            this.btnCaptureGDI.Size = new System.Drawing.Size(158, 28);
            this.btnCaptureGDI.TabIndex = 3;
            this.btnCaptureGDI.Text = "Capture / GDI";
            this.btnCaptureGDI.UseVisualStyleBackColor = true;
            this.btnCaptureGDI.Click += new System.EventHandler(this.btnCaptureGDI_Click);
            // 
            // picScreen
            // 
            this.picScreen.Location = new System.Drawing.Point(37, 120);
            this.picScreen.Name = "picScreen";
            this.picScreen.Size = new System.Drawing.Size(800, 640);
            this.picScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picScreen.TabIndex = 4;
            this.picScreen.TabStop = false;
            // 
            // tmCapture
            // 
            this.tmCapture.Interval = 200;
            this.tmCapture.Tick += new System.EventHandler(this.tmCapture_Tick);
            // 
            // btnTimer
            // 
            this.btnTimer.Location = new System.Drawing.Point(201, 86);
            this.btnTimer.Name = "btnTimer";
            this.btnTimer.Size = new System.Drawing.Size(158, 28);
            this.btnTimer.TabIndex = 5;
            this.btnTimer.Text = "Start";
            this.btnTimer.UseVisualStyleBackColor = true;
            this.btnTimer.Click += new System.EventHandler(this.btnTimer_Click);
            // 
            // openDialog
            // 
            this.openDialog.FileName = "openFileDialog1";
            this.openDialog.Filter = "All Files|*.*";
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Location = new System.Drawing.Point(869, 31);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(75, 28);
            this.btnLoadImage.TabIndex = 6;
            this.btnLoadImage.Text = "Load";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // picFilter
            // 
            this.picFilter.Location = new System.Drawing.Point(869, 65);
            this.picFilter.Name = "picFilter";
            this.picFilter.Size = new System.Drawing.Size(192, 172);
            this.picFilter.TabIndex = 8;
            this.picFilter.TabStop = false;
            // 
            // btnMatch
            // 
            this.btnMatch.Location = new System.Drawing.Point(869, 243);
            this.btnMatch.Name = "btnMatch";
            this.btnMatch.Size = new System.Drawing.Size(192, 28);
            this.btnMatch.TabIndex = 9;
            this.btnMatch.Text = "Match Descriptor";
            this.btnMatch.UseVisualStyleBackColor = true;
            this.btnMatch.Click += new System.EventHandler(this.btnMatch_Click);
            // 
            // btnMatchTemplate
            // 
            this.btnMatchTemplate.Location = new System.Drawing.Point(869, 277);
            this.btnMatchTemplate.Name = "btnMatchTemplate";
            this.btnMatchTemplate.Size = new System.Drawing.Size(192, 28);
            this.btnMatchTemplate.TabIndex = 10;
            this.btnMatchTemplate.Text = "Match Template";
            this.btnMatchTemplate.UseVisualStyleBackColor = true;
            this.btnMatchTemplate.Click += new System.EventHandler(this.btnMatchTemplate_Click);
            // 
            // chkActiveWin
            // 
            this.chkActiveWin.AutoSize = true;
            this.chkActiveWin.Location = new System.Drawing.Point(398, 91);
            this.chkActiveWin.Name = "chkActiveWin";
            this.chkActiveWin.Size = new System.Drawing.Size(142, 22);
            this.chkActiveWin.TabIndex = 11;
            this.chkActiveWin.Text = "ActiveWindow";
            this.chkActiveWin.UseVisualStyleBackColor = true;
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 793);
            this.Controls.Add(this.chkActiveWin);
            this.Controls.Add(this.btnMatchTemplate);
            this.Controls.Add(this.btnMatch);
            this.Controls.Add(this.picFilter);
            this.Controls.Add(this.btnLoadImage);
            this.Controls.Add(this.btnTimer);
            this.Controls.Add(this.picScreen);
            this.Controls.Add(this.btnCaptureGDI);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "main";
            this.Text = "ScreenCapture";
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFilter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveDialog;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnCaptureGDI;
        private System.Windows.Forms.PictureBox picScreen;
        private System.Windows.Forms.Timer tmCapture;
        private System.Windows.Forms.Button btnTimer;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.PictureBox picFilter;
        private System.Windows.Forms.Button btnMatch;
        private System.Windows.Forms.Button btnMatchTemplate;
        private System.Windows.Forms.CheckBox chkActiveWin;
    }
}


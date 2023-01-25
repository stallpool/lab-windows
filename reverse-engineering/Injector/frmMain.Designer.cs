
namespace Injector
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
            this.lstProcess = new System.Windows.Forms.ListBox();
            this.btnGetProcessList = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dlgOpenDll = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenDll = new System.Windows.Forms.Button();
            this.txtOpenDll = new System.Windows.Forms.TextBox();
            this.btnSafeInject = new System.Windows.Forms.Button();
            this.txtThread = new System.Windows.Forms.TextBox();
            this.btnGetThread = new System.Windows.Forms.Button();
            this.txtThreadInfo = new System.Windows.Forms.TextBox();
            this.btnUnSafeInject = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstProcess
            // 
            this.lstProcess.FormattingEnabled = true;
            this.lstProcess.ItemHeight = 12;
            this.lstProcess.Location = new System.Drawing.Point(12, 70);
            this.lstProcess.Name = "lstProcess";
            this.lstProcess.Size = new System.Drawing.Size(424, 484);
            this.lstProcess.TabIndex = 0;
            // 
            // btnGetProcessList
            // 
            this.btnGetProcessList.Location = new System.Drawing.Point(12, 12);
            this.btnGetProcessList.Name = "btnGetProcessList";
            this.btnGetProcessList.Size = new System.Drawing.Size(161, 23);
            this.btnGetProcessList.TabIndex = 1;
            this.btnGetProcessList.Text = "Get Process List";
            this.btnGetProcessList.UseVisualStyleBackColor = true;
            this.btnGetProcessList.Click += new System.EventHandler(this.btnGetProcessList_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(12, 41);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(161, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search Process";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(179, 43);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(257, 21);
            this.txtSearch.TabIndex = 3;
            // 
            // dlgOpenDll
            // 
            this.dlgOpenDll.FileName = "openFileDialog1";
            this.dlgOpenDll.Filter = "All Files (*.*)|*.*";
            // 
            // btnOpenDll
            // 
            this.btnOpenDll.Location = new System.Drawing.Point(442, 41);
            this.btnOpenDll.Name = "btnOpenDll";
            this.btnOpenDll.Size = new System.Drawing.Size(161, 23);
            this.btnOpenDll.TabIndex = 4;
            this.btnOpenDll.Text = "Open DLL";
            this.btnOpenDll.UseVisualStyleBackColor = true;
            this.btnOpenDll.Click += new System.EventHandler(this.btnOpenDll_Click);
            // 
            // txtOpenDll
            // 
            this.txtOpenDll.Location = new System.Drawing.Point(609, 43);
            this.txtOpenDll.Name = "txtOpenDll";
            this.txtOpenDll.ReadOnly = true;
            this.txtOpenDll.Size = new System.Drawing.Size(257, 21);
            this.txtOpenDll.TabIndex = 5;
            // 
            // btnSafeInject
            // 
            this.btnSafeInject.Location = new System.Drawing.Point(442, 12);
            this.btnSafeInject.Name = "btnSafeInject";
            this.btnSafeInject.Size = new System.Drawing.Size(161, 23);
            this.btnSafeInject.TabIndex = 6;
            this.btnSafeInject.Text = "Safe Inject";
            this.btnSafeInject.UseVisualStyleBackColor = true;
            this.btnSafeInject.Click += new System.EventHandler(this.btnSafeInject_Click);
            // 
            // txtThread
            // 
            this.txtThread.Location = new System.Drawing.Point(609, 72);
            this.txtThread.Name = "txtThread";
            this.txtThread.Size = new System.Drawing.Size(257, 21);
            this.txtThread.TabIndex = 8;
            // 
            // btnGetThread
            // 
            this.btnGetThread.Location = new System.Drawing.Point(442, 70);
            this.btnGetThread.Name = "btnGetThread";
            this.btnGetThread.Size = new System.Drawing.Size(161, 23);
            this.btnGetThread.TabIndex = 7;
            this.btnGetThread.Text = "Search Thread";
            this.btnGetThread.UseVisualStyleBackColor = true;
            this.btnGetThread.Click += new System.EventHandler(this.btnGetThread_Click);
            // 
            // txtThreadInfo
            // 
            this.txtThreadInfo.Location = new System.Drawing.Point(609, 99);
            this.txtThreadInfo.Name = "txtThreadInfo";
            this.txtThreadInfo.ReadOnly = true;
            this.txtThreadInfo.Size = new System.Drawing.Size(257, 21);
            this.txtThreadInfo.TabIndex = 9;
            // 
            // btnUnSafeInject
            // 
            this.btnUnSafeInject.Location = new System.Drawing.Point(609, 12);
            this.btnUnSafeInject.Name = "btnUnSafeInject";
            this.btnUnSafeInject.Size = new System.Drawing.Size(161, 23);
            this.btnUnSafeInject.TabIndex = 10;
            this.btnUnSafeInject.Text = "UnSafe Inject";
            this.btnUnSafeInject.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 572);
            this.Controls.Add(this.btnUnSafeInject);
            this.Controls.Add(this.txtThreadInfo);
            this.Controls.Add(this.txtThread);
            this.Controls.Add(this.btnGetThread);
            this.Controls.Add(this.btnSafeInject);
            this.Controls.Add(this.txtOpenDll);
            this.Controls.Add(this.btnOpenDll);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnGetProcessList);
            this.Controls.Add(this.lstProcess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "InjectorTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstProcess;
        private System.Windows.Forms.Button btnGetProcessList;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.OpenFileDialog dlgOpenDll;
        private System.Windows.Forms.Button btnOpenDll;
        private System.Windows.Forms.TextBox txtOpenDll;
        private System.Windows.Forms.Button btnSafeInject;
        private System.Windows.Forms.TextBox txtThread;
        private System.Windows.Forms.Button btnGetThread;
        private System.Windows.Forms.TextBox txtThreadInfo;
        private System.Windows.Forms.Button btnUnSafeInject;
    }
}


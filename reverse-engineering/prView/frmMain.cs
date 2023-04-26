using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace prView
{

    public partial class frmMain : Form
    {
        class ProcessItem
        {
            public uint Pid { get; set; }
            public string Name { get; set; }
            public Process Ref { get; set; }

            public ProcessItem(uint pid, string name)
            {
                Pid = pid;
                Name = name;
            }

            public override string ToString()
            {
                return string.Format("{0,8} :: {1}", Pid, Name);
            }
        }

        ProcessThread getMainThread(Process p)
        {
            ProcessThread main = null;
            try
            {
                foreach (ProcessThread th in p.Threads)
                {
                    if (main == null)
                    {
                        main = th;
                    }
                    else if (main.StartTime > th.StartTime)
                    {
                        main = th;
                    }
                }
            }
            catch
            {
                return null;
            }
            return main;
        }

        int thisBitness = 0;
        ProcessItem thisSelectedProcess = null;
        public frmMain()
        {
            InitializeComponent();
            string selfBitness = Environment.Is64BitProcess ? "x64" : "x86";
            thisBitness = Environment.Is64BitProcess ? 64 : 32;
            this.Text += " - " + selfBitness;
        }

        private void btnGetPrList_Click(object sender, EventArgs e)
        {
            lstPr.Items.Clear();
            Process[] processCollection = Process.GetProcesses();
            Array.Sort(processCollection, (Process x, Process y) => x.Id - y.Id);
            foreach (Process p in processCollection)
            {
                ProcessItem item = new ProcessItem((uint)p.Id, p.ProcessName);
                item.Ref = p;
                lstPr.Items.Add(item);
            }
        }

        private void btnPrSearch_Click(object sender, EventArgs e)
        {
            if (txtPrSearch.Text == "") return;
            string txt = txtPrSearch.Text;
            var i = lstPr.SelectedIndex + 1;
            var n = lstPr.Items.Count;
            if (n == 0) return;
            var j = i - 1;
            if (i >= n) i = 0;
            while (j != i)
            {
                ProcessItem item = (ProcessItem)lstPr.Items[i];
                if (item.Name.Contains(txt))
                {
                    lstPr.SelectedIndex = i;
                    return;
                }
                i++;
                if (i == n && j < 0) break;
                i %= n;
            }
        }

        private void btnGetPrInfo_Click(object sender, EventArgs e)
        {
            if (lstPr.SelectedItem == null) return;
            lstMd.Items.Clear();
            txtPrInfo.Text = "";
            ProcessItem item = (ProcessItem)lstPr.SelectedItem;
            thisSelectedProcess = item;
            Process p = item.Ref;
            txtPrInfo.Text += string.Format("name: {0}\r\n", item.Name);
            txtPrInfo.Text += string.Format(" pid: {0}\r\n", item.Pid);
            int bitness = NativeMethods.Is64Bit(p);
            switch (bitness)
            {
                case 32:
                    txtPrInfo.Text += " bit: x86\r\n";
                    break;
                case 64:
                    txtPrInfo.Text += " bit: x64\r\n";
                    break;
                case 0:
                default:
                    txtPrInfo.Text += $" bit: unknown {bitness}\r\n";
                    break;
            }
            string pfilename = NativeMethods.GetProcessFileName(p);
            txtPrInfo.Text += string.Format("file: {0}\r\n", pfilename);
            txtPrInfo.Text += string.Format("args: {0}\r\n", DotNetMethods.GetCommandLine(p));
            ProcessThread main = getMainThread(p);
            txtPrInfo.Text += string.Format(" tid: {0}\r\n", main == null ? -1 : main.Id);

            txtPrInfo.Text += $" peb: 0x{NativeMethods.GetPEBAddress(p).ToInt64():x}\r\n";

            if (thisBitness == bitness)
            {
                txtPrInfo.Text += "\r\nModules:\r\n";
                var mods = NativeMethods.GetModules(p);
                mods.Sort();
                foreach (NativeMethods.ProcessModule_ pm in mods)
                {
                    lstMd.Items.Add(pm);
                    txtPrInfo.Text += string.Format(
                        "+ {0}\r\n|- {1} (0x{1:x}) / {2} (0x{2:x})\r\n|- {3}\r\n",
                        pm.Name, pm.BaseAddr.ToInt64(), pm.Size, pm.FileName
                    );
                }

                /*
                txtPrInfo.Text += "\r\n";
                var mods2 = NativeMethods.GetModules2(p);
                foreach (NativeMethods.ProcessModule_ pm in mods2)
                {
                    txtPrInfo.Text += string.Format(
                        "+ {0}\r\n|- {1} (0x{1:x}) / {2} (0x{2:x})\r\n|- {3}\r\n",
                        pm.Name, pm.BaseAddr.ToInt64(), pm.Size, pm.FileName
                    );
                }

                txtPrInfo.Text += "\r\n";
                var mods2_2 = NativeMethods.GetModule2_2(p);
                foreach (NativeMethods.ProcessModule_ pm in mods2_2)
                {
                    txtPrInfo.Text += string.Format(
                        "+ {0}\r\n|- {1} (0x{1:x}) / {2} (0x{2:x})\r\n|- {3}\r\n",
                        pm.Name, pm.BaseAddr.ToInt64(), pm.Size, pm.FileName
                    );
                }
                */

                txtPrInfo.Text += "\r\nThreads:\r\n";
                var ths = NativeMethods.GetThreads(p);
                ths.Sort();
                foreach(NativeMethods.ProcessThread_ th in ths)
                {
                    txtPrInfo.Text += string.Format("+ {0}\r\n", th.ToString());
                }
            }
            else
            {
                lstMd.Items.Add("(different arch)");
            }
        }

        private void btnMdDump_Click(object sender, EventArgs e)
        {
            if (lstMd.SelectedItem == null) return;
            if (thisSelectedProcess == null) return;
            if (thisSelectedProcess.Ref.HasExited) return;
            dlgSave.Filter = "Dump (*.bin)|*.bin|All Files (*.*)|*.*";
            if (dlgSave.ShowDialog() != DialogResult.OK) return;
            try
            {
                NativeMethods.ProcessModule_ pm = (NativeMethods.ProcessModule_)lstMd.SelectedItem;
                byte[] buf = NativeMethods.ReadMemory(thisSelectedProcess.Ref, pm.BaseAddr, pm.Size);
                if (buf == null) return;
                using (var fs = new System.IO.FileStream(dlgSave.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                }
            }
            catch
            {
                return;
            }
        }

        private void btnSeDebugPrivilege_Click(object sender, EventArgs e)
        {
            bool ret = NativeMethods.SetProcessTokenPrivilege(Process.GetCurrentProcess(), "SeDebugPrivilege", 0x00000002 /* SE_PRIVILEGE_ENABLED */);
            btnSeDebugPrivilege.Enabled = !ret;
            if (ret)
            {
                this.Text += " (SeDebugPrivilege)";
            }
        }

        private void btnPrRead_Click(object sender, EventArgs e)
        {
            if (lstPr.SelectedItem == null) return;
            if (txtPrAddr.Text == "") return;

            IntPtr baseAddr = IntPtr.Zero;
            if (txtPrAddr.Text.StartsWith("0x"))
            {
                baseAddr = new IntPtr(Convert.ToInt64(txtPrAddr.Text.Substring(2), 16));
            }
            else
            {
                baseAddr = new IntPtr(Convert.ToInt64(txtPrAddr.Text.Substring(2), 10));
            }
            uint size = Convert.ToUInt32(txtPrReadLen.Text);
            if (size == 0) size = 1024;

            Process p = thisSelectedProcess.Ref;
            System.Collections.Generic.List<string> debug = new System.Collections.Generic.List<string>();
            byte[] buf = NativeMethods.ReadMemory(p, baseAddr, 1024, debug);

            if (buf == null)
            {
                txtPrContent.Text = "";
                foreach(string line_ in debug)
                {
                    txtPrContent.Text += line_;
                    txtPrContent.Text += "\r\n";
                }
                return;
            }

            txtPrContent.Text = "             00 01 02 03 04 05 06 07 08 09 0a 0b 0c 0d 0e 0f";
            int n = buf.Length;
            long baseAddrInt = baseAddr.ToInt64();
            string line = "";
            for (int i = 0; i < n; i++)
            {
                if (i % 16 == 0)
                {
                    txtPrContent.Text += $"{line}\r\n";
                    txtPrContent.Text += string.Format("{0:x12} ", baseAddrInt + i);
                    line = " ";
                }
                byte ch = buf[i];
                txtPrContent.Text += string.Format("{0:x02} ", ch);
                if (ch >= 32 && ch < 128)
                {
                    line += $"{(char)ch}";
                }
                else
                {
                    line += ".";
                }
            }
        }

        private void btnGetIcon_Click(object sender, EventArgs e)
        {
            if (lstPr.SelectedItem == null) return;
            lstMd.Items.Clear();
            txtPrInfo.Text = "";
            ProcessItem item = (ProcessItem)lstPr.SelectedItem;
            Process p = item.Ref;
            string pfilename = NativeMethods.GetProcessFileName(p);
            System.Drawing.Icon picon = System.Drawing.Icon.ExtractAssociatedIcon(pfilename);
            if (picon != null)
            {
                if (picIcon.Image != null) picIcon.Image.Dispose();
                System.Drawing.Bitmap ico = picon.ToBitmap();
                int icobottom = picIcon.Top + picIcon.Width;
                picIcon.Width = ico.Width;
                picIcon.Height = ico.Height;
                picIcon.Top = icobottom - ico.Height;
                picIcon.Image = ico;
                picon.Dispose();
            }
        }

        private void picIcon_DoubleClick(object sender, EventArgs e)
        {
            if (lstPr.SelectedItem == null) return;
            lstMd.Items.Clear();
            txtPrInfo.Text = "";
            ProcessItem item = (ProcessItem)lstPr.SelectedItem;
            Process p = item.Ref;
            string pfilename = NativeMethods.GetProcessFileName(p);
            System.Drawing.Icon picon = System.Drawing.Icon.ExtractAssociatedIcon(pfilename);
            using (System.IO.FileStream s = System.IO.File.Create("D:\\icon.ico"))
            {
                picon.Save(s);
            }
        }
    }
}

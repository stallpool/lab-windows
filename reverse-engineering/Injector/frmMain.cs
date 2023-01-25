using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Injector
{
    public partial class frmMain : Form
    {
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

        class ProcessItem
        {
            public uint Pid { get; set; }
            public string Name { get; set; }
            public uint Tid  { get; set; }

            public ProcessItem(uint pid, string name, uint tid)
            {
                Pid = pid;
                Name = name;
                Tid = tid;
            }

            public override string ToString()
            {
                return string.Format("{0,8} ::{2,8} {1}", Pid, Name, Tid);
            }
        }

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnGetProcessList_Click(object sender, EventArgs e)
        {
            lstProcess.Items.Clear();
            Process[] processCollection = Process.GetProcesses();
            foreach (Process p in processCollection)
            {
                ProcessThread main = getMainThread(p);
                ProcessItem item = new ProcessItem((uint)p.Id, p.ProcessName, main == null ? 0 : (uint)main.Id);
                lstProcess.Items.Add(item);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "") return;
            var i = lstProcess.SelectedIndex + 1;
            var n = lstProcess.Items.Count;
            if (n == 0) return;
            var j = i - 1;
            if (i >= n) i = 0;
            while (j != i)
            {
                ProcessItem item = (ProcessItem)lstProcess.Items[i];
                if (item.Name.Contains(txtSearch.Text))
                {
                    lstProcess.SelectedIndex = i;
                    return;
                }
                i++;
                if (i == n && j < 0) break;
                i %= n;
            }
        }

        private void btnOpenDll_Click(object sender, EventArgs e)
        {
            if (dlgOpenDll.ShowDialog() == DialogResult.OK)
            {
                txtOpenDll.Text = dlgOpenDll.FileName;
            }
        }

        private void btnSafeInject_Click(object sender, EventArgs e)
        {
            if (txtOpenDll.Text == "") return;
            if (lstProcess.SelectedIndex < 0 && txtThreadInfo.Text == "") return;
            ProcessItem selected = (ProcessItem)lstProcess.SelectedItem;
            if (selected != null && selected.Tid < 0) return;
            uint tid = selected == null ? 0 : selected.Tid;
            if (txtThreadInfo.Text != "")
            {
                tid = uint.Parse(txtThreadInfo.Text.Split(':')[2]);
            }
            IntPtr dll = NativeMethods.LoadLibraryEx(txtOpenDll.Text, IntPtr.Zero, 1 /*DONT_RESOLVE_DLL_REFERENCES*/);
            if (dll == IntPtr.Zero)
            {
                MessageBox.Show("LoadLibrary failed");
                return;
            }
            IntPtr msg_hook_proc_ov = NativeMethods.GetProcAddress(dll, "_msg_hook_proc_ov@12");
            if (msg_hook_proc_ov == IntPtr.Zero)
            {
                NativeMethods.FreeLibrary(dll);
                MessageBox.Show("GetProcAddress failed");
                return;
            }
            IntPtr hook = NativeMethods.SetWindowsHookEx(3 /*WH_GETMESSAGE*/, msg_hook_proc_ov, dll, tid);
            if (hook == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                NativeMethods.FreeLibrary(dll);
                MessageBox.Show("SetWindowsHookEx failed: " + error);
                return;
            }
            bool r = false;
            for (int i = 0; !r && i < 5; i++)
            {
                r = NativeMethods.PostThreadMessage(tid, 0x4000 /* WM_USER */ + 432, IntPtr.Zero, hook);
            }
            NativeMethods.FreeLibrary(dll);
            if (!r)
            {
                MessageBox.Show("Hook/PostThreadMessage failed");
                return;
            }
            /*
            NativeMethods.UnhookWindowsHookEx(hook);
            r = false;
            for (int i = 0; !r && i < 5; i++)
            {
                r = NativeMethods.PostThreadMessage(tid, 0 /* WM_NULL /, IntPtr.Zero, IntPtr.Zero);
            }
            if (!r)
            {
                MessageBox.Show("Unhook/PostThreadMessage failed");
            }
            */
            MessageBox.Show("OK");
        }

        private void btnGetThread_Click(object sender, EventArgs e)
        {
            if (txtThread.Text == "")
            {
                txtThreadInfo.Text = "";
                return;
            }
            IntPtr hwn = NativeMethods.FindWindow(0, txtThread.Text);
            uint pid = 0;
            uint tid = NativeMethods.GetWindowThreadProcessId(hwn, out pid);
            txtThreadInfo.Text = string.Format("{0} ::{1}", pid, tid);
        }
    }
    static class NativeMethods
    {
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern System.IntPtr FindWindow(int ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookEx", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", EntryPoint = "UnhookWindowsHookEx", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll", EntryPoint = "PostThreadMessage", SetLastError = true)]
        public static extern bool PostThreadMessage(uint dwThread, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace prView
{
    public static class NativeMethods
    {
        // see https://msdn.microsoft.com/en-us/library/windows/desktop/ms684139%28v=vs.85%29.aspx
        public static int Is64Bit(Process p)
        {
            if (!Environment.Is64BitOperatingSystem)
                return 32;
            // if this method is not available in your version of .NET, use GetNativeSystemInfo via P/Invoke instead

            bool isWow64;
            try
            {
                if (!IsWow64Process(p.Handle, out isWow64))
                    throw new Win32Exception();
            }
            catch
            {
                return 0;
            }
            return isWow64 ? 32 : 64;
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        public static string GetProcessFileName(Process p)
        {
            try
            {
                int cap = 1024;
                StringBuilder name = new StringBuilder(cap);
                QueryFullProcessImageName(p.Handle, 0, name, ref cap);
                return name.ToString(0, cap);
            }
            catch
            {
                return "(no-access)";
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

        public static bool SetProcessTokenPrivilege(Process p, string privilege, int flags)
        {
            try
            {
                bool retVal;
                TokPriv1Luid tp;
                IntPtr hproc = p.Handle;
                IntPtr htok = IntPtr.Zero;
                retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
                if (!retVal) return false;
                tp.Luid = htok.ToInt64();
                tp.Attr = flags;
                tp.Count = 1;
                retVal = LookupPrivilegeValue(null, privilege, ref tp.Luid);
                if (!retVal) return false;
                retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                if (!retVal) return false;
                return retVal;
            }
            catch
            {
                return false;
            }

        }
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public long Luid;
            public int Attr;
            public int Count;
        }

        public static IntPtr GetPEBAddress(Process p)
        {
            try
            {
                IntPtr hProc = p.Handle;
                long outLong = 0;
                PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                int queryStatus = -1;
                queryStatus = NtQueryInformationProcess(hProc, 0 /* ProcessBasicInformation */, ref pbi, (uint)Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION)), out outLong);
                return pbi.PebBaseAddress;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, uint processInformationLength, out long returnLength);
        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_BASIC_INFORMATION
        {
            internal int ExitStatus;
            internal IntPtr PebBaseAddress;
            internal IntPtr AffinityMask;
            internal uint BasePriority;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
        }

        public class ProcessModule_ : IComparable
        {
            public IntPtr BaseAddr { get; set; }
            public string Name { get; set; }
            public uint Size { get; set; }
            public string FileName { get; set; }
            public ProcessModule_(string name, IntPtr baseAddr, uint size, string fileName)
            {
                Name = name;
                BaseAddr = baseAddr;
                Size = size;
                FileName = fileName;
            }
            public override string ToString()
            {
                return string.Format("{0} ::0x{1:x}|0x{2:x}", Name, BaseAddr.ToInt64(), Size);
            }

            public int CompareTo(object another)
            {
                ProcessModule_ pm = another as ProcessModule_;
                return this.BaseAddr.ToInt64().CompareTo(pm.BaseAddr.ToInt64());
            }
        }

        public static List<ProcessModule_> GetModule2_2(Process p)
        {
            IntPtr hProc = OpenProcess(0x00000010 | 0x00000400, false, p.Id);
            List<ProcessModule_> mods = GetModules2(null, hProc);
            CloseHandle(hProc);
            return mods;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        public static List<ProcessModule_> GetModules2(Process p, IntPtr? hProc = null)
        {
            List<ProcessModule_> mods = new List<ProcessModule_>();
            // Setting up the variable for the second argument for EnumProcessModules
            IntPtr[] hMods = new IntPtr[1024];

            // Setting up the rest of the parameters for EnumProcessModules
            uint uiSize = (uint)(Marshal.SizeOf(typeof(IntPtr)) * (hMods.Length));
            uint cbNeeded = 0;

            IntPtr? hProc_ = hProc;
            if (hProc_ == null || hProc == IntPtr.Zero) hProc_ = p.Handle;
            IntPtr hProc__ = (IntPtr)hProc_;
            if (EnumProcessModules(hProc__, hMods, uiSize, out cbNeeded))
            {
                long uiTotalNumberofModules = cbNeeded / (Marshal.SizeOf(typeof(IntPtr)));

                for (int i = 0; i < (int)uiTotalNumberofModules; i++)
                {
                    StringBuilder strbld = new StringBuilder(1024);

                    GetModuleFileNameEx(hProc__, hMods[i], strbld, (uint)(strbld.Capacity));
                    string fileName = strbld.ToString();
                    string name = System.IO.Path.GetFileName(fileName);
                    ProcessModule_ pm = new ProcessModule_(name, hMods[i], 0, fileName);
                    mods.Add(pm);
                }
            }
            return mods;
        }

        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool EnumProcessModules(IntPtr hProcess, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)][In][Out] IntPtr[] lphModule, uint cb, [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded
        );

        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] uint nSize);

        public static List<ProcessModule_> GetModules(Process p)
        {
            List<ProcessModule_> mods = new List<ProcessModule_>();
            IntPtr hSnap = CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.Module32, p.Id);

            if (hSnap.ToInt64() != INVALID_HANDLE_VALUE)
            {
                MODULEENTRY32 modEntry = new MODULEENTRY32();
                modEntry.dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32));

                if (Module32First(hSnap, ref modEntry))
                {
                    do
                    {
                        ProcessModule_ pm = new ProcessModule_(modEntry.szModule, modEntry.modBaseAddr, modEntry.modBaseSize, modEntry.szExePath);
                        mods.Add(pm);
                    } while (Module32Next(hSnap, ref modEntry));
                }
            }
            CloseHandle(hSnap);

            return mods;
        }

        const Int64 INVALID_HANDLE_VALUE = -1;
        [Flags]

        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        internal struct MODULEENTRY32
        {
            internal uint dwSize;
            internal uint th32ModuleID;
            internal uint th32ProcessID;
            internal uint GlblcntUsage;
            internal uint ProccntUsage;
            internal IntPtr modBaseAddr;
            internal uint modBaseSize;
            internal IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExePath;
        }

        [DllImport("kernel32.dll")]
        private static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        private static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

        public class ProcessThread_ : IComparable
        {
            public ProcessThread Ref { get; set; }
            public List<long> Windows { get; set; }
            public ProcessThread_(ProcessThread th, List<long> win)
            {
                Ref = th;
                Windows = win;
            }
            public override string ToString()
            {
                if (Windows.Count == 0)
                {
                    return string.Format("{0}", Ref.Id);
                }
                return string.Format("{0} -> {1}", Ref.Id, string.Join(", ", Windows.ToArray()));
            }
            public int CompareTo(object another)
            {
                ProcessThread_ pm = another as ProcessThread_;
                return this.Ref.Id.CompareTo(pm.Ref.Id);
            }
        }

        public static List<ProcessThread_> GetThreads(Process p)
        {
            List<ProcessThread_> mods = new List<ProcessThread_>();
            foreach(ProcessThread t in p.Threads)
            {
                List<long> win = new List<long>();
                bool EnumThreadFn(IntPtr hWnd, IntPtr lParam)
                {
                    win.Add(hWnd.ToInt64());
                    return true;
                }
                EnumThreadWindows((uint)t.Id, new EnumThreadDelegate(EnumThreadFn), IntPtr.Zero);
                ProcessThread_ th = new ProcessThread_(t, win);
                mods.Add(th);
            }
            return mods;
        }


        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        public static byte[] ReadMemory(Process p, IntPtr baseAddr, uint size, List<string> _debug = null)
        {
            byte[] buf = new byte[size];
            uint nsize = 0;
            IntPtr hproc = OpenProcess(0x400 /* PROCESS_QUERY_INFORMATION  */ | 0x10 /* PROCESS_VM_READ */, false, p.Id);
            bool r = ReadProcessMemory(hproc, baseAddr, buf, size, out nsize);
            CloseHandle(hproc);
            Console.WriteLine($"Last p/invoke error: {GetLastErrorString()}");
            Console.WriteLine(string.Format("readMemory: {0} - {1}, {2} = {3}", r, p.Id, size, nsize));
            if (_debug != null)
            {
                _debug.Add($"Last p/invoke error: {GetLastErrorString()}");
                _debug.Add(string.Format("readMemory: {0} - {1}, {2} = {3}", r, p.Id, size, nsize));
            }
            if (!r) return null;
            if (nsize != size)
            {
                byte[] nbuf = new byte[nsize];
                Array.Copy(buf, nbuf, nsize);
                return nbuf;
            }
            return buf;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, uint dwSize, out uint lpNumberOfBytesRead);
        enum FORMAT_MESSAGE : uint
        {
            ALLOCATE_BUFFER = 0x00000100,
            IGNORE_INSERTS = 0x00000200,
            FROM_SYSTEM = 0x00001000,
            ARGUMENT_ARRAY = 0x00002000,
            FROM_HMODULE = 0x00000800,
            FROM_STRING = 0x00000400
        }

        [DllImport("kernel32.dll")]
        private static extern int FormatMessage(FORMAT_MESSAGE dwFlags, IntPtr lpSource, int dwMessageId, uint dwLanguageId, out StringBuilder msgOut, int nSize, IntPtr Arguments);

        //

        public static int GetLastError()
        {
            return (Marshal.GetLastWin32Error());
        }

        public static string GetLastErrorString()
        {
            int lastError = GetLastError();
            if (0 == lastError) return ("");
            else
            {
                StringBuilder msgOut = new StringBuilder(256);
                int size = FormatMessage(FORMAT_MESSAGE.ALLOCATE_BUFFER | FORMAT_MESSAGE.FROM_SYSTEM | FORMAT_MESSAGE.IGNORE_INSERTS,
                              IntPtr.Zero, lastError, 0, out msgOut, msgOut.Capacity, IntPtr.Zero);
                return (msgOut.ToString().Trim());
            }
        }
    }
}

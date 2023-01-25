using System.Diagnostics;
using System.Management;

namespace prView
{
    public static class DotNetMethods
    {
        public static string GetCommandLine(Process p)
        {
            try
            {
                string lastOne = "";
                using (ManagementObjectSearcher mos = new ManagementObjectSearcher(string.Format(
                    "SELECT CommandLine FROM Win32_Process WHERE ProcessId = {0}", p.Id
                )))
                {
                    foreach (ManagementObject mo in mos.Get())
                    {
                        lastOne = (string)mo["CommandLine"];
                    }
                }
                return lastOne;
            }
            catch
            {
                return "(no-access)";
            }
        }
    }
}

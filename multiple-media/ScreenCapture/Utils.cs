using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCapture
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
    }

    class Utils
    {

        public static Rectangle GetScreenSize(Form from)
        {
            Rectangle rect = Screen.FromControl(from).Bounds;
            //double dpiRate = 144.0 / from.DeviceDpi;
            double dpiRate = 1;
            rect.Width = (int)(rect.Width * dpiRate);
            rect.Height = (int)(rect.Height * dpiRate);
            return rect;
        }

        public static Bitmap CaptureScreenWithGDI(Form from, string outputFilename)
        {
            Rectangle rect = GetScreenSize(from);
            using (Bitmap bitmap = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(0, 0), Point.Empty, rect.Size);
                }
                if (outputFilename.Length > 0)
                {
                    bitmap.Save(outputFilename, ImageFormat.Png);
                }
                // MessageBox.Show("Screenshot taken! Press `OK` to continue...");
                return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.DontCare);
            }
        }
        public static Bitmap CaptureActiveWindow(string outputFilename)
        {
            IntPtr win = NativeMethods.GetForegroundWindow();
            Rectangle rect;
            if (win == IntPtr.Zero) return null;
            NativeMethods.GetWindowRect(win, out rect);
            rect.Width = rect.Width - rect.Left;
            rect.Height = rect.Height - rect.Top;
            if (rect.Width <= 0 || rect.Height <= 0) return null;
            using (Bitmap bitmap = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    Size winsize = new Size(rect.Width, rect.Height);
                    g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, winsize);
                }
                if (outputFilename.Length > 0)
                {
                    bitmap.Save(outputFilename, ImageFormat.Png);
                }
                // MessageBox.Show("Screenshot taken! Press `OK` to continue...");
                return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.DontCare);
            }
        }

        // use less memory than Graphics.CopyFromScreen
        public static Bitmap FastCaptureScreenCropAndStretch(int x, int y, int w, int h, int sw, int sh)
        {
            IntPtr desktop = NativeMethods.GetDesktopWindow();
            IntPtr src = NativeMethods.GetDC(desktop);
            Bitmap r = new Bitmap(sw, sh);
            Graphics g = Graphics.FromImage(r);
            IntPtr dst = g.GetHdc();
            NativeMethods.SetStretchBltMode(dst, NativeMethods.StretchBltMode.STRETCH_HALFTONE);
            NativeMethods.StretchBlt(dst, 0, 0, sw, sh, src, x, y, w, h, NativeMethods.TernaryRasterOperations.SRCCOPY);
            NativeMethods.DeleteDC(dst);
            g.Dispose();
            NativeMethods.ReleaseDC(desktop, src);
            return r;
        }
    }
}

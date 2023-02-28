using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;


namespace CVxLab
{
    static class BitmapUtils
    {
        public static Bitmap Crop(Image src, Rectangle bounds)
        {
            Bitmap p = new Bitmap(bounds.Width, bounds.Height);
            Rectangle r = new Rectangle(-bounds.X, -bounds.Y, bounds.X * 2 + bounds.Width, bounds.Y * 2 + bounds.Height);
            Graphics g = Graphics.FromImage(p);
            g.DrawImageUnscaledAndClipped(src, r);
            return p;
        }

        public static byte[] GetBitmapRaw(Bitmap src)
        {
            System.Drawing.Imaging.BitmapData x;
            x = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, src.PixelFormat);
            IntPtr xraw = x.Scan0;
            int len = src.Width * src.Height * 4;
            byte[] raw = new byte[len];
            System.Runtime.InteropServices.Marshal.Copy(xraw, raw, 0, len);
            src.UnlockBits(x);
            return raw;
        }

        public static Matrix[] ToRGBMatrix(Bitmap src)
        {
            // assume RBGA
            int size = src.Width * src.Height;
            byte[] raw = GetBitmapRaw(src);
            int len = raw.Length;
            double[] rawr = new double[size];
            double[] rawg = new double[size];
            double[] rawb = new double[size];
            for (int i = 0; i < len; i += 4)
            {
                int j = i >> 2;
                rawr[j] = raw[i] / 255.0;
                rawg[j] = raw[i+1] / 255.0;
                rawb[j] = raw[i+2] / 255.0;
            }
            Matrix[] ret = new Matrix[3] {
                new Matrix(src.Width, src.Height, rawr),
                new Matrix(src.Width, src.Height, rawg),
                new Matrix(src.Width, src.Height, rawb),
            };
            return ret;
        }
        public static Matrix[] ToRGBAMatrix(Bitmap src)
        {
            // assume RBGA
            int size = src.Width * src.Height;
            byte[] raw = GetBitmapRaw(src);
            int len = raw.Length;
            double[] rawr = new double[size];
            double[] rawg = new double[size];
            double[] rawb = new double[size];
            double[] rawa = new double[size];
            for (int i = 0; i < len; i += 4)
            {
                int j = i >> 2;
                rawr[j] = raw[i] / 255.0;
                rawg[j] = raw[i + 1] / 255.0;
                rawb[j] = raw[i + 2] / 255.0;
                rawa[j] = raw[i + 3] / 255.0;
            }
            Matrix[] ret = new Matrix[4] {
                new Matrix(src.Width, src.Height, rawr),
                new Matrix(src.Width, src.Height, rawg),
                new Matrix(src.Width, src.Height, rawb),
                new Matrix(src.Width, src.Height, rawa),
            };
            return ret;
        }

        public static int[][] CalcHist(Bitmap src)
        {
            int[] hr, hg, hb;
            hr = new int[256];
            hg = new int[256];
            hb = new int[256];
            byte[] raw = GetBitmapRaw(src);
            int len = raw.Length;
            for (int i = 0; i < len;)
            {
                hr[raw[i++]]++;
                hg[raw[i++]]++;
                hb[raw[i++]]++;
                i++;
            }

            int[] rgb = new[] { Utils.max(hr), Utils.max(hg), Utils.max(hb) };
            return new int[][] { hr, hg, hb, rgb };
        }

        public static Bitmap ToBitmap(Matrix src)
        {
            if (src.Col == 0 || src.Row == 0) return null;
            double[] raw = src.GetRaw();
            int len = src.Row * src.Col * 4;
            byte[] bitmapraw = new byte[len];
            for (int i = 0, j = 0; i < raw.Length; i++)
            {
                byte b = (byte)(raw[i] * 255);
                bitmapraw[j++] = b;
                bitmapraw[j++] = b;
                bitmapraw[j++] = b;
                bitmapraw[j++] = 255;
            }
            Bitmap bmp = new Bitmap(src.Row, src.Col);
            System.Drawing.Imaging.BitmapData x;
            x = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
            IntPtr xraw = x.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(bitmapraw, 0, xraw, len);
            bmp.UnlockBits(x);
            return bmp;
        }

        public static void unusedGetDpi()
        {
            IntPtr desktop = NativeMethods.GetDC(IntPtr.Zero);
            int dpiX = NativeMethods.GetDeviceCaps(desktop, 88 /* LOGPIXELSX */);
            int dpiY = NativeMethods.GetDeviceCaps(desktop, 90 /* LOGPIXELSY */);
            double scaleX = dpiX / 96.0;
            double scaleY = dpiY / 96.0;
        }
    }
}


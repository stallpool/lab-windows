using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCapture
{
    public partial class main : Form
    {
        private delegate int SetProcessDpiAwareness(int mode);

        public main()
        {
            EnableDpiAwareness();
            InitializeComponent();
        }

        private void EnableDpiAwareness()
        {
            // skip this procedure if Windows version is below
            // Shcore.dll#SetProcessDpiAwareness [min] Windows 8.1 / Windows Server 2012 R2
            IntPtr shcoreDll = NativeMethods.LoadLibrary(@"Shcore.dll");
            if (shcoreDll != IntPtr.Zero)
            {
                IntPtr ptrSetProcessDpiAwareness = NativeMethods.GetProcAddress(shcoreDll, "SetProcessDpiAwareness");
                if (ptrSetProcessDpiAwareness != IntPtr.Zero)
                {
                    SetProcessDpiAwareness fnSetProcessDpiAwareness = (SetProcessDpiAwareness)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(ptrSetProcessDpiAwareness, typeof(SetProcessDpiAwareness));
                    fnSetProcessDpiAwareness(/* DPI_AWARENESS_SYSTEM_AWARE */ 1);
                }
                NativeMethods.FreeLibrary(shcoreDll);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult r = saveDialog.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                txtFilename.Text = saveDialog.FileName;
            }
            else if (r == DialogResult.Cancel)
            {
                txtFilename.Text = "";
            }
        }

        private void btnCaptureGDI_Click(object sender, EventArgs e)
        {
            Bitmap bmp = Utils.CaptureScreenWithGDI(this, txtFilename.Text);
            if (picScreen.Image != null) picScreen.Image.Dispose();
            picScreen.Image = bmp;
        }

        private void btnTimer_Click(object sender, EventArgs e)
        {
            if (btnTimer.Text == "Start")
            {
                btnTimer.Text = "Stop";
                tmCapture.Enabled = true;
            }
            else
            {
                btnTimer.Text = "Start";
                tmCapture.Enabled = false;
            }
        }

        private void tmCapture_Tick(object sender, EventArgs e)
        {
            Bitmap bmp;
            if (chkActiveWin.Checked)
            {
                bmp = Utils.CaptureActiveWindow("");
            }
            else
            {
                bmp = Utils.CaptureScreenWithGDI(this, "");
            }
            if (bmp == null) return;
            using (var t = new ResourcesTracker())
            {
                var src = t.T(OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp));
                var gray = t.NewMat();
                var dst = t.NewMat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); ;
                //OpenCvSharp.Cv2.Canny(src, dst2, 50, 200);
                OpenCvSharp.XFeatures2D.SURF surf = OpenCvSharp.XFeatures2D.SURF.Create(hessianThreshold: 200, upright: true);
                KeyPoint[] keypoints = surf.Detect(gray, null);
                Cv2.DrawKeypoints(src, keypoints, dst, new Scalar(0, 255, 0), DrawMatchesFlags.Default);
                bmp.Dispose();
                bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            }
            if (picScreen.Image != null) picScreen.Image.Dispose();
            picScreen.Image = bmp;
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if (openDialog.ShowDialog() != DialogResult.OK) return;
            if (picFilter.Image != null) picFilter.Image.Dispose();
            picFilter.Image = Image.FromFile(openDialog.FileName);
        }

        private void btnMatch_Click(object sender, EventArgs e)
        {
            if (picScreen.Image == null || picFilter.Image == null) return;
            Bitmap screen = new Bitmap(picScreen.Image);
            Bitmap filter = new Bitmap(picFilter.Image);
            using (var t = new ResourcesTracker())
            {
                var sm = t.T(OpenCvSharp.Extensions.BitmapConverter.ToMat(screen));
                var sf = t.T(OpenCvSharp.Extensions.BitmapConverter.ToMat(filter));
                var gm = t.NewMat();
                var gf = t.NewMat();
                var dst = t.NewMat();
                Cv2.CvtColor(sm, gm, ColorConversionCodes.BGR2GRAY);
                Cv2.CvtColor(sf, gf, ColorConversionCodes.BGR2GRAY);
                OpenCvSharp.XFeatures2D.SURF surf = OpenCvSharp.XFeatures2D.SURF.Create(hessianThreshold: 200, upright: true);
                KeyPoint[] kpm, kpf;
                var desm = t.NewMat(); var desf = t.NewMat();
                surf.DetectAndCompute(gm, null, out kpm, desm);
                surf.DetectAndCompute(gf, null, out kpf, desf);
                var bf = new BFMatcher();
                DMatch[][] matches = bf.KnnMatch(desm, desf, 2);
                List<DMatch[]> good = new List<DMatch[]>();
                foreach (DMatch[] m in matches)
                {
                    if (m[0].Distance < 0.7 * m[1].Distance) good.Add(m);
                }

                Cv2.DrawMatchesKnn(sm, kpm, sf, kpf, good.ToArray(), dst);
                using (new Window("Matches", dst, WindowFlags.AutoSize))
                {
                    Cv2.WaitKey();
                }
            }
        }

        private void btnMatchTemplate_Click(object sender, EventArgs e)
        {
            if (picScreen.Image == null || picFilter.Image == null) return;
            Bitmap screen = new Bitmap(picScreen.Image);
            Bitmap filter = new Bitmap(picFilter.Image);
            using (var t = new ResourcesTracker())
            {
                var sm = t.T(OpenCvSharp.Extensions.BitmapConverter.ToMat(screen));
                var sf = t.T(OpenCvSharp.Extensions.BitmapConverter.ToMat(filter));
                var gm = t.NewMat();
                var gf = t.NewMat();
                var dst = t.NewMat();
                var r = t.NewMat();
                Cv2.CvtColor(sm, gm, ColorConversionCodes.BGR2GRAY);
                Cv2.CvtColor(sf, gf, ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gm, gf, r, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(r, r, 0.9, 255, ThresholdTypes.Tozero);

                Console.WriteLine("start ....");
                while (true)
                {
                    double minval, maxval, threshold = 0.95;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(r, out minval, out maxval, out minloc, out maxloc);
                    Console.WriteLine("Detected " + maxval + ":" + maxloc.X + " " + maxloc.Y);
                    if (maxval > threshold)
                    {
                        Console.WriteLine("Matched " + maxloc.X + " " + maxloc.Y);
                        Cv2.FloodFill(r, maxloc, new Scalar());
                    }
                    else
                    {
                        break;
                    }
                }
                Console.WriteLine("end ....");
            }
        }
    }
}

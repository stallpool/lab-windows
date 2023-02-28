#ifndef _SEVLAB_CNCV_
#define _SEVLAB_CNCV_

class SevlabBitmap;
class SevlabMonitor;
typedef unsigned char *PBITS;
typedef double (*FPDIFF)(PBITS, PBITS, int, int);
void SevlabGetDpiScale();

bool sevlabCVReady = true, sevlabCVPaused = false, sevlabCVExit = true;
bool sevlabInitScale = false;
double sevlabScaleX = 1.0, sevlabScaleY = 1.0;
DWORD sevlabTimer = 0, sevlabInterval = 200;

#define SEVLAB_MONITOR_MAX_N 25
int sevlabMonitorN = 0;
SevlabMonitor *sevlabMonitors[SEVLAB_MONITOR_MAX_N];

class SevlabBitmap {
protected:
        HBITMAP bmp;
        int w, h;
        bool disposed;

        void _set(HBITMAP bmp, int w, int h) {
                this->bmp = bmp;
                this->w = w;
                this->h = h;
                this->disposed = false;
        }

public:
        SevlabBitmap(const SevlabBitmap& pbmp) {
                this->_set(pbmp.bmp, pbmp.w, pbmp.h);
        }
        SevlabBitmap(HBITMAP bmp, int x, int y, int w, int h) {
                this->_set(SevlabBitmap::_CloneHBITMAP(bmp, x, y, w, h), w, h);
        }
        SevlabBitmap(HBITMAP bmp, int x, int y, int w, int h, int cw, int ch) {
                this->_set(SevlabBitmap::_CloneHBITMAPAndStretch(bmp, x, y, w, h, cw, ch), cw, ch);
        }
        SevlabBitmap(HBITMAP bmp) {
                BITMAP obj;
                GetObject(bmp, sizeof(obj), &obj);
                this->_set(SevlabBitmap::_CloneHBITMAP(bmp), obj.bmWidth, obj.bmHeight);
        }
        SevlabBitmap(int w, int h) {
                this->w = w;
                this->h = h;
                HWND desktop = GetDesktopWindow();
                HDC rootDC = GetDC(desktop);
                this->_set(CreateCompatibleBitmap(rootDC, w, h), w, h);
                ReleaseDC(desktop, rootDC);
        }
        ~SevlabBitmap() { this->Dispose(); }

        void Dispose() {
                if (this->disposed) return;
                DeleteObject(this->bmp);
                this->bmp = NULL;
                this->w = 0;
                this->h = 0;
                this->disposed = true;
        }

        void Draw(HDC hdc, int x, int y) {
                HDC srcHdc = CreateCompatibleDC(hdc);
                SelectObject(srcHdc, this->bmp);
                BitBlt(hdc, x, y, this->w, this->h, srcHdc, 0, 0, SRCCOPY);
                DeleteDC(srcHdc);
        }

        SevlabBitmap* Crop(int x, int y, int w, int h) {
                return new SevlabBitmap(this->bmp, x, y, w, h);
        }

        SevlabBitmap* Stretch(int w, int h) {
                return new SevlabBitmap(this->bmp, 0, 0, this->w, this->h, w, h);
        }

        SevlabBitmap* CropAndStretch(int x, int y, int w, int h, int cw, int ch) {
                return new SevlabBitmap(this->bmp, x, y, w, h, cw, ch);
        }

        UCHAR* GetBits() {
                HWND desktop = GetDesktopWindow();
                HDC rootDC = GetDC(desktop);
                HDC dstDC = CreateCompatibleDC(rootDC);
                SelectObject(dstDC, this->bmp);
                BITMAP obj;
                GetObject(this->bmp, sizeof(BITMAP), &obj);
                BITMAPINFOHEADER bi;
                bi.biSize = sizeof(BITMAPINFOHEADER);
                bi.biWidth = obj.bmWidth;
                bi.biHeight = obj.bmHeight;
                bi.biPlanes = obj.bmPlanes;
                bi.biBitCount = obj.bmBitsPixel;
                bi.biCompression = BI_RGB;
                bi.biSizeImage = 0;
                bi.biXPelsPerMeter = 0;
                bi.biYPelsPerMeter = 0;
                bi.biClrUsed = 0;
                bi.biClrImportant = 0;
                int dwBmpSize = (((obj.bmWidth * bi.biBitCount + 31) >> 5) << 2) * obj.bmHeight;
                UCHAR* out = (UCHAR*)malloc(dwBmpSize);
                GetDIBits(dstDC, this->bmp, 0, obj.bmHeight, out, (BITMAPINFO*)&bi, DIB_RGB_COLORS);
                DeleteObject(dstDC);
                ReleaseDC(desktop, rootDC);
                return out;
        }
        void SetBits(UCHAR* bits) {
                HWND desktop = GetDesktopWindow();
                HDC rootDC = GetDC(desktop);
                HDC dstDC = CreateCompatibleDC(rootDC);
                SelectObject(dstDC, this->bmp);
                BITMAP obj;
                GetObject(this->bmp, sizeof(BITMAP), &obj);
                BITMAPINFOHEADER bi;
                bi.biSize = sizeof(BITMAPINFOHEADER);
                bi.biWidth = obj.bmWidth;
                bi.biHeight = obj.bmHeight;
                bi.biPlanes = obj.bmPlanes;
                bi.biBitCount = obj.bmBitsPixel;
                bi.biCompression = BI_RGB;
                bi.biSizeImage = 0;
                bi.biXPelsPerMeter = 0;
                bi.biYPelsPerMeter = 0;
                bi.biClrUsed = 0;
                bi.biClrImportant = 0;
                SetDIBits(dstDC, this->bmp, 0, obj.bmHeight, bits, (BITMAPINFO*)&bi, DIB_RGB_COLORS);
                DeleteObject(dstDC);
                ReleaseDC(desktop, rootDC);
        }

        UINT* GetHistogram() {
                UCHAR* raw = this->GetBits(); // may out of range
                UINT* rgb = (UINT*)malloc(sizeof(UINT) * 257 * 3);
                if (!rgb) return NULL;
                UINT maxr = 0, maxg = 0, maxb = 0;
                memset(rgb, 0, sizeof(UINT) * 257 * 3);
                int size = w * h * 4;
                UCHAR* end = raw + size;
                UCHAR* cur = raw;
                while (cur < end) {
                        UCHAR ch;
                        ch = *cur; rgb[ch]++; cur++;
                        ch = *cur; rgb[ch + 257]++; cur++;
                        ch = *cur; rgb[ch + 514]++; cur++;
                        cur++;
                }
                UINT* vp = rgb;
                for (int i = 0; i < 256; i++) {
                        UINT v = vp[i];
                        if (v > maxr) maxr = v;
                }
                vp += 257;
                for (int i = 0; i < 256; i++) {
                        UINT v = vp[i];
                        if (v > maxg) maxg = v;
                }
                vp += 257;
                for (int i = 0; i < 256; i++) {
                        UINT v = vp[i];
                        if (v > maxb) maxb = v;
                }
                rgb[256] = maxr;
                rgb[256 + 257] = maxg;
                rgb[256 + 257 * 2] = maxb;
                free(raw);
                return rgb;
        }

        bool IsDisposed() { return this->disposed; }
        int GetW() { return this->w; }
        int GetH() { return this->h;  }
        HBITMAP GetRaw() { return this->bmp; }

        static SevlabBitmap* CopyFromScreen(int x, int y, int w, int h) {
                HWND desktop = GetDesktopWindow();
                HDC srcDC = GetDC(desktop);
                HDC dstDC = CreateCompatibleDC(srcDC);
                HBITMAP bmp = CreateCompatibleBitmap(srcDC, w, h);
                SelectObject(dstDC, bmp);
                BitBlt(dstDC, 0, 0, w, h, srcDC, x, y, SRCCOPY);
                DeleteDC(dstDC);
                ReleaseDC(desktop, srcDC);
                SevlabBitmap* pbmp = new SevlabBitmap(bmp);
                DeleteObject(bmp);
                return pbmp;
        }
        static SevlabBitmap* CopyFromActiveWindow() {
                HWND window = GetForegroundWindow();
                RECT rect;
                GetWindowRect(window, &rect);
                return SevlabBitmap::CopyFromScreen(
                        rect.left, rect.top,
                        rect.right - rect.left,
                        rect.bottom - rect.top
                );
        }
        static SevlabBitmap* CopyFromActiveWindow(int x, int y, int w, int h) {
                HWND window = GetForegroundWindow();
                RECT rect;
                GetWindowRect(window, &rect);
                return SevlabBitmap::CopyFromScreen(rect.left + x, rect.top + y, w, h);
        }
        static SevlabBitmap* CopyFromScreenAndStretch(int x, int y, int w, int h, int cw, int ch) {
                HWND desktop = GetDesktopWindow();
                HDC srcDC = GetDC(desktop);
                HDC dstDC = CreateCompatibleDC(srcDC);
                HBITMAP bmp = CreateCompatibleBitmap(srcDC, cw, ch);
                SelectObject(dstDC, bmp);
                SetStretchBltMode(dstDC, HALFTONE);
                StretchBlt(dstDC, 0, 0, cw, ch, srcDC, x, y, w, h, SRCCOPY);
                DeleteDC(dstDC);
                ReleaseDC(desktop, srcDC);
                SevlabBitmap* pbmp = new SevlabBitmap(bmp);
                DeleteObject(bmp);
                return pbmp;
        }
        static SevlabBitmap* CopyFromActiveWindowAndStretch(int cw, int ch) {
                HWND window = GetForegroundWindow();
                RECT rect;
                GetWindowRect(window, &rect);
                return SevlabBitmap::CopyFromScreenAndStretch(
                        rect.left, rect.top,
                        rect.right - rect.left,
                        rect.bottom - rect.top,
                        cw, ch
                );
        }
        static SevlabBitmap* CopyFromActiveWindowAndStretch(int x, int y, int w, int h, int cw, int ch) {
                HWND window = GetForegroundWindow();
                RECT rect;
                GetWindowRect(window, &rect);
                return SevlabBitmap::CopyFromScreenAndStretch(rect.left + x, rect.top + y, w, h, cw, ch);
        }
        static SevlabBitmap* FromFile(LPCWSTR filename) {
                HBITMAP bmp = (HBITMAP)LoadImageW(NULL, filename, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE | LR_CREATEDIBSECTION);
                SevlabBitmap* ret = new SevlabBitmap(bmp);
                DeleteObject(bmp);
                return ret;
        }
        static SevlabBitmap* FromArray(UCHAR* bits, int w, int h) {
                SevlabBitmap* ret = new SevlabBitmap(w, h);
                ret->SetBits(bits);
                return ret;
        }

        static HBITMAP _CloneHBITMAP(HBITMAP bmp) {
                BITMAP obj;
                GetObject(bmp, sizeof(obj), &obj);
                return _CloneHBITMAP(bmp, 0, 0, obj.bmWidth, obj.bmHeight);
        }
        static HBITMAP _CloneHBITMAP(HBITMAP bmp, int x, int y, int w, int h) {
                HWND desktop = GetDesktopWindow();
                HDC rootDC = GetDC(desktop);
                HDC srcHdc = CreateCompatibleDC(rootDC);
                HDC dstHdc = CreateCompatibleDC(rootDC);
                HBITMAP dstBmp = CreateCompatibleBitmap(rootDC, w, h);
                SelectObject(srcHdc, bmp);
                SelectObject(dstHdc, dstBmp);
                BitBlt(dstHdc, 0, 0, w, h, srcHdc, x, y, SRCCOPY);
                DeleteDC(srcHdc);
                DeleteDC(dstHdc);
                ReleaseDC(desktop, rootDC);
                return dstBmp;
        }
        static HBITMAP _CloneHBITMAPAndStretch(HBITMAP bmp, int x, int y, int w, int h, int cw, int ch) {
                HWND desktop = GetDesktopWindow();
                HDC rootDC = GetDC(desktop);
                HDC srcHdc = CreateCompatibleDC(rootDC);
                HDC dstHdc = CreateCompatibleDC(rootDC);
                HBITMAP dstBmp = CreateCompatibleBitmap(rootDC, cw, ch);
                SelectObject(srcHdc, bmp);
                SelectObject(dstHdc, dstBmp);
                SetStretchBltMode(dstHdc, HALFTONE);
                StretchBlt(dstHdc, 0, 0, cw, ch, srcHdc, x, y, w, h, SRCCOPY);
                DeleteDC(srcHdc);
                DeleteDC(dstHdc);
                ReleaseDC(desktop, rootDC);
                return dstBmp;
        }
};

class SevlabMonitor {
public:
        int x, y, w, h, cw, ch;
        int rx, ry, rw, rh; // real x, y, w, h (dpi scale)
        int diffN, diffCapN;
        double *score;
        SevlabBitmap **diff;
        PBITS *diffBits;

        SevlabMonitor(int x, int y, int w, int h, int cw, int ch, int diffCap)
        {
                this->x = x;
                this->y = y;
                this->w = w;
                this->h = h;
                if (!sevlabInitScale) {
                        sevlabInitScale = true;
                        SevlabGetDpiScale();
                }
                this->DpiFit();

                this->cw = cw;
                this->ch = ch;
                this->diffN = 0;
                this->diffCapN = diffCap;
                this->diff = (SevlabBitmap**)malloc(sizeof(SevlabBitmap*) * diffCap);
                this->diffBits = (PBITS*)malloc(sizeof(PBITS) * diffCap);
                this->score = (double*)malloc(sizeof(double) * diffCap);
        }

        ~SevlabMonitor()
        {
                this->DiffClear();
                free(this->score);
                free(this->diffBits);
                free(this->diff);
        }

        void DpiFit()
        {
                this->rx = (int)(this->x * sevlabScaleX);
                this->ry = (int)(this->y * sevlabScaleY);
                this->rw = (int)(this->w * sevlabScaleX);
                this->rh = (int)(this->h * sevlabScaleY);
        }

        bool Diff(PBITS one, FPDIFF fn)
        {
                for(int i = 0; i < this->diffN; i ++) {
                        PBITS bits = this->diffBits[i];
                        if (!bits) continue;
                        this->score[i] = fn(one, bits, this->cw, this->ch);
                }
                return true;
        }

        double* GetScore() { return this->score; }

        bool DiffAdd(PBITS one)
        {
                if (this->diffN >= this->diffCapN) return false;
                SevlabBitmap* bmp = SevlabBitmap::FromArray(one, this->cw, this->ch);
                int size = this->cw * this->ch * 4;
                PBITS bits = (PBITS)malloc(sizeof(unsigned char) * size);
                memcpy(bits, one, size);
                this->diffBits[this->diffN] = bits;
                this->diff[this->diffN++] = bmp;
                return true;
        }

        bool DiffClear()
        {
                while (this->diffN-- > 0) {
                        SevlabBitmap* bmp = this->diff[this->diffN];
                        PBITS bmpBits = this->diffBits[this->diffN];
                        free(bmpBits);
                        delete bmp;
                }
                this->diffN = 0;
                return true;
        }
};


void SevlabGetForegroundWindowTitle(WCHAR* out, int size) {
        HWND window = GetForegroundWindow();
        GetWindowTextW(window, out, size);
}

void SevlabGetDpiScale()
{
        HDC desktop = GetDC(NULL);
        int dpix = GetDeviceCaps(desktop, LOGPIXELSX);
        int dpiy = GetDeviceCaps(desktop, LOGPIXELSY);
        ReleaseDC(NULL, desktop);
        if (!dpix) dpix = 96;
        if (!dpiy) dpiy = 96;
        sevlabScaleX = dpix / 96.0;
        sevlabScaleY = dpiy / 96.0;
}

using SevlabSetProcessDpiAwareness = HRESULT(WINAPI *)(int);
void EnableDpiAwareness()
{
        HMODULE shcoreDll = LoadLibraryW(L"shcore.dll");
        if (shcoreDll) {
                SevlabSetProcessDpiAwareness fnSetProcessDpiAwareness = (SevlabSetProcessDpiAwareness)GetProcAddress(shcoreDll, "SetProcessDpiAwareness");
                if (fnSetProcessDpiAwareness) {
                        fnSetProcessDpiAwareness(DPI_AWARENESS_SYSTEM_AWARE);
                        SevlabGetDpiScale();
                }
                FreeLibrary(shcoreDll);
        }
}

double diffBitsSquadSum(PBITS c1, PBITS c2, int w, int h) {
        int size = w * h * 4;
        double diff = 0.0;
        while (size--) {
                if (size % 4 == 0) { c1++; c2++; continue; }
                UCHAR v1 = *c1++, v2 = *c2++;
                if (v1 == v2) continue;
                double d = (v1 > v2 ? (v1 - v2) : (v2 - v1)) / 255.0;
                diff += d * d;
        }
        return diff;
}

bool SevlabMonitorBitsAdd(int index, PBITS one)
{
        bool ret = false;
        sevlabCVReady = false;
        if (index < 0 || index >= sevlabMonitorN) return false; //break;
        SevlabMonitor* m = sevlabMonitors[index];
        m->DiffAdd(one);
        ret = true;
        sevlabCVReady = true;
        return ret;
}

bool SevlabMonitorBitsClear(int index)
{
        bool ret = false;
        sevlabCVReady = false;
        if (index < 0 || index >= sevlabMonitorN) return false; //break;
        SevlabMonitor* m = sevlabMonitors[index];
        m->DiffClear();
        ret = true;
        sevlabCVReady = true;
        return ret;
}

double* SevlabMonitorBitsDiffScore(int index, int* outN)
{
        // side effect: we do not lock the memory; may have race condition
        if (index < 0 || index >= sevlabMonitorN) return NULL;
        SevlabMonitor* m = sevlabMonitors[index];
        *outN = m->diffN;
        return m->GetScore();
}

bool SevlabMonitorAdd(int x, int y, int w, int h, int cw, int ch, int diffCap)
{
        bool ret = false;
        sevlabCVReady = false;
        if (sevlabMonitorN >= SEVLAB_MONITOR_MAX_N) return false; // break;
        SevlabMonitor* m = new SevlabMonitor(x, y, w, h, cw, ch, diffCap);
        sevlabMonitors[sevlabMonitorN++] = m;
        ret = true;
        sevlabCVReady = true;
        return ret;
}

bool SevlabMonitorClear()
{
        bool ret = false;
        sevlabCVReady = false;
        for(int i = 0; i < SEVLAB_MONITOR_MAX_N; i++) {
                SevlabMonitor* m = sevlabMonitors[i];
                if (!m) continue;
                delete m;
        }
        sevlabMonitorN = 0;
        ret = true;
        sevlabCVReady = true;
        return ret;
}

DWORD WINAPI SevlabThreadProc(void* Param)
{
        // XXX: there is race condition on modifying monitors
        wchar_t namebuf[1024];
        while (!sevlabCVExit) {
                if (!sevlabCVReady) {
                        sevlabCVPaused = true;
                        Sleep(sevlabInterval);
                        continue;
                }
                sevlabCVPaused = false;
                __try {
                        SevlabGetForegroundWindowTitle(namebuf, 1024);
                        if (lstrcmpW(L"League of Legends", namebuf) == 0) {
                                SevlabBitmap* x0 = SevlabBitmap::CopyFromActiveWindow();
                                for (int i = 0; i < sevlabMonitorN; i++) {
                                        SevlabMonitor* m = sevlabMonitors[i];
                                        SevlabBitmap* x = x0->CropAndStretch(m->rx, m->ry, m->rw, m->rh, m->cw, m->ch);
                                        PBITS xbits = x->GetBits();
                                        m->Diff(xbits, diffBitsSquadSum);
                                        free(xbits);
                                        delete x;
                                }
                                delete x0;
                        }
                        sevlabTimer = (sevlabTimer + 1) % 10000;
                        if (sevlabTimer % 5 == 0) {
                                // refresh dpi scale per 5 ticks
                                double lastsx = sevlabScaleX, lastsy = sevlabScaleY;
                                SevlabGetDpiScale();
                                if (lastsx != sevlabScaleX || lastsy != sevlabScaleY) {
                                        // dpi scale change (e.g. adjust capture rect)
                                        for (int i = 0; i < sevlabMonitorN; i++) {
                                                SevlabMonitor* m = sevlabMonitors[i];
                                                m->DpiFit();
                                        }
                                }
                        }
                } __finally {
                }
                Sleep(sevlabInterval);
        }
        return 0;
}

void SevlabCVStart()
{
        sevlabCVExit = false;
        sevlabCVReady = false;
        // check ready
        EnableDpiAwareness(); // ensure dpi awareness
        sevlabCVReady = true;
        DWORD tid = 0;
        CreateThread(nullptr, 0, SevlabThreadProc, NULL, 0, &tid);
}

void SevlabCVStop()
{
        // cleanup
        SevlabMonitorClear();
        sevlabCVReady = false;
        sevlabCVExit = true;
}

#endif

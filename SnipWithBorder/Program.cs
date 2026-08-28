using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnipWithBorder
{
    internal static class Program
    {
        #region Win32

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData, flags, time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode, scanCode, flags, time;
            public IntPtr dwExtraInfo;
        }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int idHook, HookProc proc, IntPtr hMod, uint threadId);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")] private static extern IntPtr WindowFromPoint(POINT pt);
        [DllImport("user32.dll")] private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")] private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
        [DllImport("user32.dll")] private static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr name);
        [DllImport("user32.dll")] private static extern IntPtr CopyIcon(IntPtr hIcon);
        [DllImport("user32.dll")] private static extern bool SetSystemCursor(IntPtr hCursor, uint id);
        [DllImport("user32.dll")] private static extern bool SystemParametersInfo(uint action, uint uiParam, IntPtr pvParam, uint fWinIni);
        [DllImport("user32.dll")] private static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")] private static extern bool SetProcessDPIAware();
        [DllImport("shcore.dll")] private static extern int SetProcessDpiAwareness(int value);
        [DllImport("shcore.dll")] private static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);
        [DllImport("dwmapi.dll")] private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);
        [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string name);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(string section, string key, string def,
            System.Text.StringBuilder buf, uint size, string file);

        private const int  WH_MOUSE_LL              = 14;
        private const int  WH_KEYBOARD_LL           = 13;
        private const int  WM_LBUTTONDOWN           = 0x0201;
        private const int  WM_RBUTTONDOWN           = 0x0204;
        private const int  WM_KEYDOWN               = 0x0100;
        private const uint GA_ROOT                  = 2;
        private const uint MONITOR_DEFAULTTONEAREST = 2;
        private const int  MDT_EFFECTIVE_DPI        = 0;
        private const uint OCR_NORMAL               = 32512;  // System arrow cursor id
        private const uint SPI_SETCURSORS           = 0x0057;
        private const uint VK_ESCAPE                = 0x1B;
        private const uint VK_Z                     = 0x5A;   // default trigger key (Alt+Z)
        private const int  WM_SYSKEYDOWN            = 0x0104;  // sent while Alt is held
        private const uint LLKHF_ALTDOWN            = 0x20;   // flag bit in KBDLLHOOKSTRUCT.flags
        private const int  DWMWA_EXTENDED_FRAME_BOUNDS = 9;    // visible window rect, excludes transparent resize border
        private static int  BorderAt96Dpi               = 6;    // base border width in physical pixels at 96 DPI; overridden by config

        // Virtual desktop bounds
        private static Rectangle VirtualScreen => new Rectangle(
            GetSystemMetrics(76), GetSystemMetrics(77),
            GetSystemMetrics(78), GetSystemMetrics(79));

        #endregion

        private enum AppState { Waiting, Selecting }

        private static IntPtr   _mouseHook    = IntPtr.Zero;
        private static IntPtr   _keyboardHook = IntPtr.Zero;
        private static HookProc _mouseProc;    // Strong references – prevent GC while hooks are live
        private static HookProc _keyboardProc;
        private static AppState      _state    = AppState.Waiting;
        private static bool             _done;
        private static uint             _triggerVk = VK_Z;   // overridden by config at startup

        [STAThread]
        static void Main()
        {
            // Ensure only one instance runs at a time.
            bool createdNew;
            var mutex = new System.Threading.Mutex(true, "SnipWithBorder_SingleInstance", out createdNew);
            if (!createdNew)
                return;

            // Per-monitor DPI awareness keeps GetWindowRect / CopyFromScreen in physical pixels.
            // Fall back to system-DPI awareness on Windows 7 where shcore.dll is absent.
            try { SetProcessDpiAwareness(2 /* PROCESS_PER_MONITOR_DPI_AWARE */); }
            catch { try { SetProcessDPIAware(); } catch { } }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ReadConfig();

            // Only the keyboard hook starts immediately.
            // no mouse hook is installed until the user presses the trigger (Alt+P).
            _keyboardProc = OnKeyboardEvent;
            _keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, GetModuleHandle(null), 0);

            // A minimised form gives the app a taskbar button so the user can close it.
            // Closing the form (via taskbar right-click → Close, or the title-bar X when
            // restored) ends Application.Run() and falls through to the cleanup below.
            using (var mainForm = new Form
            {
                Text             = "SnipWithBorder",
                ShowInTaskbar    = true,
                WindowState      = FormWindowState.Minimized,
                FormBorderStyle  = FormBorderStyle.FixedSingle,
                MaximizeBox      = false,
                ClientSize       = new Size(310, 48),
                StartPosition    = FormStartPosition.CenterScreen,
            })
            {
                mainForm.Controls.Add(new Label
                {
                    Text     = "Alt-" + (char)_triggerVk,
                    AutoSize = true,
                    Location = new Point(8, 14),
                });

                string iconPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Application.ExecutablePath),
                    @"Icons\SnipWithBorder.ico");
                if (System.IO.File.Exists(iconPath))
                    mainForm.Icon = new Icon(iconPath);

                Application.Run(mainForm);  // Ends when the form is closed or Application.Exit() is called
            }

            if (_mouseHook != IntPtr.Zero) UnhookWindowsHookEx(_mouseHook);
            UnhookWindowsHookEx(_keyboardHook);
            RestoreDefaultCursors();
        }

        // ── State transitions ────────────────────────────────────────────────────

        // ── Config ───────────────────────────────────────────────────────────────

        // Reads [SnipWithBorder] from %LocalAppData%\RightClickTools\RightClickTools.ini
        // and applies Key= and BorderWidth= settings.
        private static void ReadConfig()
        {
            string iniPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"RightClickTools\RightClickTools.ini");

            var buf = new System.Text.StringBuilder(64);

            GetPrivateProfileString("SnipWithBorder", "Key", "Alt-Z", buf, (uint)buf.Capacity, iniPath);
            _triggerVk = ParseAltKey(buf.ToString().Trim());

            GetPrivateProfileString("SnipWithBorder", "BorderWidth", "6", buf, (uint)buf.Capacity, iniPath);
            int bw;
            if (int.TryParse(buf.ToString().Trim(), out bw) && bw > 0)
                BorderAt96Dpi = bw;
        }

        // Parses "Alt-X" or "Alt+X" (case-insensitive) into a virtual-key code.
        // Returns VK_Z (the default) if the string cannot be parsed.
        private static uint ParseAltKey(string value)
        {
            char[] separators = { '-', '+' };
            foreach (string part in value.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                string p = part.Trim();
                if (string.Equals(p, "Alt", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (p.Length == 1 && (char.IsLetter(p[0]) || char.IsDigit(p[0])))
                    return (uint)char.ToUpperInvariant(p[0]);
            }
            return VK_Z;
        }

        private static void BeginSelecting()
        {
            _state = AppState.Selecting;
            SetPickingCursor();
            _mouseProc = OnMouseEvent;
            _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, GetModuleHandle(null), 0);
        }

        private static void BeginWaiting()
        {
            _state = AppState.Waiting;
            if (_mouseHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHook);
                _mouseHook = IntPtr.Zero;
            }
            RestoreDefaultCursors();
        }

        // ── Cursor helpers ───────────────────────────────────────────────────────

        private static void SetPickingCursor()
        {
            IntPtr hHand = LoadCursor(IntPtr.Zero, new IntPtr(32649 /* IDC_HAND */));
            if (hHand == IntPtr.Zero) return;
            IntPtr copy = CopyIcon(hHand);  // SetSystemCursor takes ownership of the handle
            if (copy != IntPtr.Zero)
                SetSystemCursor(copy, OCR_NORMAL);
        }

        private static void RestoreDefaultCursors()
        {
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, 0);
        }

        private static IntPtr OnMouseEvent(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && _state == AppState.Selecting && !_done)
            {
                int msg = (int)wParam;
                if (msg == WM_LBUTTONDOWN)
                {
                    var info = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                    Capture(info.pt);
                    return new IntPtr(1);   // Swallow – don't let the click reach the target window
                }
                if (msg == WM_RBUTTONDOWN)
                {
                    BeginWaiting();         // Right-click cancels selection; tool keeps waiting
                    return new IntPtr(1);
                }
            }
            return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
        }

        private static IntPtr OnKeyboardEvent(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = (int)wParam;
                var info = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                // Alt+<key> – trigger: toggle between Waiting and Selecting
                if (msg == WM_SYSKEYDOWN
                    && info.vkCode == _triggerVk
                    && (info.flags & LLKHF_ALTDOWN) != 0)
                {
                    if (_state == AppState.Waiting)
                        BeginSelecting();
                    else if (_state == AppState.Selecting)
                        BeginWaiting();
                    return new IntPtr(1);   // Swallow so Alt+P doesn't activate menus
                }

                // Escape: cancel selection and return to waiting
                if (msg == WM_KEYDOWN && info.vkCode == VK_ESCAPE && _state == AppState.Selecting)
                    BeginWaiting();
            }
            return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        private static void Capture(POINT clickPt)
        {
            _done = true;
            RestoreDefaultCursors();  // Restore before the screenshot so the crosshair doesn't appear

            try
            {
                // Identify the root (top-level) window at the click point
                IntPtr hwnd = WindowFromPoint(clickPt);
                if (hwnd != IntPtr.Zero)
                    hwnd = GetAncestor(hwnd, GA_ROOT);

                if (hwnd == IntPtr.Zero)
                    return;

                // Effective DPI of the monitor containing the click point
                IntPtr hMon = MonitorFromPoint(clickPt, MONITOR_DEFAULTTONEAREST);
                uint dpiX = 96;
                try
                {
                    uint dpiY;
                    GetDpiForMonitor(hMon, MDT_EFFECTIVE_DPI, out dpiX, out dpiY);
                }
                catch { }
                if (dpiX == 0) dpiX = 96;

                // Border scaled proportionally to monitor DPI so it appears the same physical size
                // regardless of scaling setting (BorderAt96Dpi px at 96 DPI / 100 %).
                int border = (int)Math.Round(BorderAt96Dpi * (double)dpiX / 96.0);

                // DWMWA_EXTENDED_FRAME_BOUNDS returns the visible window rect, which excludes the
                // transparent resize shadow that GetWindowRect includes on the left, right, and bottom
                // but not the top.  Using the visible rect makes all four border strips equal width.
                RECT wr;
                if (DwmGetWindowAttribute(hwnd, DWMWA_EXTENDED_FRAME_BOUNDS,
                                          out wr, Marshal.SizeOf(typeof(RECT))) != 0)
                {
                    if (!GetWindowRect(hwnd, out wr))   // fallback for non-DWM windows
                        return;
                }

                var captureRect = new Rectangle(
                    wr.Left  - border, wr.Top    - border,
                    (wr.Right  - wr.Left) + border * 2,
                    (wr.Bottom - wr.Top)  + border * 2);

                // Clamp to the virtual desktop so maximised windows don't produce empty space
                captureRect.Intersect(VirtualScreen);

                if (captureRect.Width <= 0 || captureRect.Height <= 0)
                    return;

                var bmp = new Bitmap(captureRect.Width, captureRect.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                    g.CopyFromScreen(captureRect.Location, Point.Empty, captureRect.Size, CopyPixelOperation.SourceCopy);

                Clipboard.SetImage(bmp);
            }
            finally
            {
                _done = false;
                BeginWaiting();  // Return to idle – next Alt+P starts a new capture
            }
        }
    }
}

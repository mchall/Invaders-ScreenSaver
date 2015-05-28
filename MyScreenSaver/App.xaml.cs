using System;
using System.Windows;
using System.Windows.Interop;

namespace MyScreenSaver
{
    public partial class App : Application
    {
        private HwndSource _winWPFContent;
        private ScreenSaver _winSaver;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                ScreenSaver win = new ScreenSaver();
                win.WindowState = WindowState.Maximized;
                win.Topmost = true;
                win.Show();
                return;
            }

            if (e.Args[0].ToLower().StartsWith("/p"))
            {
                _winSaver = new ScreenSaver();

                Int32 previewHandle = Convert.ToInt32(e.Args[1]);
                IntPtr pPreviewHnd = new IntPtr(previewHandle);

                RECT lpRect = new RECT();
                bool bGetRect = Win32API.GetClientRect(pPreviewHnd, ref lpRect);

                HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams");

                sourceParams.PositionX = 0;
                sourceParams.PositionY = 0;
                sourceParams.Height = lpRect.Bottom - lpRect.Top;
                sourceParams.Width = lpRect.Right - lpRect.Left;
                sourceParams.ParentWindow = pPreviewHnd;
                sourceParams.WindowStyle = (int)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN);

                _winWPFContent = new HwndSource(sourceParams);
                _winWPFContent.Disposed += new EventHandler(WinWPFContent_Disposed);
                _winWPFContent.RootVisual = _winSaver.LayoutRoot;
            }
            else if (e.Args[0].ToLower().StartsWith("/s"))
            {
                ScreenSaver win = new ScreenSaver();
                win.WindowState = WindowState.Maximized;
                win.Show();
            }
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
                MessageBox.Show("No settings to configure");
                Application.Current.Shutdown();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void WinWPFContent_Disposed(object sender, EventArgs e)
        {
            _winSaver.Close();
        }
    }
}
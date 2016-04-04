using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wox.Plugin.ChangeTheme
{
    /// <summary>
    /// This static class can be used to manage themes in Windows 10 (not tested for other versions of Windows)
    /// It can retrieve all the themes
    /// It can switch the active theme
    /// 
    /// Thanks to this isopropanol for his answer : http://stackoverflow.com/questions/546818/how-do-i-change-the-current-windows-theme-programatically#answer-23205242
    /// author: seza443
    /// </summary>
    public static class ThemeManager
    {

        /// Handles to Win 32 API
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("dwmapi.dll")]
        public static extern IntPtr DwmIsCompositionEnabled(out bool pfEnabled);

        /// Windows Constants
        private const uint WM_CLOSE = 0x10;


        public static List<Theme> getAllThemesNames()
        {
            String localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            localAppDataPath += "\\Microsoft\\Windows\\Themes"; // Add the static path to the theme folder
            String extension = "*.theme";
            string[] themesArray = Directory.GetFiles(localAppDataPath, extension);

            List<Theme> result = new List<Theme>();
            foreach (string item in themesArray)
            {
                Theme th = new Theme();
                th.Path = item;
                th.Name = item.Substring(localAppDataPath.Length + 1, item.Length - localAppDataPath.Length - extension.Length);
                result.Add(th);
            }
            return result;
        }

        public static List<Theme> getFilteredThemesNames(String filter)
        {
            filter = filter.ToLower(); // to case insensitive comparison
            String localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            localAppDataPath += "\\Microsoft\\Windows\\Themes"; // Add the static path to the theme folder
            String extension = "*.theme";
            string[] themesArray = Directory.GetFiles(localAppDataPath, extension);

            List<Theme> result = new List<Theme>();
            foreach (string item in themesArray)
            {
                Theme th = new Theme();
                th.Path = item;
                th.Name = item.Substring(localAppDataPath.Length + 1, item.Length - localAppDataPath.Length - extension.Length);
                if (th.Name.ToLower().Contains(filter))
                {
                    result.Add(th);
                }
            }
            return result;
        }

        public static Boolean SwitchTheme(string themePath)
        {
            try
            {
                Process.Start(themePath);
                System.Threading.Thread.Sleep(1000);

                /// Close the Theme UI Window
                IntPtr hWndTheming = FindWindow("CabinetWClass", null);
                SendMessage(hWndTheming, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }//try
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured while setting the theme: " + ex.Message);

                return false;
            }//catch
            return true;
        }

        public static string GetActiveThemeName()
        {
            string RegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes";
            string theme;
            theme = (string)Registry.GetValue(RegistryKey, "CurrentTheme", string.Empty);
            theme = theme.Split('\\').Last().Split('.').First().ToString();
            return theme;
        }

        private static String StartProcessAndWait(string filename, string arguments, int seconds, ref Boolean bExited)
        {
            String msg = String.Empty;
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = arguments;
            p.Start();

            bExited = false;
            int counter = 0;
            /// give it "seconds" seconds to run
            while (!bExited && counter < seconds)
            {
                bExited = p.HasExited;
                counter++;
                System.Threading.Thread.Sleep(1000);
            }//while
            if (counter == seconds)
            {
                msg = "Program did not close in expected time.";
            }//if

            return msg;
        }
    }
}

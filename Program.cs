using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AltF4Blocker
{
    internal static class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private static int VK_LMENU = 0xA4; // LAlt
        private static int VK_RMENU = 0xA5; // RAlt
        private static int VK_F4 = 0x73;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static bool is_alt_pressed = false;
        private static Main main;
        private static NoNoPopup noNoPopup;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _hookID = SetHook(_proc);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            main = new Main();
            noNoPopup = new NoNoPopup();

            Application.Run(main);
            
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            int vkCode = Marshal.ReadInt32(lParam);
            if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                if (vkCode == VK_LMENU || vkCode == VK_RMENU)
                {
                    is_alt_pressed = true;
                }
                if (vkCode == VK_F4)
                { 
                    if (is_alt_pressed)
                    {
                        if (!noNoPopup.Visible)
                        {
                            if(noNoPopup.IsDisposed)
                            {
                                noNoPopup = new NoNoPopup();
                            }
                            noNoPopup.Show(main);
                        }
                        return (IntPtr)1;
                    }
                }
            }
            if (wParam == (IntPtr)WM_KEYUP)
            {
                if (vkCode == VK_LMENU || vkCode == VK_RMENU)
                {
                    is_alt_pressed = false;
                }
            }


            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}

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
        private const int VK_LMENU = 0xA4;
        private const int VK_F4 = 0x73;

        private static readonly LowLevelKeyboardProc hook_callback = HookCallback;
        private static IntPtr hook_id = IntPtr.Zero;

        private static bool is_alt_pressed = false;
        private static AltF4Blocker altf4_blocker;
        private static NoNoPopup no_no_popup;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            hook_id = SetHook(hook_callback);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            altf4_blocker = new AltF4Blocker();
            no_no_popup = new NoNoPopup();

            Application.Run(altf4_blocker);
            
            UnhookWindowsHookEx(hook_id);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc callback)
        {
            using (Process cur_process = Process.GetCurrentProcess())
            using (ProcessModule cur_module = cur_process.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, callback, GetModuleHandle(cur_module.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(hook_id, nCode, wParam, lParam);
            }

            int vk_code = Marshal.ReadInt32(lParam);
            if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                if (vk_code == VK_LMENU)
                {
                    is_alt_pressed = true;
                }
                if (vk_code == VK_F4)
                { 
                    if (is_alt_pressed)
                    {
                        if (!no_no_popup.Visible)
                        {
                            if(no_no_popup.IsDisposed)
                            {
                                no_no_popup = new NoNoPopup();
                            }
                            no_no_popup.Show(altf4_blocker);
                        }
                        return (IntPtr)1;
                    }
                }
            }
            if (wParam == (IntPtr)WM_KEYUP)
            {
                if (vk_code == VK_LMENU)
                {
                    is_alt_pressed = false;
                }
            }


            return CallNextHookEx(hook_id, nCode, wParam, lParam);
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

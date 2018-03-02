using System;
using System.Windows.Forms;

namespace mth.keylogger.forms
{
    public partial class LoggerTests : Form
    {
        public KeyboardHook KeyboardHook { get; set; }

        public LoggerTests()
        {
            InitializeComponent();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (this.KeyboardHook == null)
            {
                this.KeyboardHook = KeyboardHook.GetInstance();
                this.KeyboardHook.BufferChanged += _kbHook_BufferChanged;
            }

            if (!this.KeyboardHook.IsHooked)
            {
                this.KeyboardHook.Hook();
                this.btnIniciar.Text = "Detener";
            }
            else
            {
                this.KeyboardHook.UnHook();
                this.btnIniciar.Text = "Iniciar";
            }
        }

        public void _kbHook_BufferChanged(object sender, EventArgs e)
        {
            this.LogOutput.Text = KeyboardHook.Buffer;
        }
    }
}

using System;
using System.Windows.Forms;

namespace AltF4Blocker
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.TransparencyKey = this.BackColor;
        }
    }
}

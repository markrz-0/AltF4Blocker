using System;
using System.Windows.Forms;

namespace AltF4Blocker
{
    public partial class AltF4Blocker : Form
    {
        public AltF4Blocker()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.TransparencyKey = this.BackColor;
        }
    }
}

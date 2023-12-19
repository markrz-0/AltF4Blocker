using System;
using System.Windows.Forms;

namespace AltF4Blocker
{
    public partial class NoNoPopup : Form
    {

        public NoNoPopup()
        {
            InitializeComponent();
        }

        private void NoNoPopup_Load(object sender, EventArgs e)
        {
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;

            System.Timers.Timer timer = new System.Timers.Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    this.Close();
                }));
            }
            catch (Exception ex) { }
        }
    }
}

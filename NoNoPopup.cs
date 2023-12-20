using System;
using System.Drawing;
using System.Windows.Forms;

namespace AltF4Blocker
{
    public partial class NoNoPopup : Form
    {
        private readonly int HIDE_AFTER_MILIS = 2000;

        private System.Timers.Timer close_form_timer;
        private System.Windows.Forms.Timer change_labels_color_timer;

        public NoNoPopup()
        {
            InitializeComponent();
        }

        private void NoNoPopup_Load(object sender, EventArgs e)
        {
            this.TransparencyKey = this.BackColor;

            label.ForeColor = Color.Red;

            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;

            close_form_timer = new System.Timers.Timer(HIDE_AFTER_MILIS);
            close_form_timer.Elapsed += ClosePopup;
            close_form_timer.Start();

            change_labels_color_timer = new System.Windows.Forms.Timer();
            change_labels_color_timer.Interval = HIDE_AFTER_MILIS / 255;
            change_labels_color_timer.Tick += ChangeLabelsColorTick;
            change_labels_color_timer.Start();
        }

        private void ChangeLabelsColorTick(object sender, EventArgs e)
        {
            Color previous_color = label.ForeColor;
            label.ForeColor = Color.FromArgb(
                Math.Max(previous_color.R - 2, 0),
                Math.Min(previous_color.G + 2, 255),
                0);
        }

        private void ClosePopup(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    change_labels_color_timer.Stop();
                    this.Close();
                }));
            }
            catch (Exception ex) { }
        }
    }
}

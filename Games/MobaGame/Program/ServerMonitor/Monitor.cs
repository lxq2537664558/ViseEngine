using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerMonitor
{
    public partial class Monitor : Form
    {
        public Monitor()
        {
            InitializeComponent();
            ServerModule.Instance.Start();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {            
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;            
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;                
            }
        }

        private void ShowForm_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void HideForm_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void ExitForm_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;
            Application.ExitThread();
            ServerModule.Instance.Stop();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;            
            e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ServerModule.Instance.Tick();
        }

        private void Monitor_Load(object sender, EventArgs e)
        {
            
        }
    }
}

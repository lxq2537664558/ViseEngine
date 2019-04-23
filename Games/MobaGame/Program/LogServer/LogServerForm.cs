using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogServer
{
    public partial class LogServerForm : Form
    {
        public LogServerForm(string[] args)
        {
            InitializeComponent();
            ServerModule.Instance.Start(args);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "LogServer:" + ServerModule.Instance.LogServer.LinkState.ToString();
            ServerModule.Instance.Tick();
        }

        private void LogServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerModule.Instance.LogServer.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Log.FileLog.Instance.ReadLog();
        }
    }
}

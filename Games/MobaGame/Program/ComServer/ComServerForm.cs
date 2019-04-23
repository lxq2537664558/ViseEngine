using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComServer
{
    public partial class  ComServerForm : Form
    {
        public ComServerForm(string[] args)
        {
            InitializeComponent();
            ServerModule.Instance.Start(args);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "ComServer:" + ServerModule.Instance.ComServer.LinkState.ToString();
            ServerModule.Instance.Tick();
        }

        private void ComServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerModule.Instance.ComServer.Stop();
        }

        private void ComServerForm_Load(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = Server.ComServer.Instance;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Log.FileLog.Instance.ReadLog();
        }
    }
}

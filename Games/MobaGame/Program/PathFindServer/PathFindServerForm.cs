using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathFindServer
{
    public partial class PathFindServerForm : Form
    {
        public PathFindServerForm(string[] args)
        {
            InitializeComponent();
            ServerModule.Instance.Start(args);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Log.FileLog.Instance.ReadLog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "PathFindServer:" + ServerModule.Instance.PathServer.LinkState.ToString();
            ServerModule.Instance.Tick();
        }

        private void PathFindServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerModule.Instance.PathServer.Stop();
        }

        private void PathFindServerForm_Load(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = PathFindServer.ServerModule.Instance.PathServer;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GateServer
{
    public partial class GateServerForm : Form
    {
        public GateServerForm(string[] args)
        {
            InitializeComponent();
            ServerModule.Instance.Start(args);
            this.propertyGrid1.SelectedObject = ServerModule.Instance.GateServer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Log.FileLog.Instance.ReadLog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ServerModule.Instance.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServerModule.Instance.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "GateServer:" + ServerModule.Instance.GateServer.LinkState.ToString();
            //ServerModule.Instance.Tick();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();

            var gateServer = ServerModule.Instance.GateServer;

            TreeNode node = this.treeView1.Nodes.Add("Client");
            foreach (var elem in gateServer.ClientLinkers)
            {
                if (elem != null)
                {
                    var connect = elem.mForwardInfo.Gate2ClientConnect as SCore.TcpServer.TcpConnectHP;
                    node.Nodes.Add(connect.IpAddress + ":" + connect.Port).Tag = elem;
                }
            }
        }

        private void GateServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerModule.Instance.Stop();
        }
    }
}

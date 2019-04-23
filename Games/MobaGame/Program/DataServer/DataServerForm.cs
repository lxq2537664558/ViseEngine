using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataServer
{
    public partial class DataServerForm : Form
    {
        public DataServerForm(string[] args)
        {
            InitializeComponent();
            ServerModule.Instance.Start(args);

            this.propertyGrid1.SelectedObject = ServerModule.Instance.DataServer;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "DataServer:" + ServerModule.Instance.DataServer.LinkState.ToString();
            ServerModule.Instance.Tick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ServerModule.Instance.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();

            Server.DataServer dataServer = ServerModule.Instance.DataServer;

            TreeNode node = this.treeView1.Nodes.Add("GateServer");
            foreach (var elem in dataServer.GateServers)
            {
                node.Nodes.Add(elem.Value.IpAddress + ":" + elem.Value.Port).Tag = elem;
            }

            node = this.treeView1.Nodes.Add("PlanesServer");
            foreach (var elem in dataServer.HallServers)
            {
                node.Nodes.Add(elem.Value.EndPoint.IpAddress + ":" + elem.Value.EndPoint.Port).Tag = elem;
            }
        }

        private void DataServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerModule.Instance.Stop();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Log.FileLog.Instance.ReadLog();
        }
    }
}

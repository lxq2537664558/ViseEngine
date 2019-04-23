using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegServer
{
    public partial class RegServerForm : Form
    {
        public RegServerForm(string[] args)
        {
            InitializeComponent();
            ServerModule.Instance.Start(args);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = Log.FileLog.Instance.ReadLog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ServerModule.Instance.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServerModule.Instance.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();

            Server.RegisterServer regServer = ServerModule.Instance.RegServer;//.DataServer

            TreeNode node = this.treeView1.Nodes.Add("DataServer");
            if (null != regServer.DataServer)
                node.Nodes.Add(regServer.DataServer.IpAddress + ":" + regServer.DataServer.Port).Tag = regServer.DataServer;

            node = this.treeView1.Nodes.Add("ComServer");
            if (null != regServer.ComServer)
                node.Nodes.Add(regServer.ComServer.IpAddress + ":" + regServer.ComServer.Port).Tag = regServer.ComServer;

            node = this.treeView1.Nodes.Add("LogServer");
            if (null != regServer.LogServer)
                node.Nodes.Add(regServer.LogServer.IpAddress + ":" + regServer.LogServer.Port).Tag = regServer.LogServer;

            node = this.treeView1.Nodes.Add("GateServer");
            foreach (var ep in regServer.GateServers)
            {
                node.Nodes.Add(ep.Value.Ip + ":" + ep.Value.Port).Tag = ep.Value;
            }

            node = this.treeView1.Nodes.Add("HallServer");
            foreach (var ep in regServer.HallServers)
            {
                node.Nodes.Add(ep.Value.Ip + ":" + ep.Value.Port).Tag = ep.Value;
            }
            node = this.treeView1.Nodes.Add("PathFindServer");
            foreach (var ep in regServer.PathFindServers)
            {
                node.Nodes.Add(ep.Value.Ip + ":" + ep.Value.Port).Tag = ep.Value;
            }
        }

        private void RegServerForm_Load(object sender, EventArgs e)
        {

        }

        private void RegServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerModule.Instance.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "RegServer:" + ServerModule.Instance.RegServer.LinkState.ToString();
            ServerModule.Instance.Tick();
        }
    }
}

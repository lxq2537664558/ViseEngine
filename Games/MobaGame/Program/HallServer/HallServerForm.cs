using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HallServer
{
    public partial class HallServerForm : Form
    {
        public HallServerForm(string[] args)
        {
            InitializeComponent();
            ServerModule.Instance.Start(args);
            this.propertyGrid1.SelectedObject = ServerModule.Instance;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = Log.FileLog.Instance.ReadLog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var xml = RPC.RPCNetworkMgr.Instance.PrintAllRPCCounter();
            CSUtility.Support.XmlHolder.SaveXML("RPC_PlanesServer.xml", xml, true);

            var xml1 = RPC.RPCNetworkMgr.Instance.PrintAllCallCounter();
            CSUtility.Support.XmlHolder.SaveXML("Call_PlanesServer.xml", xml1, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();

            var server = ServerModule.Instance.HallServer;

            TreeNode node = this.treeView1.Nodes.Add("GateServer");
            foreach (var elem in server.GateServers)
            {
                node.Nodes.Add(elem.Value.IpAddress + ":" + elem.Value.Port).Tag = elem;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "HallServer:" + ServerModule.Instance.HallServer.LinkState.ToString();
            ServerModule.Instance.Tick();
        }

        private void HallServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerModule.Instance.HallServer.Stop();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPCCodeBuilder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : DockControl.Controls.DockAbleWindowBase
    {
        public MainWindow()
        {
            InitializeComponent();

            var clientWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "../../bin/DllWindow/ClientCommon.dll");
            CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Windows, "cscommon", clientWindowsAssembly);

            var serverWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "ServerCommon.dll");
            CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "cscommon", serverWindowsAssembly);
        }

        private void Button_Click_RefreshConfig(object sender, RoutedEventArgs e)
        {            
            Config.Instance.Load(Config.Instance.ConfigFile);
            comboBox_Project.ItemsSource = Config.Instance.ProjConfigList;
            comboBox_Project.DisplayMemberPath = "ProjectName";
            comboBox_Project.SelectedValuePath = "ProjectName";

            if (comboBox_Project.Items.Count > 0)
                comboBox_Project.SelectedIndex = 0;
        }
         
        private void Button_Click_EditProj(object sender, RoutedEventArgs e)
        {            
            var form = new EditDlg();
            form.listBox.ItemsSource = Config.Instance.ProjConfigList;
            form.listBox.DisplayMemberPath = "ProjectName";
            form.RefreshProject = Button_Click_RefreshConfig;
            form.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Button_Click_RefreshConfig(null,null);
        }

        private void comboBox_Project_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.Instance.SetCurProject(comboBox_Project.SelectedValue.ToString());
        }

        private void DockAbleWindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current?.Shutdown();
        }

        private void Button_Click_Server(object sender, RoutedEventArgs e)
        {
            //读取上次CSharp代码生成配置
            if (Config.Instance.CurProjectConfig == null)
                return;
                        
            string[] segs = Config.Instance.CurProjectConfig.ServerModuleName.Split(';');
            List<string> modules = new List<string>();
            foreach (var i in segs)
            {
                modules.Add(i);
            }
            MakeCodeFromModule(modules, Config.Instance.CurProjectConfig.ServerCallerModuleName, true);

            this.textBox_Caller.Text = Config.Instance.CurProjectConfig.ServerCaller;
            this.textBox_Callee.Text = Config.Instance.CurProjectConfig.ServerCallee;
        }

        private void Button_Click_Client(object sender, RoutedEventArgs e)
        {
            //读取上次C++代码生成配置

            if (Config.Instance.CurProjectConfig == null)
                return;
            
            //CppMakeCodeFromFolder(Config.Instance.CurProjectConfig.CppFolder);
            string[] segs = Config.Instance.CurProjectConfig.ClientModuleName.Split(';');
            List<string> lst = new List<string>();
            foreach (var i in segs)
            {
                lst.Add(i);
            }

            MakeCodeFromModule(lst, Config.Instance.CurProjectConfig.ClientCallerModuleName, false);

            this.textBox_Caller.Text = Config.Instance.CurProjectConfig.ClientCaller;
            this.textBox_Callee.Text = Config.Instance.CurProjectConfig.ClientCallee;
        }

        object mCurVersionManager;
        List<object> mAddList = new List<object>();
        List<object> mCurList = new List<object>();
        void MakeCodeFromModule(List<string> modules, string moduleCaller, bool bServer)
        {
            if (modules.Count == 0)
                return;

            this.treeView.Items.Clear();

            {
                System.Type verType;
                System.Reflection.Assembly assembly;
                if (bServer)
                {
                    assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, moduleCaller);
                    verType = assembly.GetType("RPCServerVersion");
                }
                else
                {
                    assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, moduleCaller);
                    verType = assembly.GetType("RPCClientVersion");
                }

                if (verType == null)
                {
                    mCurVersionManager = new RPC.RPCVersionManager();
                }
                else
                {
                    var met = verType.GetMethod("GetManager");
                    mCurVersionManager = met.Invoke(null, null);
                }

                mAddList.Clear();
                mCurList.Clear();
            }

            foreach (var moduleName in modules)
            {
                System.Reflection.Assembly assembly;
                if(bServer)
                    assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, moduleName);
                else
                    assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, moduleName);

                if (assembly == null)
                    continue;

                //RPC.RPCEntrance.BuildRPCMethordExecuter(pAssem);                
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(t, typeof(RPC.RPCClassAttribute).FullName, false);                    
                    if (att != null)
                    {
                        var rpcType = (Type)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "RPCType");
                        TreeViewItem cNode = new TreeViewItem()
                        {
                            Header = t.FullName,
                            Tag = rpcType,      
                            Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemStyle_Default")) as System.Windows.Style,                                  
                        };                        
                        this.treeView.Items.Add(cNode);

                        System.Reflection.PropertyInfo[] props = t.GetProperties();
                        foreach (System.Reflection.PropertyInfo p in props)
                        {
                            if (p.Name == "Item")
                            {
                                var propAtt = CSUtility.Helper.AttributeHelper.GetCustomAttribute(p, typeof(RPC.RPCIndexObjectAttribute).FullName, false);                                
                                if (propAtt != null)
                                {
                                    var childIndex = (int)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(propAtt, "ChildIndex");
                                    TreeViewItem coNode = new TreeViewItem()
                                    {
                                        Header = "(I:" + childIndex.ToString() + ")" + p.Name,
                                        Tag = p,
                                        Name = "ChildIndex" + childIndex.ToString(),
                                        Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemStyle_Default")) as System.Windows.Style,
                                    };                                    
                                    cNode.Items.Add(coNode);                                                                        
                                }
                            }
                            else
                            {
                                var propAtt = CSUtility.Helper.AttributeHelper.GetCustomAttribute(p, typeof(RPC.RPCChildObjectAttribute).FullName, false);                                
                                if (propAtt != null)
                                {
                                    var childIndex = (int)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(propAtt, "ChildIndex");
                                    TreeViewItem coNode = new TreeViewItem()
                                    {
                                        Header = "(P:" + childIndex.ToString() + ")" + p.Name,
                                        Tag = p,
                                        Name = "ChildIndex" + childIndex.ToString(),
                                        Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemStyle_Default")) as System.Windows.Style,
                                    };                                    
                                    cNode.Items.Add(coNode);                                                                        
                                }
                            }
                        }
                        {
                            System.Reflection.MethodInfo[] methords = t.GetMethods();
                            foreach (System.Reflection.MethodInfo m in methords)
                            {
                                var propAtt = CSUtility.Helper.AttributeHelper.GetCustomAttribute(m, typeof(RPC.RPCMethodAttribute).FullName, false);                                
                                if (propAtt != null)
                                {
                                    var isWeakPkg = (bool)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(propAtt, "IsWeakPkg");
                                    var noClientCall = (bool)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(propAtt, "NoClientCall");

                                    MethodDesc desc = new MethodDesc();
                                    desc.IsWeakPkg = isWeakPkg;
                                    desc.NoClientCall = noClientCall;
                                    desc.mi = m;                                    
                                    desc.hostType = t;
                                    desc.HashcodeOfMethod = RPC.RPCEntrance.GetMethodHashCode(m, t.FullName);
                                    bool bAdd = false;
                                    var GetMethodIndexMethod = mCurVersionManager.GetType().GetMethod("GetMethodIndex");
                                    var rmi = GetMethodIndexMethod.Invoke(mCurVersionManager, new object[] { t.FullName, desc.HashcodeOfMethod, bAdd });
                                    //RPC.RPCMethodInfo rmi = mCurVersionManager .GetMethodIndex(t.FullName, desc.HashcodeOfMethod, out bAdd);
                                    var fieldFullName = rmi.GetType().GetField("FullName");
                                    fieldFullName.SetValue(rmi, t.FullName + "->" + m.Name);
                                    var fieldIndex = rmi.GetType().GetField("Index");
                                    desc.MethodIndex = (byte)fieldIndex.GetValue(rmi);
                                    TreeViewItem cmNode = new TreeViewItem()
                                    {
                                        Header = m.Name,
                                        Tag = desc,
                                        Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemStyle_Default")) as System.Windows.Style,
                                    };                                    
                                    cNode.Items.Add(cmNode);
                                    
                                    mCurList.Add(rmi);
                                    if (bAdd)
                                    {
                                        mAddList.Add(rmi);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (mAddList.Count > 0)
            {
                string msg = "新增RPC:\n";
                foreach (var t in mAddList)
                {
                    var fieldFullName = t.GetType().GetField("FullName");                    
                    msg += fieldFullName.GetValue(t) + "\n";
                }
                MessageBox.Show(msg);
            }

            var method = mCurVersionManager.GetType().GetMethod("GetMethods");
            var allMth = method.Invoke(mCurVersionManager, null) as IEnumerable;
            var removeMethod = allMth.GetType().GetMethod("Remove");
            foreach (var t in mCurList)
            {
                //allMth.Remove(t);
                removeMethod.Invoke(allMth, new object[] { t });
            }

            var proCount = allMth.GetType().GetProperty("Count");
            int count = (int)proCount.GetValue(allMth, null);
            if (count > 0)
            {
                string msg = "删除RPC:\n";
                foreach (var d in allMth)
                {
                    var proValue = d.GetType().GetProperty("Value");
                    var valueObj = proValue.GetValue(d);
                    if(valueObj != null)
                    {
                        var proFullName = valueObj.GetType().GetField("FullName");
                        msg += proFullName.GetValue(valueObj) + "\n";
                    }
                }
                MessageBox.Show(msg);
            }

            ServerMakeCode(bServer);
        }

        private void ServerMakeCode(bool bServer)
        {
            //  Caller
            this.textBoxCaller.Text = "//Server Caller\r\n";
            foreach (TreeViewItem node in this.treeView.Items)
            {
                this.textBoxCaller.Text += RPCClassBuilder.MakeClientClassCode(node);
            }

            if (bServer)
                this.textBoxCaller.Text += RPCClassBuilder.MakeMethodIndexRegisterCode(mCurList, "RPCServerVersion");
            else
                this.textBoxCaller.Text += RPCClassBuilder.MakeMethodIndexRegisterCode(mCurList, "RPCClientVersion");

            //  Callee
            this.textBoxCallee.Text = "//Server Callee\r\n";
            this.textBoxCallee.Text += "namespace RPC_ExecuterNamespace{" + "\r\n";
            string mappingCode = "";
            foreach (TreeViewItem node in this.treeView.Items)
            {
                var rpcType = node.Tag as Type;
                if (rpcType == null)
                    continue;
                foreach (TreeViewItem mnode in node.Items)
                {
                    MethodDesc mi = mnode.Tag as MethodDesc;
                    if (mi != null)
                    {
                        this.textBoxCallee.Text += RPCClassBuilder.MakeMethodExecuterCode(mi, bServer);//RPCClassBuilder.MakeMethodExecuterCode(mi, ca.RPCType);
                        mappingCode += "    RPC.RPCNetworkMgr.AddExecuterIndxer(" + mi.HashcodeOfMethod + " , " + mi.MethodIndex + ");\r\n";
                    }
                    else
                    {
                        System.Reflection.PropertyInfo pi = mnode.Tag as System.Reflection.PropertyInfo;
                        if (pi != null && pi.Name == "Item")
                        {
                            this.textBoxCallee.Text += RPCClassBuilder.MakeIndexerExecuterCode(pi, rpcType,bServer);
                        }
                    }
                }
            }

            this.textBoxCallee.Text += "public class MappingHashCode2Index{\r\n";

            this.textBoxCallee.Text += "public static void BuildMapping(){\r\n";
            this.textBoxCallee.Text += mappingCode;
            this.textBoxCallee.Text += "}\r\n";

            this.textBoxCallee.Text += "}\r\n\r\n";

            this.textBoxCallee.Text += "}" + "\r\n";
        }

        private void Button_Click_GenerateCode(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(this.textBox_Callee.Text))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(this.textBox_Callee.Text);
                if (sw != null)
                {
                    sw.Write(this.textBoxCallee.Text);
                    sw.Close();
                }
            }
            if (System.IO.File.Exists(this.textBox_Caller.Text))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(this.textBox_Caller.Text);
                if (sw != null)
                {
                    sw.Write(this.textBoxCaller.Text);
                    sw.Close();
                }
            }
        }

        private void Button_Click_SaveConfig(object sender, RoutedEventArgs e)
        {
            Config.Instance.Save(Config.Instance.ConfigFile);
        }
    }
}

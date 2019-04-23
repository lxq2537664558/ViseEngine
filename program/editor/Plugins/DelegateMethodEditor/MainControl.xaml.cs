using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace DelegateMethodEditor
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "DelegateMethodEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/逻辑图")]
    [Guid("B0EF83B4-4310-48B2-B593-4350A9D9A7DE")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public string PluginName
        {
            get { return "逻辑图"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "逻辑图",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {

        }

        Dictionary<string, List<Type>> mDelegateTypeDictionary = new Dictionary<string, List<Type>>();

        //string mAvailableDelegateFilter = "";
        //public string AvailableDelegateFilter
        //{
        //    get { return mAvailableDelegateFilter; }
        //    set
        //    {
        //        mAvailableDelegateFilter = value;

        //        var strLower = mAvailableDelegateFilter.ToLower();
        //        foreach (ListBoxItem item in ListBox_Available.Items)
        //        {
        //            if (string.IsNullOrEmpty(mAvailableDelegateFilter))
        //                item.Visibility = Visibility.Visible;
        //            else
        //            {
        //                if (!item.Content.ToString().ToLower().Contains(strLower))
        //                    item.Visibility = Visibility.Collapsed;
        //                else
        //                    item.Visibility = Visibility.Visible;
        //            }
        //        }

        //        OnPropertyChanged("AvailableDelegateFilter");
        //    }
        //}

        EventListItem mSelectedItem = null;

        public MainControl()
        {
            InitializeComponent();

            var template = this.TryFindResource("EventSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("EventSet", template);

            // 注册分析的dll文件
            CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Common, CSUtility.enPlatform.Windows, "DelegateMethodEditor", this.GetType().Assembly);

            EvtListControl.OnSelectionChanged = new EventListControl.Delegate_OnSelectionChanged(OnEventListControlSelectionChanged);
            InitializeTypesComboBox();

            NodesContainerControl_Common.OnPreviewCode = PreViewCode;
            NodesContainerControl_Common.OnSave = _OnNodesContainerSave;
            NodesContainerControl_Common.CodeViewer.CSType = CSUtility.Helper.enCSType.Common;
            NodesContainerControl_Client.OnPreviewCode = PreViewCode;
            NodesContainerControl_Client.OnSave = _OnNodesContainerSave;
            NodesContainerControl_Client.CodeViewer.CSType = CSUtility.Helper.enCSType.Client;
            NodesContainerControl_Server.OnPreviewCode = PreViewCode;
            NodesContainerControl_Server.OnSave = _OnNodesContainerSave;
            NodesContainerControl_Server.CodeViewer.CSType = CSUtility.Helper.enCSType.Server;
        }

        private void _OnNodesContainerSave()
        {
            Save(mSelectedItem);
        }

        bool mInitialized = false;
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!mInitialized)
            {
                NodesContainerControl_Common.Initialize(CSUtility.Helper.enCSType.Common);
                NodesContainerControl_Client.Initialize(CSUtility.Helper.enCSType.Client);
                NodesContainerControl_Server.Initialize(CSUtility.Helper.enCSType.Server);

                mInitialized = true;
            }
        }

        private void OnEventListControlSelectionChanged(EventListItem item)
        {
            if (mSelectedItem != null)
            {
                Save(mSelectedItem);
                NodesContainerControl_Common.OnDirtyChanged -= mSelectedItem.OnCommonLinkControlDirtyChanged;
                NodesContainerControl_Client.OnDirtyChanged -= mSelectedItem.OnClientLinkControlDirtyChanged;
                NodesContainerControl_Server.OnDirtyChanged -= mSelectedItem.OnServerLinkControlDirtyChanged;
            }

            mSelectedItem = item;
            Program.SelectedEventListItem = item;

            NodesContainerControl_Common.OnContainLinkNodesChanged = item.OnContainLinkNodesChanged;
            NodesContainerControl_Common.OnDirtyChanged += item.OnCommonLinkControlDirtyChanged;

            NodesContainerControl_Client.OnContainLinkNodesChanged = item.OnContainLinkNodesChanged;
            NodesContainerControl_Client.OnDirtyChanged += item.OnClientLinkControlDirtyChanged;

            NodesContainerControl_Server.OnContainLinkNodesChanged = item.OnContainLinkNodesChanged;
            NodesContainerControl_Server.OnDirtyChanged += item.OnServerLinkControlDirtyChanged;

            TryLoad(item);
        }

        private void InitializeTypesComboBox()
        {
            mDelegateTypeDictionary.Clear();
            ComboBox_Types.Items.Clear();

            foreach (var classType in CSUtility.Program.GetTypes(CSUtility.Helper.enCSType.All))
            {
                var atts = classType.GetCustomAttributes(typeof(CSUtility.Editor.DelegateMethodEditor_AllowedDelegate), true);
                if (atts.Length == 0)
                    continue;

                var str = ((CSUtility.Editor.DelegateMethodEditor_AllowedDelegate)atts[0]).TypeStr;

                List<Type> tempTypeList;
                if(!mDelegateTypeDictionary.TryGetValue(str, out tempTypeList))
                {
                    tempTypeList = new List<Type>();
                    tempTypeList.Add(classType);
                    mDelegateTypeDictionary[str] = tempTypeList;

                    ComboBox_Types.Items.Add(str);
                }
                else
                    tempTypeList.Add(classType);
            }

            if (ComboBox_Types.Items.Count > 0)
                ComboBox_Types.SelectedIndex = 0;
        }
        
        private void ComboBox_Types_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Types.SelectedIndex < 0)
                return;

            List<Type> tempTypeList;
            if (!mDelegateTypeDictionary.TryGetValue(ComboBox_Types.SelectedValue.ToString(), out tempTypeList))
                return;

            ComboBox_Methods.Items.Clear();
            foreach (var type in tempTypeList)
            {
                var item = new TextBlock();
                item.Text = type.FullName;
                item.Tag = type;

                var atts = type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                if(atts.Length > 0)
                {
                    item.ToolTip = ((System.ComponentModel.DescriptionAttribute)atts[0]).Description;
                }
                ComboBox_Methods.Items.Add(item);
            }
        }
        private void ComboBox_Methods_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Methods.SelectedIndex < 0)
                return;

            if (mSelectedItem != null)
            {
                Save(mSelectedItem);
                mSelectedItem = null;
            }

            EvtListControl.DelegateType = ((TextBlock)ComboBox_Methods.SelectedItem).Tag as Type;

            NodesContainerControl_Common.ClearControlNodes();
            NodesContainerControl_Client.ClearControlNodes();
            NodesContainerControl_Server.ClearControlNodes();
        }
        

#region 保存读取

        private bool TryLoad(EventListItem item)
        {
            if (item == null)
                return false;

            if (item.EventId == Guid.Empty)
                return false;

            var dir = CSUtility.Support.IFileConfig.DefaultEventDirectory + "\\" + item.EventId.ToString();

            var eventFileName = dir + "\\" + item.EventId.ToString() + ".xml";
            if (System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + eventFileName))
            {
                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(eventFileName);
                item.Load(xmlHolder, CSUtility.Helper.enCSType.Client);
            }

            var commonFileName = dir + "\\" + EventListItem.GetFileName(item.EventId, CSUtility.Helper.enCSType.Common);
            if (System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + commonFileName))
            {
                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(commonFileName);
                if (xmlHolder != null)
                {
                    NodesContainerControl_Common.LoadXML(xmlHolder);
                }
            }
            else
            {
                NodesContainerControl_Common.ClearControlNodes();
                var methodNode = NodesContainerControl_Common.AddOrigionNode(typeof(CodeDomNode.MethodNode), CodeDomNode.MethodNode.GetParamInMethodInfo(item.MethodInfo) + ",true,false", 0, 0) as CodeDomNode.MethodNode;
                if (methodNode != null)
                {
                    methodNode.IsStatic = true;
                    methodNode.UseBaseVisibility = Visibility.Collapsed;
                }
            }

            var clientFileName = dir + "\\" + EventListItem.GetFileName(item.EventId, CSUtility.Helper.enCSType.Client);
            if (System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + clientFileName))
            {
                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(clientFileName);
                if (xmlHolder != null)
                {
                    NodesContainerControl_Client.LoadXML(xmlHolder);
                }
            }
            else
            {
                NodesContainerControl_Client.ClearControlNodes();
                var methodNode = NodesContainerControl_Client.AddOrigionNode(typeof(CodeDomNode.MethodNode), CodeDomNode.MethodNode.GetParamInMethodInfo(item.MethodInfo) + ",true,false", 0, 0) as CodeDomNode.MethodNode;
                if (methodNode != null)
                {
                    methodNode.IsStatic = true;
                    methodNode.UseBaseVisibility = Visibility.Collapsed;
                }
            }

            var serverFileName = dir + "\\" + EventListItem.GetFileName(item.EventId, CSUtility.Helper.enCSType.Server);
            if (System.IO.File.Exists(CSUtility.Support.IFileManager.Instance.Root + serverFileName))
            {
                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(serverFileName);
                if (xmlHolder != null)
                {
                    NodesContainerControl_Server.LoadXML(xmlHolder);
                }
            }
            else
            {
                NodesContainerControl_Server.ClearControlNodes();
                var methodNode = NodesContainerControl_Server.AddOrigionNode(typeof(CodeDomNode.MethodNode), CodeDomNode.MethodNode.GetParamInMethodInfo(item.MethodInfo) + ",true,false", 0, 0) as CodeDomNode.MethodNode;
                if (methodNode != null)
                {
                    methodNode.IsStatic = true;
                    methodNode.UseBaseVisibility = Visibility.Collapsed;
                }
            }

            return true;
        }

        private bool Save(EventListItem item)
        {
            if (item == null)
                return false;

            if (item.EventId == Guid.Empty)
                return false;

            var dir = CSUtility.Support.IFileConfig.DefaultEventDirectory;
            var absDir = CSUtility.Support.IFileManager.Instance.Root + dir;
            if (!System.IO.Directory.Exists(absDir))
            {
                System.IO.Directory.CreateDirectory(absDir);

                EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} 目录{dir}使用版本控制添加失败!");
                    }
                }, absDir, $"AutoCommit 增加逻辑图{item.NickName}", true);
            }

            dir += "\\" + item.EventId.ToString();
            absDir += "\\" + item.EventId.ToString();
            if (!System.IO.Directory.Exists(absDir))
            {
                System.IO.Directory.CreateDirectory(absDir);

                //SvnInterface.Commander.Instance.AddFolder(absDir);
                //SvnInterface.Commander.Instance.Commit(absDir);
            }

            if(item.IsDirty)
            {
                if (!CheckCompile())
                    return false;

                var fileName = dir + "\\" + item.EventId.ToString() + ".xml";
                var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Event", "");

                // 版本更新
                CSUtility.Helper.EventCallBackVersionManager.Instance.VersionUpdate(item.EventId, CSUtility.Helper.enCSType.Client);
                item.EventCallBack.Version = CSUtility.Helper.EventCallBackVersionManager.Instance.GetVersion(item.EventId, CSUtility.Helper.enCSType.Client);

                item.Save(xmlHolder, CSUtility.Helper.enCSType.Client);
                CSUtility.Support.XmlHolder.SaveXML(fileName, xmlHolder, true);

                /*/ SVN
                {
                    var absFile = CSUtility.Support.IFileManager.Instance.Root + fileName;
                    var status = SvnInterface.Commander.Instance.CheckStatusEx(absFile);
                    switch (status)
                    {
                        case SvnInterface.SvnStatus.NotControl:
                            SvnInterface.Commander.Instance.Add(absFile);
                            break;
                    }

                    SvnInterface.Commander.Instance.Commit(absFile);
                }//*/

                item.IsDirty = false;
            }

            if (item.CommonIsDirty)
            {
                if (item.CommonHasLinks)
                {
                    var fileName = dir + "\\" + EventListItem.GetFileName(item.EventId, CSUtility.Helper.enCSType.Common);

                    var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Event", "");
                    NodesContainerControl_Common.SaveXML(xmlHolder);
                    CSUtility.Support.XmlHolder.SaveXML(fileName, xmlHolder, true); 
               
                    // SVN
                    {
                        var absFile = CSUtility.Support.IFileManager.Instance.Root + fileName;

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFile}使用版本控制上传失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                                {
                                    if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFile}使用版本控制上传失败!");
                                    }
                                }, absFile, $"AutoCommit 上传逻辑图{item.NickName}公共部分");
                            }
                        }, absFile);
                    }//*/

                    if (!item.ClientHasLinks)
                    {
                        var tw = CodeGenerator.CodeGenerator.GenerateCode(item.EventCallBack, NodesContainerControl_Common, CSUtility.Helper.enCSType.Client);

                        // 生成客户端dll
                    }
                    if (!item.ServerHasLinks)
                    {

                        // 生成服务器端dll
                    }
                }
                else
                {
                    // 删除文件
                    var absFileName = CSUtility.Support.IFileManager.Instance.Root + dir + "\\" + EventListItem.GetFileName(item.EventId, CSUtility.Helper.enCSType.Common);
                    if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFileName}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                                {
                                    if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFileName}使用版本控制删除失败!");
                                    }
                                }, absFileName, $"AutoCommit 删除逻辑图{item.NickName}公共部分");
                            }
                        }, absFileName);
                    }
                    else
                    {
                        System.IO.File.Delete(absFileName);
                    }

                    if (!item.ClientHasLinks)
                    {

                        // 删除客户端dll
                    }
                    if (!item.ServerHasLinks)
                    {

                        // 删除服务器端dll
                    }
                }

                // 全部保存完毕以及生成dll成功后
                item.CommonIsDirty = false;
            }
            if (item.ClientIsDirty)
            {
                var fileName = dir + "\\" + EventListItem.GetFileName(item.EventId, CSUtility.Helper.enCSType.Client);

                if (item.ClientHasLinks)
                {
                    var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Event", "");
                    NodesContainerControl_Client.SaveXML(xmlHolder);
                    CSUtility.Support.XmlHolder.SaveXML(fileName, xmlHolder, true);

                    // SVN
                    {
                        var absFile = CSUtility.Support.IFileManager.Instance.Root + fileName;

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFile}使用版本控制上传失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                                {
                                    if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFile}使用版本控制上传失败!");
                                    }
                                }, absFile, $"AutoCommit 上传逻辑图{item.NickName}客户端部分");
                            }
                        }, absFile);
                    }//*/

                    // 生成客户端dll
                }
                else
                {
                    var absFile = CSUtility.Support.IFileManager.Instance.Root + fileName;


                    if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                            {
                                if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                {
                                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFile}使用版本控制删除失败!");
                                }
                            }, absFile, $"AutoCommit 删除逻辑图{item.NickName}客户端部分");
                        }, absFile);
                    }
                    else
                    {
                        System.IO.File.Delete(absFile);
                    }

                    // 删除客户端dll
                }

                // 全部保存完毕以及生成dll成功后
                item.ClientIsDirty = false;
            }
            if (item.ServerIsDirty)
            {
                var fileName = dir + "\\" + EventListItem.GetFileName(item.EventId, CSUtility.Helper.enCSType.Server);

                if (item.ServerHasLinks)
                {
                    var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Event", "");
                    NodesContainerControl_Server.SaveXML(xmlHolder);
                    CSUtility.Support.XmlHolder.SaveXML(fileName, xmlHolder, true);

                    // SVN
                    {
                        var absFile = CSUtility.Support.IFileManager.Instance.Root + fileName;

                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFile}使用版本控制上传失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                                {
                                    if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFile}使用版本控制上传失败!");
                                    }
                                }, absFile, $"AutoCommit 上传逻辑图{item.NickName}服务器部分");
                            }
                        }, absFile);
                    }//*/

                    // 生成服务器端dll

                }
                else
                {
                    // 删除文件
                    var absFileName = CSUtility.Support.IFileManager.Instance.Root + fileName;
                    if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFileName}使用版本控制删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                                {
                                    if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, $"逻辑图{item.NickName} {absFileName}使用版本控制删除失败!");
                                    }
                                }, absFileName, $"AutoCommit 删除逻辑图{item.NickName}服务器部分");
                            }
                        }, absFileName);
                    }
                    else
                    {
                        System.IO.File.Delete(absFileName);
                    }

                    // 删除服务器端dll
                }

                // 全部保存完毕以及生成dll成功后
                item.ServerIsDirty = false;
            }

            // 客户端和服务器刷新新版本event
            CSUtility.Helper.EventCallBackManager.Instance.UpdateEventWithServer(item.EventId);

            return true;
        }

#endregion

#region 代码预览
        
        private void PreViewCode()
        {
            if(mSelectedItem == null)
                return;
                        
            NodesContainerControl_Client.CodeViewer.EventId = mSelectedItem.EventId;
            NodesContainerControl_Server.CodeViewer.EventId = mSelectedItem.EventId;
            System.IO.TextWriter tw;

            if (mSelectedItem.ClientHasLinks)
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Client, CSUtility.Helper.enCSType.Client);
            else
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Common, CSUtility.Helper.enCSType.Client);
            NodesContainerControl_Client.CodeViewer.Text_Code = tw.ToString();

            if(mSelectedItem.ServerHasLinks)
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Server, CSUtility.Helper.enCSType.Server);
            else
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Common, CSUtility.Helper.enCSType.Server);
            NodesContainerControl_Server.CodeViewer.Text_Code = tw.ToString();
        }

        private bool CheckCompile()
        {
            var clientDllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EventDlls_Client_Directory;
            var serverDllDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EventDlls_Server_Directory;
            if (!System.IO.Directory.Exists(clientDllDir))
            {
                System.IO.Directory.CreateDirectory(clientDllDir);
            }
            if (!System.IO.Directory.Exists(serverDllDir))
            {
                System.IO.Directory.CreateDirectory(serverDllDir);
            }

            System.IO.TextWriter tw;

            if (mSelectedItem.ClientHasLinks)
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Client, CSUtility.Helper.enCSType.Client);
            else
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Common, CSUtility.Helper.enCSType.Client);
            
            var dllFile = clientDllDir + "\\" + CSUtility.Helper.EventCallBack.GetAssemblyFileName(mSelectedItem.EventId, CSUtility.Helper.enCSType.Client);
            var compileResult = DelegateMethodEditor.CodeGenerator.CodeGenerator.CompileCode(tw.ToString(), CSUtility.Helper.enCSType.Client, mSelectedItem.EventId,dllFile);
            if (compileResult.Errors.HasErrors)
            {
                EditorCommon.MessageBox.Show("客户端生成代码编译未通过，请查看代码生成结果!");
                PreViewCode();
                return false;
            }

            if (mSelectedItem.ServerHasLinks)
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Server, CSUtility.Helper.enCSType.Server);
            else
                tw = DelegateMethodEditor.CodeGenerator.CodeGenerator.GenerateCode(mSelectedItem.EventCallBack, NodesContainerControl_Common, CSUtility.Helper.enCSType.Server);

            dllFile = serverDllDir + "\\" + CSUtility.Helper.EventCallBack.GetAssemblyFileName(mSelectedItem.EventId, CSUtility.Helper.enCSType.Server);
            compileResult = DelegateMethodEditor.CodeGenerator.CodeGenerator.CompileCode(tw.ToString(), CSUtility.Helper.enCSType.Server, mSelectedItem.EventId,dllFile);
            if (compileResult.Errors.HasErrors)
            {
                EditorCommon.MessageBox.Show("服务器端生成代码编译未通过，请查看代码生成结果!");
                PreViewCode();
                return false;
            }

            return true;
        }

#endregion

#region 资源反查

        public void SelectedEvent(Type delegateType, Guid eventId)
        {
            if (delegateType == null || eventId == Guid.Empty)
                return;

            var atts = delegateType.GetCustomAttributes(typeof(CSUtility.Editor.DelegateMethodEditor_AllowedDelegate), true);
            if (atts.Length == 0)
                return;

            var att = atts[0] as CSUtility.Editor.DelegateMethodEditor_AllowedDelegate;
            ComboBox_Types.SelectedValue = att.TypeStr;

            List<Type> tempTypeList;
            if (!mDelegateTypeDictionary.TryGetValue(ComboBox_Types.SelectedValue.ToString(), out tempTypeList))
                return;

            for (int i = 0; i < tempTypeList.Count; i++)
            {
                if (tempTypeList[i] == delegateType)
                {
                    ComboBox_Methods.SelectedIndex = i;
                    break;
                }
            }

            EvtListControl.SelectedEvent(eventId);
        }

#endregion

    }
}

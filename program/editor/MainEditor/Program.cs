using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace MainEditorAttribute
{
    // 控制函数显示
    public sealed class UserControlAttribute : System.Attribute
    {
        private string mControlName;
        public string ControlName
        {
            get { return mControlName; }
        }

        private bool mMultiInstance = false;
        public bool MultiInstance
        {
            get { return mMultiInstance; }
        }

        public UserControlAttribute(string ctrlName, bool multiInstance = false)
        {
            mControlName = ctrlName;
            mMultiInstance = multiInstance;
        }
    }

    public sealed class PanelAttribute : System.Attribute
    {
        private string mControlName;
        public string ControlName
        {
            get { return mControlName; }
        }

        public PanelAttribute(string ctrlName)
        {
            mControlName = ctrlName;
        }
    }
}

namespace MainEditor
{
    public class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

#region 删除文件处理，主要解决文件被占用的情况

        // 保存需要删除的文件（绝对路径）
        protected static List<string> mDeleteFileList = new List<string>();
        public static void AddDeleteFile(string fileName)
        {
            if (!mDeleteFileList.Contains(fileName))
                mDeleteFileList.Add(fileName);

            SaveDeleteFileList();
        }
        public static void DelDeleteFile(string fileName)
        {
            mDeleteFileList.Remove(fileName);

            SaveDeleteFileList();
        }
        public static void DelFileInFileList()
        {
            for (int i = mDeleteFileList.Count - 1; i >= 0; i--)
            {
                if (System.IO.File.Exists(mDeleteFileList[i]))
                {
                    try
                    {
                        // todo: SVN处理

                        System.IO.File.Delete(mDeleteFileList[i]);
                        mDeleteFileList.RemoveAt(i);
                    }
                    catch (System.Exception ex)
                    {
                        EditorCommon.MessageBox.Show("Program.DelFileInFileList exception\r\n" + ex.Message);
                    }
                }
            }

            SaveDeleteFileList();
        }
        public static void SaveDeleteFileList()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("DeleteFiles", "");
            foreach (var file in mDeleteFileList)
            {
                var node = xmlHolder.RootNode.AddNode("File", "",xmlHolder);
                node.AddAttrib("Value", file);
            }

            CSUtility.Support.XmlHolder.SaveXML(CSUtility.Support.IFileConfig.EditorSourceDirectory + "DeleteFiles.xml", xmlHolder, true);
        }
        public static void LoadDeleteFileList()
        {
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(CSUtility.Support.IFileConfig.EditorSourceDirectory + "DeleteFiles.xml");
            if (xmlHolder == null || xmlHolder.RootNode == null)
                return;

            var nodes = xmlHolder.RootNode.FindNodes("File");
            foreach (var node in nodes)
            {
                var att = node.FindAttrib("Value");
                if (att == null)
                    continue;

                mDeleteFileList.Add(att.Value);
            }
        }

#endregion
        

#region PluginProcess

        // 
        public static void OnOpenEditor(object[] obj)
        {
            if (obj == null)
                return;

            if (obj.Length < 2)
                return;
            
            var pluginItem = MainEditor.PluginAssist.PluginManagerWindow.Instance.GetPluginItem((string)(obj[0]));
            if (pluginItem == null)
                return;

            var editParam = new object[obj.Length - 1];
            for (int i = 1; i < obj.Length; i++)
            {
                editParam[i - 1] = obj[i];
            }

            var editCtrl = GetPluginControl(pluginItem.PluginObject);
            if (editCtrl != null)
            {
                if (editCtrl.IsShowing)
                {
                    var parentTabItem = Program.GetParent(editCtrl, typeof(TabItem)) as TabItem;
                    var parentTabControl = Program.GetParent(editCtrl, typeof(TabControl)) as TabControl;
                    if (parentTabControl != null && parentTabItem != null)
                    {
                        parentTabControl.SelectedItem = parentTabItem;
                    }

                    // 将包含该控件的窗体显示到最前
                    var parentWin = Program.GetParent(editCtrl, typeof(Window)) as Window;

                    Program.BringWindowToTop(new System.Windows.Interop.WindowInteropHelper(parentWin).Handle);
                }
                else
                {
                    //var win = new DockControl.DockAbleWindow();
                    var tabItem = new DockControl.Controls.DockAbleTabItem()
                    {
                        Header = editCtrl.KeyValue,
                        Content = editCtrl
                    };
                    //win.SetContent(tabItem);
                    //win.Show();
                    MainEditor.MainWindow.Instance.CurrentSurface.AddChild(tabItem);
                }
                editCtrl.PluginObject.SetObjectToEdit(editParam);
            }
            else
                pluginItem.PluginObject.SetObjectToEdit(editParam);
        }

        protected static Dictionary<EditorCommon.PluginAssist.IEditorPlugin, MainEditor.PluginAssist.PluginControlContainer> mPluginControlsDictionary = new Dictionary<EditorCommon.PluginAssist.IEditorPlugin, MainEditor.PluginAssist.PluginControlContainer>();
        // 取得插件控件
        public static MainEditor.PluginAssist.PluginControlContainer GetPluginControl(EditorCommon.PluginAssist.IEditorPlugin pluginObj)
        {
            if (!(pluginObj is System.Windows.FrameworkElement))
                return null;

            bool isMulti = false;
            var atts = pluginObj.GetType().GetCustomAttributes(typeof(PartCreationPolicyAttribute), false);
            if (atts.Length > 0)
            {
                switch (((PartCreationPolicyAttribute)(atts[0])).CreationPolicy)
                {
                    case CreationPolicy.NonShared:
                        isMulti = true;
                        break;
                }
            }

            EditorCommon.PluginAssist.IEditorPlugin tagPlugin = pluginObj;
            //atts = pluginObj.GetType().GetCustomAttributes(typeof(Export), false);
            //if (atts.Length <= 0)
            //    return null;

            //var contractName = ((Export)(atts[0])).Definition.ContractName;
            //tagPlugin = PluginAssist.PluginManagerWindow.Instance.CompositionContainer.GetExportedValue<EditorCommon.PluginAssist.IPlugin>(contractName);

            if (isMulti)
            {
                tagPlugin = System.Activator.CreateInstance(pluginObj.GetType()) as EditorCommon.PluginAssist.IEditorPlugin;
            }

            MainEditor.PluginAssist.PluginControlContainer retCtrl;
            if (mPluginControlsDictionary.TryGetValue(tagPlugin, out retCtrl))
                return retCtrl;

            retCtrl = new PluginAssist.PluginControlContainer();
            retCtrl.Content = tagPlugin;
            retCtrl.PluginObject = tagPlugin;
            if (retCtrl.PluginObject is EditorCommon.PluginAssist.IObjectEditorOperation)
            {
                ((EditorCommon.PluginAssist.IObjectEditorOperation)(retCtrl.PluginObject)).OnOpenEditor += OnOpenEditor;
            }
            mPluginControlsDictionary[tagPlugin] = retCtrl;


            return retCtrl;
        }

#endregion


        public static bool RemoveControlFromParent(FrameworkElement control)
        {
            //var parentCtrl = control.Parent;

            //while (parentCtrl != null)
            //{
            //    if (parentCtrl is FlyWindow)
            //    {
            //        ((FlyWindow)parentCtrl).RemoveControl(control);
            //    }
            //    else if (parentCtrl is UserControlContainer)
            //    {
            //        ((UserControlContainer)parentCtrl).RemoveControl(control);
            //    }

            //    parentCtrl = ((FrameworkElement)parentCtrl).Parent;
            //}
            if (control == null)
                return false;

            if (control.Parent == null)
                return false;


            if (control.Parent is ItemsControl)
            {
                var itemsCtrl = control.Parent as ItemsControl;
                itemsCtrl.Items.Remove(control);
            }
            else if (control.Parent is System.Windows.Controls.Panel)
            {
                var panel = control.Parent as System.Windows.Controls.Panel;
                panel.Children.Remove(control);
            }
            else if (control.Parent is ContentControl)
            {
                var contentCtrl = control.Parent as ContentControl;
                contentCtrl.Content = null;
            }
            else
                return false;

            return true;
        }

        public static string GetDescription(object obj)
        {
            if (obj == null)
                return "";

            var _type = obj.GetType();
            if(_type.IsEnum)
            {
                var fi =_type.GetField(System.Enum.GetName(_type, obj));
                if (fi != null)
                {
                    var dna = Attribute.GetCustomAttribute(fi, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
                    if (dna != null && !string.IsNullOrEmpty(dna.Description))
                        return dna.Description;
                }
            }

            return _type.Name;
        }

        public static FrameworkElement GetParent(FrameworkElement childElement, Type parentType)
        {
            if (childElement == null)
                return null;

            var parent = childElement.Parent as FrameworkElement;
            while (parent != null)
            {
                if (parent.GetType() == parentType)
                    return parent;

                var baseType = parent.GetType().BaseType;
                while (baseType != null)
                {
                    if (baseType == parentType)
                        return parent;

                    baseType = baseType.BaseType;
                }

                parent = parent.Parent as FrameworkElement;
            }

            return null;
        }
    }
}

// 刷新各种xml和xnd文件
/*/Template////////////////////////////////////////////////////////////////////////
var CSCommonAssembly = CSUtility.Program.GetAssemblyFromDllFileName("CSUtility.dll");
var files = System.IO.Directory.EnumerateFiles(CSUtility.Support.IFileManager.Instance.Root + "ZeusGame/Template", "*.*", SearchOption.AllDirectories);
foreach (var tempFile in files)
{
    var relFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(tempFile);

    var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(relFile);
    if (xmlHolder == null || xmlHolder.RootNode == null)
        continue;

    var typeStr = xmlHolder.RootNode.Name;
    var type = CSCommonAssembly.GetType(typeStr);

    if (type == null)
        continue;

    var ins = System.Activator.CreateInstance(type);
    if (ins == null)
        continue;

    CSUtility.Support.IConfigurator.FillProperty(ins, relFile);
    CSUtility.Support.IConfigurator.SaveProperty(ins, typeStr, relFile);
}
//////////////////////////////////////////////////////////////////////////


//Action////////////////////////////////////////////////////////////////////////
var actionFiles = System.IO.Directory.EnumerateFiles(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultActionDirectory, "*" + CSUtility.Support.IFileConfig.ActionExtension, SearchOption.AllDirectories);
foreach (var actFile in actionFiles)
{
    var actionSource = CSUtility.Animation.ActionNodeManager.Instance.GetActionSource(actFile, true, CSUtility.Helper.enCSType.All);
    if (actionSource == null)
        continue;
    actionSource.SaveActionNotifier(actFile, false, false);
}
//////////////////////////////////////////////////////////////////////////


//Effect////////////////////////////////////////////////////////////////////////
var files = System.IO.Directory.GetFiles(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultEffectDirectory, "*" + CSUtility.Support.IFileConfig.EffectExtension, System.IO.SearchOption.AllDirectories);
foreach (var effectFile in files)
{
    var effectTemplate = FrameSet.Effect.EffectManager.Instance.LoadEffectTemplate(effectFile);
    if (effectTemplate == null)
        continue;

    FrameSet.Effect.EffectManager.Instance.SaveEffectTemplate(effectTemplate.Id);
}
//////////////////////////////////////////////////////////////////////////

//Illumination////////////////////////////////////////////////////////////////////////
CCore.WeatherSystem.IlluminationManager.Instance.LoadAllIllumination();
foreach (var ilu in CCore.WeatherSystem.IlluminationManager.Instance.IlluminationDic)
{
    CCore.WeatherSystem.IlluminationManager.Instance.SaveIllumination(ilu.Key);
}
//////////////////////////////////////////////////////////////////////////

//Prefab////////////////////////////////////////////////////////////////////////
var files = System.IO.Directory.GetFiles(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultPrefabFolder, "*" + CSUtility.Support.IFileConfig.PrefabResExtension, System.IO.SearchOption.AllDirectories);
foreach (var prefabFile in files)
{
    var preRes = MidLayer.Prefab.PrefabResourceManager.Instance.LoadPrefabResource(prefabFile);
    MidLayer.Prefab.PrefabResourceManager.Instance.SavePrefabResource(preRes);
}
//////////////////////////////////////////////////////////////////////////

//Map////////////////////////////////////////////////////////////////////////
var folders = System.IO.Directory.GetDirectories(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.MapDirectory, "*.*", System.IO.SearchOption.TopDirectoryOnly);
foreach (var folder in folders)
{
    var folderInfo = new System.IO.DirectoryInfo(folder);
    var guid = CSUtility.Support.IHelper.GuidTryParse(folderInfo.Name);
    if (guid == Guid.Empty)
        continue;

    var world = new FrameSet.Scene.CellScene(System.Guid.NewGuid());
    world.LoadWorld("Maps/" + guid.ToString() + "/");
    world.LoadServerActor(CSUtility.Component.EActorGameType.NpcInitializer);
    world.SetNeighborSide(500);
    world.TravelTo(0, 0);
    var tileScene = world.SceneGraph as MidLayer.TileBased.ITileScene;
    var meterPerLevelX = world.SceneInit.SceneMeterX / world.SceneInit.SceneSizeX;
    var meterPerLevelZ = world.SceneInit.SceneMeterZ / world.SceneInit.SceneSizeZ;
    for (int x = 0; x < world.SceneInit.SceneSizeX; x++)
    {
        for (int z = 0; z < world.SceneInit.SceneSizeZ; z++)
        {
            world.TravelTo(x * meterPerLevelX + 1, z * meterPerLevelZ + 1);
        }
    }
    while (tileScene.GetWaitAddTileObjectsCount() > 0)
    {
        world.TravelTo(0, 0);
        world.Tick();
        System.Threading.Thread.Sleep(1);
    }
    world.SaveWorld("", true, IWorld.enSaveWorldType.ClientScene);
    world.SaveWorld("", true, IWorld.enSaveWorldType.NPC);
    world.SaveWorld("", true, IWorld.enSaveWorldType.Trigger);
    world.SaveWorld("", true, IWorld.enSaveWorldType.DynamicBlock);
    world.SaveWorld("", true, IWorld.enSaveWorldType.PostProcess);
    world.SaveWorld("", true, IWorld.enSaveWorldType.Grid);
    world.SaveWorld("", true, IWorld.enSaveWorldType.Camera);
    world.SaveWorld("", true, IWorld.enSaveWorldType.Lights);
}
//////////////////////////////////////////////////////////////////////////

//ScenePoint////////////////////////////////////////////////////////////////////////
var folders = System.IO.Directory.GetDirectories(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.MapDirectory, "*.*", System.IO.SearchOption.TopDirectoryOnly);
foreach (var folder in folders)
{
    var folderInfo = new System.IO.DirectoryInfo(folder);
    var guid = CSUtility.Support.IHelper.GuidTryParse(folderInfo.Name);
    if(guid == Guid.Empty)
        continue;

    var groups = CSUtility.Map.ScenePointGroupManager.Instance.LoadAllGroups(guid);
    var dir = CSUtility.Map.ScenePointGroupManager.Instance.GetDirectionWithMapId(guid);
    foreach (var group in groups)
    {

        var xndHolder = CSUtility.Support.XndHolder.NewXNDHolder();
        var att = xndHolder.Node.AddAttrib("Type_1");
        var groupType = group.GetType();
        att.BeginWrite();
        string typeStr = groupType.Assembly.GetName().Name + "|" + groupType.FullName;
        att.Write(typeStr);
        att.EndWrite();
        att = xndHolder.Node.AddAttrib("Datas");
        att.BeginWrite();
        group.Write(att);
        att.EndWrite();

        var file = dir + group.Id.ToString() + ".sp";
        CSUtility.Support.XndHolder.SaveXND(file, xndHolder);

        xndHolder.Node.TryReleaseHolder();
    }
}
//////////////////////////////////////////////////////////////////////////



            //UVAnim////////////////////////////////////////////////////////////////////////
            var files = System.IO.Directory.GetFiles(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUVAnimDirectory, "*" + CSUtility.Support.IFileConfig.UVAnimExtension, System.IO.SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var anim = new UISystem.UVAnim();
                if (!CSUtility.Support.IConfigurator.FillProperty(anim, file))
                    continue;

                CSUtility.Support.IConfigurator.SaveProperty(anim, anim.UVAnimName, file);
            }
            /////////////////////////////////////////////////////////////////////////*/
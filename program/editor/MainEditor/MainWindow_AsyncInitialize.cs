using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MainEditor
{
    public partial class MainWindow
    {
        Thread mInitializeThread;
        
        public void AsyncShow()
        {
            if(mInitializeThread == null)
            {
                // 控件在主线程创建
                var pmwIns = MainEditor.PluginAssist.PluginManagerWindow.Instance;
                var vcmIns = EditorCommon.VersionControl.VersionControlManager.Instance;

                mInitializeThread = new Thread(AsyncInitializeProcess);
                mInitializeThread.Start();
                //AsyncInitializeProcess();
            }
        }

        private void AsyncInitializeProcess()
        {
            try
            {
                EditorLoading.Instance.InitializeRuning = true;

                GenerateToolMenu(0.05);

                var assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Server_Directory + "/ServerCommon.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "cscommon", assembly);
                assembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.Server_Directory + "/Server.dll");
                CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "game", assembly);

                EditorCommon.PluginAssist.PluginManager.Instance.InitializePlugins(0.9, new Action<string, double>((info, progressChange)=>
                {
                    EditorLoading.Instance.UpdateProcess(info, progressChange);
                }));

                EditorCommon.VersionControl.VersionControlManager.Instance.InitializePlugins(EditorCommon.PluginAssist.PluginManager.Instance.Plugins);
                EditorCommon.VersionControl.VersionControlManager.Instance.ActiveVersionControlSystem("SVN");

                // 生成Metadata
                EditorCommon.ClassMetadataGenerator.ProcessMetadata(0.1, new Action<string, double>((info, progressChange) =>
                {
                    EditorLoading.Instance.UpdateProcess(info, progressChange);
                }));

                EditorLoading.Instance.UpdateProcess("正在加载布局信息...", 0);

                this.Dispatcher.Invoke(() =>
                {
                    DockControl.DockManager.Instance.LayoutConfigFileName = mConfigFileName;
                    LoadLayoutConfig(mConfigFileName);

                    // 处理文件删除列表
                    Program.LoadDeleteFileList();
                    Program.DelFileInFileList();

                    UpdateArrangementConfigs();
                });

                this.Dispatcher.Invoke(() =>
                {
                    EditorLoading.Instance.FinishInitialize();
                    this.Show();
                });
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}

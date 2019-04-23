using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace WeatherEditor
{
    /// <summary>
    /// PostProcess.xaml 的交互逻辑
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "WeatherEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/后期特效")]
    [Guid("AA023378-8965-4898-BB06-0E033F46FE8C")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class PostProcess : UserControl, EditorCommon.PluginAssist.IEditorPlugin
    {
        ///////////////////////////////////////////////////////////
        public string PluginName
        {
            get { return "后期特效"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "后期特效的编辑",
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
        ///////////////////////////////////////////////////////////

        public PostProcess()
        {
            InitializeComponent();

            CCore.Program.OnWorldLoaded += Program_OnWorldLoaded_PostProcess;
            InitializePostProcess(CCore.Client.MainWorldInstance);
        }

        private void Program_OnWorldLoaded_PostProcess(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (componentName.Equals("后期特效"))
                    InitializePostProcess(world);
            });
        }

        private void InitializePostProcess(CCore.World.World world)
        {
            StackPanel_PostProcess.Children.Clear();

            if(world.PostProceses.Count == 0)
            {
                // 临时代码，日后后期特效改为连线 ////////////////////////////////
                world.AddPostProcess(new CCore.Graphics.PostProcess_Bloom());
                world.AddPostProcess(new CCore.Graphics.PostProcess_ColorGrading());
                world.AddPostProcess(new CCore.Graphics.PostProcess_ToneMapping());
                ///////////////////////////////////////////////////////////////
            }

            foreach (var postProcess in world.PostProceses)
            {
                WPG.PropertyGrid pg = new WPG.PropertyGrid()
                {
                    ExpanderHeadline = true,
                    ShowHeadline = true,
                    Headline = postProcess.Name,
                };
                pg.Instance = postProcess;
                StackPanel_PostProcess.Children.Add(pg);
            }
        }
    }
}

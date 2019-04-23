using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace WorldPropertys
{
    /// <summary>
    /// Interaction logic for WorldAttributeEditor.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "WorldPropertys")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/WorldPropertys")]
    [Guid("E35947F0-3DDB-4DD2-91C8-E14D2CD56003")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class WorldAttributeEditor : UserControl, EditorCommon.PluginAssist.IEditorPlugin
    {
        public string PluginName
        {
            get { return "WorldPropertys"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "WorldPropertys",
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

        public void Tick()
        {

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

        /////////////////////////////////////////////////////////////////////////

        public WorldAttributeEditor()
        {
            InitializeComponent();
            
            WorldPropertyGrid.Instance = CCore.Client.MainWorldInstance.WorldInit;
        }
    }
}

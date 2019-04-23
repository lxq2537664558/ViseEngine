using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ResourcesBrowser
{
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "MapEditor")]
    [Guid("0B2414D3-1358-4B8A-85C9-45E339F79660")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MapResourceProcesser : INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string PluginName
        {
            get { return "地图编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "地图编辑器",
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
            if (obj == null)
                return;

            if (obj.Length == 0)
                return;

            try
            {
                var resInfo = obj[0] as ResourceInfos.MapResourceInfo;
                if (resInfo == null)
                    return;   

                CCore.Engine.Instance.Client.MainWorld.Cleanup();

                var newWorld = new CCore.World.World(resInfo.Id);

                var worldInit = CCore.World.WorldInitFactory.Instance.CreateWorldInit((byte)(resInfo.WorldInitType));
                worldInit.Load(resInfo.AbsResourceFileName);
                CCore.Engine.Instance.Client.MainWorld = newWorld;
                newWorld.Initialize(worldInit);
                newWorld.Initialize(resInfo.AbsResourceFileName);
                newWorld.LoadWorld(resInfo.AbsResourceFileName);

                CCore.Engine.Instance.Client.OnReCreateChiefRole?.Invoke();

            }
            catch (System.Exception e)
            {
                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Error, e.ToString());
            }
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
    }
}

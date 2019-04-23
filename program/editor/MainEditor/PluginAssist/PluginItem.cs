using System;
using System.ComponentModel;

namespace MainEditor.PluginAssist
{
    public class PluginItem : EditorCommon.PluginAssist.PluginItem
    {
        [Browsable(false)]
        public System.Windows.UIElement InstructionControl
        {
            get
            {
                if (PluginObject == null)
                    return null;

                return PluginObject.InstructionControl;
            }
        }

        string mVersion = "";
        [Browsable(false)]
        public string Version
        {
            get { return mVersion; }
            set
            {
                mVersion = value;
                OnPropertyChanged("Version");
            }
        }

        bool mActive = true;
        [Browsable(false)]
        public override bool Active
        {
            get { return mActive; }
            set
            {
                mActive = value;
                OnPropertyChanged("Active");    
                
                if (HostMeuItem != null)
                {
                    if (mActive)
                    {
                        HostMeuItem.IsEnabled = true;
                    }
                    else
                    {
                        HostMeuItem.IsEnabled = false;
                    }
                }   
                
            }
        }


        System.Windows.Visibility mDelBtnVisible = System.Windows.Visibility.Collapsed;
        [Browsable(false)]
        public System.Windows.Visibility DelBtnVisible
        {
            get { return mDelBtnVisible; }
            set
            {
                mDelBtnVisible = value;
                OnPropertyChanged("DelBtnVisible");
            }
        }
        //        ComposablePartCatalog mCatalog;

        

        //public PluginItem(ComposablePartCatalog catalog)
        //{
        //    mCatalog = catalog;
        //}

        public PluginItem(Guid id, EditorCommon.PluginAssist.IEditorPlugin plugin, EditorCommon.PluginAssist.IEditorPluginData pluginData)
        {
            mId = id;
            mPluginObject = plugin;
            mPluginData = pluginData;

            if (plugin != null)
            {
                PluginName = plugin.PluginName;
                Version = plugin.Version;
                
                if(plugin is EditorCommon.PluginAssist.IEditorPluginOperation)
                {
                    var obj = plugin as EditorCommon.PluginAssist.IEditorPluginOperation;
                    AssemblyPath = obj.AssemblyPath;
                    DelBtnVisible = System.Windows.Visibility.Visible;
                }
            }
        }
    }
}

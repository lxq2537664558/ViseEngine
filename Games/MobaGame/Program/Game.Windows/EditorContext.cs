using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Windows
{
    public class EditorContext
    {
        public void _OnGameWindowDragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            mEditor?.OnGameWindowDragEnter(sender, e);
        }
        public void _OnGameWindowDragLeave(object sender, EventArgs e)
        {
            mEditor?.OnGameWindowDragLeave(sender, e);
        }
        public void _OnGameWindowDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            mEditor?.OnGameWindowDragDrop(sender, e);
        }
        public void _OnGameWindowDragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            mEditor?.OnGameWindowDragOver(sender, e);
        }

        bool mMainEditorAssemblyLoaded = false;
        System.Reflection.Assembly mMainEditorAssembly = null;
        public System.Reflection.Assembly MainEditorAssembly
        {
            get
            {
                if (mMainEditorAssemblyLoaded)
                    return mMainEditorAssembly;

                if (mMainEditorAssembly == null)
                {
                    mMainEditorAssemblyLoaded = true;
                    try
                    {
                        mMainEditorAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "MainEditor.dll");
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }

                return mMainEditorAssembly;
            }
        }

        static EditorContext smInstance = new EditorContext();
        public static EditorContext Instance
        {
            get { return smInstance; }
        }

        CCore.EditorClassBase mEditor = null;

        public bool IsEditorMode
        {
            get
            {
                if (mEditor != null)
                    return mEditor.IsEditorMode;
                return false;
            }
            set
            {
                if (mEditor != null)
                    mEditor.IsEditorMode = value;
            }
        }

        public void FinalInstance()
        {
            //ExamplePlugins.ZeusGame.WorldEditorOperation.FinalInstance();
            mEditor?.FinalInstance();
            smInstance = null; 
        }

        public EditorContext()
        {
            if(MainEditorAssembly != null)
            {
                mEditor = MainEditorAssembly.CreateInstance("MainEditor.WorldEditorOperation") as CCore.EditorClassBase;
            }
        }

        public void InitializeEditor(CCore.Graphics.REnviroment env, System.Windows.Forms.Form gameForm)
        {
            mEditor?.Initialize(env, gameForm);
        }

        public void OnGameWindowChanged(int left, int top, int height, int width)
        {
            //ExamplePlugins.ZeusGame.WorldEditorOperation.UpdateDockingWindow(this.Left, this.Top, this.Height, this.Width);
            mEditor?.OnGameWindowChanged(left, top, height, width);
        }

        public void Tick()
        {
            mEditor?.Tick();
        }
    }
}

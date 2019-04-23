using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Windows
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run();

            var form = new GameForm();
            form.Show();
            
            var clientWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "ClientCommon.dll");
            CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Windows, "cscommon", clientWindowsAssembly);
            var clientAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Client, "Client.dll");
            CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.enPlatform.Windows, "game", clientAssembly);
            CCore.Support.ReflectionManager.Instance.RegAssembly(clientAssembly);

            var init = new Client.GameInit();
            init.ViewTarget = new CCore.Graphics.ViewTarget();
            init.ViewTarget.SetControl(form);
            //init.ViewTarget.MRTScale = 0.5F;
            bool bD3D = true;
            if(bD3D)
            {
                init.DeviceType = CCore.Graphics.EDeviceType.TypeD3D9;
                Client.AppConfig.Instance.MTRendering = true;
            }
            else
            {
                init.DeviceType = CCore.Graphics.EDeviceType.TypeGLES2;
                Client.AppConfig.Instance.MTRendering = false;
            }

            Client.Game.Instance.Start(init);
            // 创建项目时决定是否将纹理变换到SRGB空间来计算光照。目前手机上不支持SRGB，可以在这里关掉。
            CCore.Engine.Instance.Client.Graphics.UseSRGBSpace = true;
            EditorContext.Instance.InitializeEditor(Client.Game.Instance.REnviroment, form);
            CCore.Engine.Instance.DoLoop(Client.AppConfig.Instance.Interval, ()=>
            {
                Client.Game.Instance.Tick();
                EditorContext.Instance.Tick();
            }, form.Handle);
            EditorContext.Instance.FinalInstance();
            Client.Game.Instance.Stop();

            System.GC.Collect();

            System.GC.WaitForFullGCComplete();
            System.GC.WaitForPendingFinalizers();
        }
    }
}

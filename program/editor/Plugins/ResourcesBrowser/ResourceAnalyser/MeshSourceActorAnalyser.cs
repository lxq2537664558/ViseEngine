using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesBrowser.ResourceAnalyser
{
    [D3DViewer.ResourceAnalyser(AnalyserType = "MeshSource")]
    public class MeshSourceActorAnalyser : D3DViewer.IResourceAnalyser
    {
        public object[] GetResources(object[] info)
        {
            if (info == null)
                return null;

            if (info.Length == 0)
                return null;

            var meshFileName = (string)(info[0]);

            if (string.IsNullOrEmpty(meshFileName))
                return null;

            var mshInit = new CCore.Mesh.MeshInit();
            CCore.Mesh.MeshInitPart mshInitPart = new CCore.Mesh.MeshInitPart();
            mshInitPart.MeshName = meshFileName.Replace("/", "\\");
            mshInit.MeshInitParts.Add(mshInitPart);
            mshInit.CanHitProxy = false;
            mshInit.ForceLoad = true;

            var visual = new CCore.Mesh.Mesh();
            visual.Initialize(mshInit, null);
            var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetDefaultMaterial();
            for (int i = 0; i < visual.GetMaxMaterial(0); ++i)
            {
                visual.SetMaterial(0, i, mtl);
            }

            var actInit = new CCore.World.MeshActorInit();
            var retActor = new CCore.World.MeshActor();
            retActor.Initialize(actInit);
            retActor.SetPlacement(new CSUtility.Component.StandardPlacement(retActor));
            retActor.mUpdateAnimByDistance = false;
            retActor.Visual = visual;

            return new object[] { "Actor", retActor };
        }
    }

}

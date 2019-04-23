using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesBrowser.ResourceAnalyser
{

    [D3DViewer.ResourceAnalyser(AnalyserType = "Mesh")]
    public class MeshActorAnalyser : D3DViewer.IResourceAnalyser
    {
        public object[] GetResources(object[] info)
        {
            if (info == null)
                return null;

            if (info.Length == 0)
                return null;

            var res = info[0] as CCore.Mesh.Mesh;

            var actInit = new CCore.World.MeshActorInit();
            var retActor = new CCore.World.MeshActor();
            retActor.Initialize(actInit);
            retActor.SetPlacement(new CSUtility.Component.StandardPlacement(retActor));
            retActor.mUpdateAnimByDistance = false;
            retActor.Visual = res;

            return new object[] { "Actor", retActor };
        }
    }
}

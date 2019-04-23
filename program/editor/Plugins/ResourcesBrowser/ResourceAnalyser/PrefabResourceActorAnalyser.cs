using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesBrowser.ResourceAnalyser
{
    [D3DViewer.ResourceAnalyser(AnalyserType = "PrefabResource")]
    class PrefabResourceActorAnalyser : D3DViewer.IResourceAnalyser
    {
        public object[] GetResources(object[] info)
        {
            if (info == null)
                return null;

            if (info.Length == 0)
                return null;

            var mt = (CCore.Mesh.MeshTemplate)(info[0]);
            if (mt == null)
                return null;

            var mshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = mt.MeshID,
                CanHitProxy = false
            };
            var visual = new CCore.Mesh.Mesh();
            visual.Initialize(mshInit, null);

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

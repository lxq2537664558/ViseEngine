using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesBrowser.ResourceAnalyser
{
    [D3DViewer.ResourceAnalyser(AnalyserType = "Effect")]
    public class EffectActorAnalyser : D3DViewer.IResourceAnalyser
    {
        public object[] GetResources(object[] info)
        {
            if (info == null)
                return null;

            if (info.Length == 0)
                return null;

            var effect = info[0] as CCore.Effect.EffectTemplate;
            if (effect == null)
                return null;

            var visInit = new CCore.Component.EffectVisualInit()
            {
                EffectTemplateID = effect.Id,
                CanHitProxy = false
            };
            var visual = new CCore.Component.EffectVisual();
            visual.Initialize(visInit, null);

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

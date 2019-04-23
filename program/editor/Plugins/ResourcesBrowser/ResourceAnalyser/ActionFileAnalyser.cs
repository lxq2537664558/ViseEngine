using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesBrowser.ResourceAnalyser
{
    [D3DViewer.ResourceAnalyser(AnalyserType = "ActionFile")]
    public class ActionFileAnalyser : D3DViewer.IResourceAnalyser
    {
        public object[] GetResources(object[] info)
        {
            try
            {
                if (info == null)
                    return null;

                if (info.Length < 2)
                    return null;

                List<object> retList = new List<object>();
                retList.Add("Action");

                retList.Add(info[0]);
                var actionName = (string)(info[1]);
                var animNode = new CCore.AnimTree.AnimTreeNode_Action();
                animNode.Initialize();
                animNode.ActionName = actionName;
                retList.Add(animNode);

                return retList.ToArray();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return null;
        }
    }
}

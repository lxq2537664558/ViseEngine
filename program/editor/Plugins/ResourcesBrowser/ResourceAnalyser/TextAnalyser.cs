using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesBrowser.ResourceAnalyser
{
    [D3DViewer.ResourceAnalyser(AnalyserType = "Text")]
    public class TextAnalyser : D3DViewer.IResourceAnalyser
    {
        public object[] GetResources(object[] info)
        {
            if (info == null)
                return null;

            if (info.Length == 0)
                return null;

            List<object> retList = new List<object>();
            retList.Add("Text");
            retList.AddRange(info);

            return retList.ToArray();
        }
    }
}

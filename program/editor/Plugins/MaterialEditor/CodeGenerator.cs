namespace MaterialEditor
{
    public class CodeGenerator
    {
        public static System.IO.TextWriter GenerateCode(MaterialFileInfo info, CodeGenerateSystem.Base.NodesContainer container, Controls.MaterialControl setValueCtrl)
        {
            if (info == null)
                return null;

            //string strRequire = getValueCtrl.GetRequireValueString();
            //if (string.IsNullOrEmpty(strRequire))
            //    strRequire = "DiffuseUV";
            //else if (!strRequire.Contains("DiffuseUV"))
            //    strRequire += "|DiffuseUV";
            string strRequire = "DiffuseUV";
            foreach(var node in container.CtrlNodeList)
            {
                if(node is Controls.Value.PixelMaterialData)
                {
                    var retStr = ((Controls.Value.PixelMaterialData)node).GetRequireString();
                    if (!strRequire.Contains(retStr))
                        strRequire += "|" + retStr;
                }
            }

            foreach (var node in container.CtrlNodeList)
            {
                if (node is MaterialEditor.Controls.MaterialStreamRequire)
                {
                    var reStr = ((MaterialEditor.Controls.MaterialStreamRequire)node).GetStreamRequire();
                    if (!strRequire.Contains(reStr))
                        strRequire += "|" + reStr;
                }
            }

            info.SetRequire(strRequire);

            var tw = new System.IO.StringWriter();

            // #include...
            string includeString = "";
            foreach (var node in container.CtrlNodeList)
            {
                if (node is Controls.Operation.Function)
                {
                    var funcCtrl = node as Controls.Operation.Function;
                    if (!includeString.Contains(funcCtrl.Include))
                        includeString += "#include \"" + funcCtrl.Include + "\"\r\n";
                    //tw.WriteLine("#include \"" + funcCtrl.Include + "\"");
                }
            }

            tw.WriteLine(includeString);
            tw.WriteLine("");

            foreach (var node in container.CtrlNodeList)
            {
                if(node is Controls.BaseNodeControl_ShaderVar)
                {
                    var ctrl = node as Controls.BaseNodeControl_ShaderVar;
                    tw.Write(ctrl.GetValueDefine());
                }
            }
            tw.WriteLine("");

            tw.WriteLine("#ifdef ByLayerBased");
            tw.WriteLine("void " + info.MainMethodName + "(inout PixelMaterialTrans pssem)");
            tw.WriteLine("#else");
            tw.WriteLine("void	DoMaterial(inout PixelMaterialTrans pssem)");
            tw.WriteLine("#endif");
            tw.WriteLine("{");
            string strSegment = "";
            string strDefinitionSegment = "";
            setValueCtrl.GCode_GenerateCode(ref strDefinitionSegment, ref strSegment, 1, null);
            tw.WriteLine(strDefinitionSegment + "\r\n");
            tw.WriteLine(strSegment);
            tw.WriteLine("}");

            info.SetMaterialCodeStringBlock(tw.ToString());
            //}
            //else
            //    tw.Write(m_strMaterialCodeStringBlock);

            return tw;
        }
    }
}

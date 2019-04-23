using System;
using System.Collections;

namespace CSUtility.Support
{
    public sealed class DataValueAttribute : System.Attribute
    {
        public bool DefaultValueNotSave;
        public System.String Name;
        public DataValueAttribute(System.String name, bool defaultValueNotSave = true)
        {
            Name = name;
            DefaultValueNotSave = defaultValueNotSave;
        }
    }

    public interface IXmlSaveLoadProxy
    {
        bool Save(XmlNode node, XmlHolder holder);
        bool Load(XmlNode node);
    }

    public class IConfigurator
    {

        static unsafe void FreeNativeString(char* str)
        {
            if (str == (char*)0)
                return;

            System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)str);
        }

        static unsafe char* AllocateNativeString(string value)
        {
            if (value == null || string.IsNullOrEmpty(value))
                return (char*)0;
            else
            {
                return (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(value).ToPointer();
            }
        }

        public static bool FillProperty(System.Object obj, System.String file)
        {
            if (obj == null)
                return false;
            if (!System.IO.Path.IsPathRooted(file))
                file = CSUtility.Support.IFileManager.Instance.Root + file;

            unsafe
            {
                var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(file);
                if (xmlHolder == null)
                    return false;
                if (xmlHolder.RootNode == null)
                    return false;

                return FillProperty(obj, xmlHolder.RootNode);
                //var att = xmlHolder.RootNode.FindAttrib("Ver");
                //if (att != null)
                //{
                //    var ver = System.Convert.ToInt32(att.Value);
                //    switch (ver)
                //    {
                //        default:
                //            return FillProperty(obj, xmlHolder.RootNode);
                //    }
                //}
            }
        }

        public static bool FillProperty(System.Object obj, System.String path, System.Guid guid, System.String postFix)
        {
            System.String file = CSUtility.Support.IFileManager.Instance.Root + path + guid.ToString() + "." + postFix;
            return FillProperty(obj, file);

            //unsafe
            //{
            //    //IntPtr strPtr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(file);
            //    var xmlPtr = IDllImportAPI.RapidXml_LoadFileA(file);
            //    var root = IDllImportAPI.RapidXmlA_RootNode(xmlPtr);
            //    bool ret = FillProperty(obj, root);

            //    IDllImportAPI.RapidXmlA_Delete(xmlPtr);
            //    return ret;
            //}
        }

        public static unsafe bool FillProperty(System.Object obj, CSUtility.Support.XmlNode node)
        {
            if (node == null)
                return false;

            try
            {
                unsafe
                {
                    System.Type type = obj.GetType();
                    foreach (System.Reflection.PropertyInfo prop in type.GetProperties())
                    {
                        try
                        {
                            if (prop.CanWrite == false)
                                continue;
                            System.Object[] attrs = prop.GetCustomAttributes(typeof(DataValueAttribute), true);
                            if (attrs == null)
                                continue;
                            if (attrs.Length <= 0)
                                continue;
                            DataValueAttribute dv = (DataValueAttribute)attrs[0];
                            if (dv == null)
                                continue;

                            var cNode = node.FindNode(dv.Name);
                            if (cNode != null)
                            {
                                var attType = cNode.FindAttrib("Type");
                                var proType = Program.GetTypeFromSaveString(attType.Value);

                                if (!prop.PropertyType.IsAbstract && !prop.PropertyType.IsInterface && prop.PropertyType != proType)
                                    continue;

                                prop.SetValue(obj, ReadValue(cNode, proType), null);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                            System.Diagnostics.Debug.WriteLine(prop);
                        }
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return false;
        }

        public static unsafe object ReadValue(XmlNode cNode, Type itemType)//, System.Reflection.Assembly assem)
        {
            var attV = cNode.FindAttrib("Value");

            if(itemType.GetInterface(typeof(IXmlSaveLoadProxy).FullName) != null)
            {
                var slpObj = System.Activator.CreateInstance(itemType) as IXmlSaveLoadProxy;
                if(slpObj.Load(cNode))
                    return slpObj;
            }
            else if (itemType.IsGenericType)
            {
                if (itemType.IsValueType)
                {
                    // KeyValuePair<TKey, TValue>
                    var pIKey = itemType.GetProperty("Key");
                    var pIValue = itemType.GetProperty("Value");
                    if (pIKey == null || pIValue == null)
                        return null;

                    var keyNode = cNode.FindNode("Key");
                    var valueNode = cNode.FindNode("Value");
                    if (keyNode == null || valueNode == null)
                        return null;

                    var keyTypeAtt = keyNode.FindAttrib("Type");
                    if (keyTypeAtt == null)
                        return null;
                    var keyType = Program.GetTypeFromSaveString(keyTypeAtt.Value);
                    var keyObj = ReadValue(keyNode, keyType);

                    var valueTypeAtt = valueNode.FindAttrib("Type");
                    if (valueTypeAtt == null)
                        return null;
                    var valueType = Program.GetTypeFromSaveString(valueTypeAtt.Value);
                    var valueObj = ReadValue(valueNode, valueType);

                    var genericObj = System.Activator.CreateInstance(itemType, new object[] { keyObj, valueObj });

                    //pIKey.SetValue(genericObj, keyObj);
                    //pIValue.SetValue(genericObj, valueObj);

                    return genericObj;
                }
                else
                {
                    var genericObj = System.Activator.CreateInstance(itemType);

                    System.Reflection.MethodInfo addMethod = itemType.GetMethod("Add");
                    if (addMethod == null)
                        return null;

                    foreach (var childNode in cNode.GetNodes())
                    {
                        var attC = childNode.FindAttrib("Type");
                        if (attC == null)
                            continue;

                        var tempType = Program.GetTypeFromSaveString(attC.Value);
                        var vl = ReadValue(childNode, tempType);

                        var methodparamters = addMethod.GetParameters();
                        if (methodparamters.Length == 1)
                            addMethod.Invoke(genericObj, new System.Object[] { vl });
                        else if (methodparamters.Length == 2)
                        {
                            var pIKey = vl.GetType().GetProperty("Key");
                            var pIValue = vl.GetType().GetProperty("Value");
                            if (pIKey != null && pIValue != null)
                            {
                                addMethod.Invoke(genericObj, new object[] { pIKey.GetValue(vl), pIValue.GetValue(vl) });
                            }
                        }
                    }

                    return genericObj;
                }
            }
            else if(itemType.IsArray)
            {
                var att = cNode.FindAttrib("Rank");
                if (att == null)
                    return null;
                
                var elementTypeName = itemType.FullName.Remove(itemType.FullName.IndexOf('['));
                var elementType = itemType.Assembly.GetType(elementTypeName);

                int rank = System.Convert.ToInt32(att.Value);
                int[] longlength = new int[rank];
                for(int i=0; i<rank; i++)
                {
                    att = cNode.FindAttrib("LongLength" + i);
                    if (att == null)
                        return null;

                    var len = System.Convert.ToInt32(att.Value);
                    longlength[i] = len;
                }

                int[] idxs = new int[rank];
                for(int i=0; i<rank; i++)
                {
                    idxs[i] = 0;
                }
                var retValue = Array.CreateInstance(elementType, longlength);
                foreach (var childNode in cNode.GetNodes())
                {
                    var attC = childNode.FindAttrib("Type");
                    if (attC == null)
                        continue;

                    var tempType = Program.GetTypeFromSaveString(attC.Value);
                    var vl = ReadValue(childNode, tempType);

                    retValue.SetValue(vl, idxs);

                    idxs[rank - 1]++;
                    for(int i=rank - 1; i>=0; i--)
                    {
                        if(idxs[i] >= longlength[i])
                        {
                            if (i == 0)
                            {
                                idxs[0] = 0;
                                break;
                            }
                            idxs[i - 1]++;
                            idxs[i] = 0;
                        }
                    }
                }
                return retValue;
            }
            else if (itemType == typeof(System.Boolean))
            {
                return System.Convert.ToBoolean(attV.Value);
            }
            else if (itemType == typeof(System.Char) || itemType == typeof(System.SByte))
            {
                return System.Convert.ToSByte(attV.Value);
            }
            else if (itemType == typeof(System.Int16))
            {
                return System.Convert.ToInt16(attV.Value);
            }
            else if (itemType == typeof(System.Int32))
            {
                return System.Convert.ToInt32(attV.Value);
            }
            else if (itemType == typeof(System.Int64))
            {
                return System.Convert.ToInt64(attV.Value);
            }
            else if (itemType == typeof(System.Byte))
            {
                return System.Convert.ToByte(attV.Value);
            }
            else if (itemType == typeof(System.UInt16))
            {
                return System.Convert.ToUInt16(attV.Value);
            }
            else if (itemType == typeof(System.UInt32))
            {
                return System.Convert.ToUInt32(attV.Value);
            }
            else if (itemType == typeof(System.UInt64))
            {
                return System.Convert.ToUInt64(attV.Value);
            }
            else if (itemType == typeof(System.Single))
            {
                return System.Convert.ToSingle(attV.Value);
            }
            else if (itemType == typeof(System.Double))
            {
                return System.Convert.ToDouble(attV.Value);
            }
            else if (itemType == typeof(System.String))
            {
                return attV.Value;
            }
            else if (itemType == typeof(System.Guid))
            {
                System.String clrStr = attV.Value;
                return CSUtility.Support.IHelper.GuidParse(clrStr);
            }
            else if (itemType == typeof(SlimDX.Vector2))
            {
                System.String clrStr = attV.Value;
                System.String[] strs = clrStr.Split(',');
                if (strs.Length != 2)
                    return false;
                SlimDX.Vector2 vec2;
                vec2.X = System.Convert.ToSingle(strs[0]);
                vec2.Y = System.Convert.ToSingle(strs[1]);
                return vec2;
            }
            else if (itemType == typeof(SlimDX.Vector3))
            {
                System.String clrStr = attV.Value;
                System.String[] strs = clrStr.Split(',');
                if (strs.Length != 3)
                    return false;
                SlimDX.Vector3 vec3;
                vec3.X = System.Convert.ToSingle(strs[0]);
                vec3.Y = System.Convert.ToSingle(strs[1]);
                vec3.Z = System.Convert.ToSingle(strs[2]);
                return vec3;
            }
            else if (itemType == typeof(SlimDX.Vector4))
            {
                System.String clrStr = attV.Value;
                System.String[] strs = clrStr.Split(',');
                if (strs.Length != 4)
                    return false;
                SlimDX.Vector4 vec4;
                vec4.X = System.Convert.ToSingle(strs[0]);
                vec4.Y = System.Convert.ToSingle(strs[1]);
                vec4.Z = System.Convert.ToSingle(strs[2]);
                vec4.W = System.Convert.ToSingle(strs[3]);
                return vec4;
            }
            else if (itemType == typeof(CSUtility.Support.Color))
            {
                int color = System.Convert.ToInt32(attV.Value);
                return CSUtility.Support.Color.FromArgb(color);
            }
            else if (itemType == typeof(CSUtility.Support.Thickness))
            {
                System.String clrStr = attV.Value;
                System.String[] strs = clrStr.Split(',');
                if (strs.Length != 4)
                    return null;
                double left = System.Convert.ToDouble(strs[0]);
                double top = System.Convert.ToDouble(strs[1]);
                double right = System.Convert.ToDouble(strs[2]);
                double bottom = System.Convert.ToDouble(strs[3]);
                return new CSUtility.Support.Thickness(left, top, right, bottom);
            }
            else if (itemType == typeof(System.DateTime))
            {
                DateTime outValue = DateTime.MinValue;
                if (System.DateTime.TryParse(attV.Value, out outValue))
                    return outValue;

                return null;
            }
            else if (itemType.IsEnum)
            {
                var enumValueStr = attV.Value;
                return System.Enum.Parse(itemType, enumValueStr);
            }
            else
            {
                if (itemType.IsAbstract || itemType.IsInterface)
                    return null;

                System.Object subObj = System.Activator.CreateInstance(itemType);
                if (subObj == null)
                    return null;

                if (false == FillProperty(subObj, cNode))
                    return null;

                return subObj;
            }

            return null;
        }

        public static bool SaveProperty(System.Object obj, System.String name, System.String file)
        {
            var xmlHolder = XmlHolder.NewXMLHolder(obj.GetType().FullName, "");

            // 版本号
            xmlHolder.RootNode.AddAttrib("Ver", "0");

            bool ret = SaveProperty(obj, xmlHolder.RootNode, xmlHolder);
            if (ret == false)
                return false;
            XmlHolder.SaveXML(file, xmlHolder, true);
            return true;
        }

        public static bool SaveProperty(System.Object obj, XmlNode node, XmlHolder holder)
        {
            if (obj == null)
                return false;
            var type = obj.GetType();
            foreach (System.Reflection.PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanRead == false)
                    continue;
                var attrs = prop.GetCustomAttributes(typeof(DataValueAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                DataValueAttribute dv = (DataValueAttribute)attrs[0];
                if (dv == null)
                    continue;

                var name = dv.Name;
                var cNode = node.AddNode(name, "", holder);

                var valueType = prop.PropertyType;
                var value = prop.GetValue(obj, null);
                if (value != null)
                    valueType = value.GetType();
                SaveValue(cNode, valueType, value, holder);
            }

            return true;
        }

        public static bool SaveValue(XmlNode cNode, System.Type type, object inValue, XmlHolder holder)
        {
            string typeName = Program.GetTypeSaveString(type);
            var attr_Type =  cNode.AddAttrib("Type", typeName);

            if(inValue is IXmlSaveLoadProxy)
            {
                var slp = inValue as IXmlSaveLoadProxy;
                if(slp != null)
                {
                    return slp.Save(cNode, holder);
                }
            }
            else if (type.IsGenericType)
            {
                if(type.IsValueType)
                {
                    // KeyValuePair<TKey, TValue>
                    var pIKey = type.GetProperty("Key");
                    var pIValue = type.GetProperty("Value");
                    if(pIKey != null && pIValue != null)
                    {
                        var childKey = cNode.AddNode("Key", "", holder);
                        var key = pIKey.GetValue(inValue, null);
                        if (key == null)
                            return false;
                        SaveValue(childKey, key.GetType(), key, holder);

                        var childValue = cNode.AddNode("Value", "", holder);
                        var value = pIValue.GetValue(inValue, null);
                        if (value == null)
                            return false;
                        SaveValue(childValue, value.GetType(), value, holder);
                    }
                }
                else
                {
                    var enumerableValue = inValue as IEnumerable;
                    if(enumerableValue != null)
                    {
                        var enumerator = enumerableValue.GetEnumerator();
                        foreach(var item in enumerableValue)
                        {
                            var child = cNode.AddNode("Data", "", holder);

                            if (item == null)
                                continue;
                            SaveValue(child, item.GetType(), item, holder);
                        }
                    }
                }
            }
            else if(type.IsArray)
            {
                var array = inValue as System.Array;
                if (array == null)
                    return false;

                cNode.AddAttrib("Rank", array.Rank.ToString());
                for(int i=0; i<array.Rank; i++)
                {
                    cNode.AddAttrib("LongLength" + i, array.GetLongLength(i).ToString());
                }

                foreach(var item in array)
                {
                    var child = cNode.AddNode("Data", "", holder);

                    if (item == null)
                        continue;
                    SaveValue(child, item.GetType(), item, holder);
                }
            }
            else if (type == typeof(bool) ||
                     type == typeof(char) ||
                     type == typeof(System.SByte) ||
                     type == typeof(System.Int16) ||
                     type == typeof(System.Int32) ||
                     type == typeof(System.Int64) ||
                     type == typeof(System.Byte) ||
                     type == typeof(System.UInt16) ||
                     type == typeof(System.UInt32) ||
                     type == typeof(System.UInt64) ||
                     type == typeof(System.Single) ||
                     type == typeof(System.Double) ||
                     type == typeof(System.Guid) ||
                     type.IsEnum
                     )
            {
                //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
                var temp = inValue.ToString();
                cNode.AddAttrib("Value", temp);
            }
            else if (type == typeof(System.String))
            {
                string temp = "";
                if (!string.IsNullOrEmpty((string)inValue))
                    temp = (string)inValue;

                cNode.AddAttrib("Value", temp);
            }
            else if (type == typeof(SlimDX.Vector2))
            {
                SlimDX.Vector2 vv = (SlimDX.Vector2)inValue;
                var temp = System.String.Format("{0},{1}", vv.X, vv.Y);
                cNode.AddAttrib("Value", temp);
            }
            else if (type == typeof(SlimDX.Vector3))
            {
                //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
                SlimDX.Vector3 vv = (SlimDX.Vector3)inValue;
                var temp = System.String.Format("{0},{1},{2}", vv.X, vv.Y, vv.Z);
                cNode.AddAttrib("Value", temp);
            }
            else if (type == typeof(SlimDX.Vector4))
            {
                //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
                SlimDX.Vector4 vv = (SlimDX.Vector4)inValue;
                var temp = System.String.Format("{0},{1},{2},{3}", vv.X, vv.Y, vv.Z, vv.W);
                cNode.AddAttrib("Value", temp);
            }
            else if (type == typeof(CSUtility.Support.Color))
            {
                //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
                CSUtility.Support.Color color = (CSUtility.Support.Color)inValue;
                int colorValue = color.ToArgb();
                var temp = System.String.Format("{0}", colorValue);
                cNode.AddAttrib("Value", temp);
            }
            else if (type == typeof(CSUtility.Support.Thickness))
            {
                //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
                CSUtility.Support.Thickness thickness = (CSUtility.Support.Thickness)inValue;
                var temp = System.String.Format("{0},{1},{2},{3}", thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
                cNode.AddAttrib("Value", temp);
            }
            else if (type == typeof(System.DateTime))
            {
                cNode.AddAttrib("Value", inValue.ToString());
            }
            else
            {
                //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);

                cNode.AddAttrib("Value", "null");
                SaveProperty(inValue, cNode, holder);
            }


            return true;
        }

        //private static bool SaveProperty(System.Object obj, IntPtr node, IXmlHolder holder)
        //{
        //    unsafe
        //    {
        //        //VXmlDocA *doc = node.document();

        //        System.Type type = obj.GetType();
        //        var properties = type.GetProperties();
        //        foreach (System.Reflection.PropertyInfo prop in properties)
        //        {
        //            if (prop.CanRead == false)
        //                continue;
        //            var attrs = prop.GetCustomAttributes(typeof(DataValueAttribute), true);
        //            if (attrs == null || attrs.Length == 0)
        //                continue;
        //            DataValueAttribute dv = (DataValueAttribute)attrs[0];
        //            if (dv == null)
        //                continue;

        //            //var name = AllocateNativeString(dv.Name);
        //            var name = dv.Name;
        //            //VXmlNodeA *cNode = doc.allocate_node(rapidxml.node_element,name);
        //            IntPtr cNode = IDllImportAPI.RapidXmlNodeA_allocate_node(holder.Holder, name, "");

        //            //FreeNativeString(name);
        //            //holder.mSavePtr.push_back(name);
        //            //node.append_node( cNode );
        //            IDllImportAPI.RapidXmlNodeA_append_node(node, cNode);

        //            SaveValue(cNode, prop.PropertyType, prop.GetValue(obj, null), holder);
        //        }

        //    }
        //    return true;
        //}

        //private static bool SaveValue(IntPtr cNode, System.Type type, object inValue, IXmlHolder holder)
        //{
        //    string typeName = "";
        //    if (type.IsGenericType)
        //        typeName = type.Namespace + "." + type.Name;// type.Assembly.FullName + "@" + type.FullName;
        //    else
        //        typeName = type.FullName;
        //    var attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);

        //    if (type.IsGenericType)
        //    {
        //        var pICount = type.GetProperty("Count");
        //        var pIItem = type.GetProperty("Item");

        //        var argTypes = type.GetGenericArguments();
        //        string itemType = "";
        //        if (argTypes.Length > 0)
        //        {
        //            if (argTypes[0].IsGenericType)
        //                itemType = argTypes[0].Namespace + "." + argTypes[0].Name;
        //            else
        //                itemType = argTypes[0].FullName;

        //            while (argTypes[0].IsGenericType)
        //            {
        //                argTypes = argTypes[0].GetGenericArguments();
        //                if (argTypes.Length > 0)
        //                {
        //                    if (argTypes[0].IsGenericType)
        //                        itemType += "|" + argTypes[0].Namespace + "." + argTypes[0].Name;
        //                    else
        //                        itemType += "|" + argTypes[0].FullName;
        //                }
        //            }
        //        }
        //        var attV = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "ItemType", itemType);
        //        IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attV);

        //        if (pICount != null && pIItem != null)
        //        {
        //            int count = (int)pICount.GetValue(inValue, null);
        //            for (int i = 0; i < count; i++)
        //            {
        //                var child = IDllImportAPI.RapidXmlNodeA_allocate_node(holder.Holder, "Data", "");

        //                System.Object[] params1 = new System.Object[1];
        //                params1[0] = i;
        //                var item = pIItem.GetValue(inValue, params1);
        //                if (item == null)
        //                    continue;
        //                SaveValue(child, item.GetType(), item, holder);

        //                IDllImportAPI.RapidXmlNodeA_append_node(cNode, child);
        //            }
        //        }
        //    }
        //    else if (type == typeof(bool) ||
        //             type == typeof(char) ||
        //             type == typeof(System.SByte) ||
        //             type == typeof(System.Int16) ||
        //             type == typeof(System.Int32) ||
        //             type == typeof(System.Int64) ||
        //             type == typeof(System.Byte) ||
        //             type == typeof(System.UInt16) ||
        //             type == typeof(System.UInt32) ||
        //             type == typeof(System.UInt64) ||
        //             type == typeof(System.Single) ||
        //             type == typeof(System.Double) ||
        //             type == typeof(System.Guid) ||
        //             type.IsEnum
        //             )
        //    {
        //        //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
        //        var temp = inValue.ToString();
        //        var attV = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Value", temp);
        //        IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attV);
        //    }
        //    else if (type == typeof(System.String))
        //    {
        //        string temp = "";
        //        if (!string.IsNullOrEmpty((string)inValue))
        //            temp = (string)inValue;

        //        var attV = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Value", temp);
        //        IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attV);
        //    }
        //    else if (type == typeof(SlimDX.Vector3))
        //    {
        //        //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
        //        SlimDX.Vector3 vv = (SlimDX.Vector3)inValue;
        //        var temp = System.String.Format("{0},{1},{2}", vv.X, vv.Y, vv.Z);
        //        var attV = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Value", temp);
        //        IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attV);
        //    }
        //    else if (type == typeof(CSUtility.Support.Color))
        //    {
        //        //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
        //        CSUtility.Support.Color color = (CSUtility.Support.Color)inValue;
        //        int colorValue = color.ToArgb();
        //        var temp = System.String.Format("{0}", colorValue);
        //        var attV = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Value", temp);
        //        IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attV);
        //    }
        //    else if (type == typeof(CSUtility.Support.Thickness))
        //    {
        //        //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);
        //        CSUtility.Support.Thickness thickness = (CSUtility.Support.Thickness)inValue;
        //        var temp = System.String.Format("{0},{1},{2},{3}", thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        //        var attV = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Value", temp);
        //        IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attV);
        //    }
        //    else
        //    {
        //        //attr_Type = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Type", typeName);

        //        var attV = IDllImportAPI.RapidXmlNodeA_allocate_attribute(holder.Holder, "Value", "null");
        //        IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attV);
        //        SaveProperty(inValue, cNode, holder);
        //    }

        //    IDllImportAPI.RapidXmlNodeA_append_attribute(cNode, attr_Type);


        //    return true;
        //}
    }

}

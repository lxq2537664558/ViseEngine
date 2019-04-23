using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;
using CSUtility.Support;
using System.Windows.Media;
using System.Windows.Documents;

namespace XMLToXND
{
    /// <summary>
    /// Interaction logic for XMLToXND.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "XMLToXNDEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/XML与XND转换器")]
    [Guid("5db09f5f-8d82-45ec-90be-bff0e3edeccd")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class XMLToXNDEditor : System.Windows.Controls.UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public string PluginName
        {
            get { return "XML与XND转换器"; }
        }

        public string Version
        {
            get { return "1.0.0"; }
        }

        public string AssemblyPath
        {
            get { return this.GetType().Assembly.Location; }
        }

        UIElement mInstructionControl = new TextBlock()
        {
            Text = "XML与XND转换器",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }

        public bool OnDeactive()
        {
            return true;
        }

        public void Tick()
        {

        }
        
        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        ///////////////////////////////////////////////////////////

        System.Collections.ObjectModel.ObservableCollection<string> mResultChanged = new System.Collections.ObjectModel.ObservableCollection<string>();
        public System.Collections.ObjectModel.ObservableCollection<string> ResultChanged
        {
            get { return mResultChanged; }
            set { mResultChanged = value; }
        }

        string mExportFileName = "";
        public string ExportFileName
        {
            get { return mExportFileName; }
            set
            {
                mExportFileName = value;
                OnPropertyChanged("ExportFileName");
            }
        }

        string mImportFileName = "";
        public string ImportFileName
        {
            get { return mImportFileName; }
            set
            {
                mImportFileName = value;
                OnPropertyChanged("ImportFileName");
            }
        }

        public XMLToXNDEditor()
        {
            InitializeComponent();

        }

        private void userControl_Initialized(object sender, EventArgs e)
        {
        }

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        Type GetClassTypeFromXml(string file)
        {
            var xmlHolder = LoadXmlFile(file);
            if (xmlHolder == null)
                return null;
            var assemblys = CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.Client);
            System.Type classType = null;
            foreach (var ass in assemblys)
            {
                classType = ass.GetType(xmlHolder.RootNode.Name);
                if (classType != null)
                    break;
            }
            return classType;
        }

        private void Button_ChangeFileClick(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputWindow.InputWindow();
            inputWindow.Title = "文件后缀名";
            inputWindow.Description = "输入新的文件后缀名:";
            inputWindow.Value = "xnd";

            if (inputWindow.ShowDialog((value, cultureInfo) =>
            {
                if (value == null)
                    return new ValidationResult(false, "内容不合法");
                return new ValidationResult(true, "");
            }) == false)
                return;


            if (mExportFileName == "" || mImportFileName == "")
                return;

            var files = Directory.GetFiles(mExportFileName, "*.*", SearchOption.AllDirectories);
            if (OptionOne.IsChecked == true)
            {
                foreach (var file in files)
                {
                    //var type = GetClassTypeFromXml(file);
                    //if (type == null)
                    //    continue;
                    //var obj = System.Activator.CreateInstance(type);
                    //CSUtility.Support.IConfigurator.FillProperty(obj, file);
                    //if (obj == null)
                    //    continue;
                    //var slObj = obj as CSUtility.Support.XndSaveLoadProxy;
                    //if (slObj == null)
                    //    continue;
                    //var xndHolder = XndHolder.NewXNDHolder();
                    //xndHolder.Node.SetName(obj.GetType().ToString());
                    //var att = xndHolder.Node.AddAttrib("Data");
                    //att.BeginWrite();
                    ////Int32 hashCode = (Int32)CSUtility.Program.GetNewClassHash(obj.GetType(), typeof(CSUtility.Support.AutoSaveLoadAttribute));
                    ////att.Write(hashCode);
                    ////att.Write(0);
                    //slObj.Write(att);
                    //att.EndWrite();
                    ////if (SaveFileToXndFromObj(obj, xndHolder.Node) == false)
                    ////    continue;
                    ////InitNewXNDNodeWithXMLNode(xndHolder.Node, xmlHolder.RootNode, true);
                    //var fileSplits = file.Split('\\');
                    //var fileFilters = fileSplits[fileSplits.Length - 1].Split('.');
                    //string fileName = "";
                    //for (int i = 0; i < fileFilters.Length - 1; i++)
                    //{
                    //    if (fileName != "")
                    //        fileName += '.';
                    //    fileName += fileFilters[i];
                    //}
                    //var importFileName = mImportFileName + '\\' + fileName + '.' + inputWindow.Value;
                    //CSUtility.Support.XndHolder.SaveXND(importFileName, xndHolder);
                    if (!EditorCommon.MeshTemplate.Instance.XmlChangeToXnd(file, mImportFileName, inputWindow.Value.ToString()))
                        continue;
                    // 提示文件转换成功
                    OutputInfo(string.Format("{0}文件从XML格式转换XND格式成功！", file), Brushes.Red);
                    //this.Dispatcher.Invoke(new Action(() =>
                    //{
                    //    OutputInfo(string.Format("{0}文件从XML格式转换XND格式成功！", mExportFileName), Brushes.Red);
                    //}));
                }
            }
            else
            {
                foreach (var file in files)
                {
                    var xndHolder = LoadXndFile(file);
                    if (xndHolder == null)
                        continue;
                    var xmlHolder = XmlHolder.NewXMLHolder(xndHolder.Node.GetName(), "");
                    //InitNewXMLNodeWithXNDNode(xmlHolder.RootNode, xndHolder.Node, true);
                    var fileSplits = file.Split('\\');
                    var fileFilters = fileSplits[fileSplits.Length - 1].Split('.');
                    string fileName = "";
                    for (int i = 0; i < fileFilters.Length - 1; i++)
                    {
                        if (fileName != "")
                            fileName += '.';
                        fileName += fileFilters[i];
                    }
                    var importFileName = mImportFileName + '\\' + fileName + '.' + inputWindow.Value;
                    //CSUtility.Support.XmlHolder.SaveXML(importFileName, xmlHolder, true);
                    // 提示文件转换成功
                    OutputInfo(string.Format("{0}文件从XND格式转换XML格式成功！", file), Brushes.Green);
                    //this.Dispatcher.Invoke(new Action(() =>
                    //{
                    //    OutputInfo(string.Format("{0}文件从XND格式转换XML格式成功！", mExportFileName), Brushes.Green);
                    //}));
                }
            }
        }
        
        public void OutputInfo(string info, Brush brush)
        {
            Paragraph p = new Paragraph()
            {
                Margin = new System.Windows.Thickness(0)
            };
            Span span = new Span(new Run(System.DateTime.Now.ToString() + ": "))
            {
                Foreground = Brushes.LightGray
            };
            p.Inlines.Add(span);
            span = new Span(new Run(info))
            {
                Foreground = brush
            };
            p.Inlines.Add(span);
            RichTextBox_Info.Document.Blocks.Add(p);

            RichTextBox_Info.ScrollToEnd();
        }

        private void Button_OpenExpotFileClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ExportFileName = fd.SelectedPath;
            }
        }

        uint GetXmlHashCode(XmlNode xmlNode)
        {
            var attrib = xmlNode.FindAttrib("Type");
            if (attrib == null)
                return uint.MaxValue;
            var index = attrib.Value.IndexOf('@');
            string attribTypeStr = attrib.Value;
            if (index > 0)
            {
                attribTypeStr = attrib.Value.Substring(index + 1);
            }
            var typeString = XmlClassTypeString + attribTypeStr + xmlNode.Name;
            return CSUtility.Support.UniHash.DefaultHash(typeString);
        }
        
        public bool SaveFileToXndFromObj(object obj, XndNode xndNode)
        {
            if (obj == null)
                return false;
            var type = obj.GetType();
            CSUtility.Support.ClassInfo classInfo = CSUtility.Support.ClassInfoManager.Instance.GetLastVersionClassInfo(type);
            if (classInfo == null)
                return false;
            foreach (var prop in classInfo.mPropertyInfoDic.Values)
            {
                if (prop.ProInfo.CanRead == false)
                    continue;
                var node = xndNode.AddNode(prop.mName, 0, 0);
                var att = node.AddAttrib("Data");
                att.BeginWrite();
                CSUtility.Support.XndSaveLoadProxy.WriteValue(att, prop.mType, prop.ProInfo.GetValue(obj, null));
                att.EndWrite();
            }
            return true;
        }

        string XmlClassTypeString = "";
        void InitNewXNDNodeWithXMLNode(XndNode xndNode, XmlNode xmlNode, bool isRootNode)
        {
            XndNode newXndNode = null;
            Int32 hashCode = Int32.MaxValue;
            if (isRootNode)
            {
                newXndNode = xndNode;
                newXndNode.SetName(xmlNode.Name);
                XmlClassTypeString = xmlNode.Name;
                var assemblys = CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.Client);
                System.Type classType = null;
                foreach (var ass in assemblys)
                {
                    classType = ass.GetType(xmlNode.Name);
                    if (classType != null)
                        break;
                }
                hashCode = (Int32)CSUtility.Program.GetNewClassHash(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute));
            }
            else
            {
                newXndNode = xndNode.AddNode(xmlNode.Name, 0, 0);
                hashCode = (System.Int32)GetXmlHashCode(xmlNode);
            }

            Type type = null;
            //var typeString = xmlNode.FindAttrib("Type");
            //if (typeString != null)
            //{
            //    type = CSUtility.Program.GetTypeFromSaveString(typeString.Value);
            //    if (type != null)
            //    {
                    
            //    }
            //}
            var att = newXndNode.AddAttrib("Data");
            att.BeginWrite();
            if (hashCode != int.MaxValue)
                att.Write(hashCode);
            foreach (var attrib in xmlNode.GetAttribs())
            {
                if (attrib.Name == "Type")
                {
                    type = CSUtility.Program.GetTypeFromSaveString(attrib.Value);
                    if (type == null)
                        continue;
                    object obj = attrib.Value;
                    if (type.IsGenericType)
                    {
                        if (type.IsValueType)
                        {
                            var pIKey = type.GetProperty("Key");
                            var pIValue = type.GetProperty("Value");
                            if (pIKey != null && pIValue != null)
                            {
                                var key = pIKey.GetValue(obj, null);
                                var value = pIValue.GetValue(obj, null);
                                if (key != null && value != null)
                                {
                                    var keyTypeStr = CSUtility.Program.GetTypeSaveString(key.GetType());
                                    att.Write(keyTypeStr);

                                    var valueTypeStr = CSUtility.Program.GetTypeSaveString(value.GetType());
                                    att.Write(valueTypeStr);
                                }
                            }
                        }
                        else
                        {
                            att.Write(xmlNode.GetNodes().Count);
                            foreach (var item in xmlNode.GetNodes())
                            {
                                foreach (var itemAttrib in item.GetAttribs())
                                {
                                    if (itemAttrib.Name == "Type")
                                    {
                                        //var itemType = CSUtility.Program.GetTypeSaveString(itemAttrib.Value);
                                        att.Write(itemAttrib.Value);
                                    }
                                }
                            }
                        }
                    }
                    else if (type.IsArray)
                    {
                        var array = obj as System.Array;
                        if (array != null)
                        {
                            int rank = array.Rank;
                            att.Write(rank);
                            for (int i = 0; i < rank; i++)
                            {
                                Int64 longLength = array.GetLongLength(i);
                                att.Write(longLength);
                            }

                            int length = array.Length;
                            att.Write(length);
                            foreach (var item in array)
                            {
                                if (item == null)
                                {
                                    att.Write("");
                                }
                                else
                                {
                                    var typeStr = CSUtility.Program.GetTypeSaveString(item.GetType());
                                    att.Write(typeStr);
                                }
                            }
                        }
                    }
                    else if (attrib.Name == "Ver")
                    {
                        UInt32 ver = 0;
                        if (UInt32.TryParse(attrib.Value, out ver))
                            att.Write(ver);
                    }
                    continue;
                }
                try
                {
                    if (type != null)
                    {
                        if (type == typeof(string))
                        {
                            att.Write(attrib.Value);
                        }
                        else if (type == typeof(Guid))
                        {
                            att.Write(Guid.Parse(attrib.Value));
                        }
                        else if (type == typeof(SByte))
                        {
                            att.Write(Convert.ToSByte(attrib.Value));
                        }
                        else if (type == typeof(Int16))
                        {
                            att.Write(Convert.ToInt16(attrib.Value));
                        }
                        else if (type == typeof(Int32))
                        {
                            att.Write(Convert.ToInt32(attrib.Value));
                        }
                        else if (type == typeof(System.DateTime))
                        {
                            att.Write(attrib.Value);
                        }
                        else if (type == typeof(Int64))
                        {
                            att.Write(Convert.ToInt64(attrib.Value));
                        }
                        else if (type == typeof(Byte))
                        {
                            att.Write(Convert.ToByte(attrib.Value));
                        }
                        else if (type == typeof(UInt16))
                        {
                            att.Write(Convert.ToUInt16(attrib.Value));
                        }
                        else if (type == typeof(UInt32))
                        {
                            att.Write(Convert.ToUInt32(attrib.Value));
                        }
                        else if (type == typeof(UInt64))
                        {
                            att.Write(Convert.ToUInt64(attrib.Value));
                        }
                        else if (type == typeof(Single))
                        {
                            att.Write(Convert.ToSingle(attrib.Value));
                        }
                        else if (type == typeof(Double))
                        {
                            att.Write(Convert.ToDouble(attrib.Value));
                        }
                        else if (type == typeof(bool))
                        {
                            att.Write(Convert.ToBoolean(attrib.Value));
                        }
                        else if (type == typeof(CSUtility.Support.Color))
                        {
                            att.Write(Convert.ToInt32(attrib.Value));
                        }
                        else if (type == typeof(System.Drawing.PointF))
                        {
                            var converter = new System.Drawing.PointConverter();
                            System.Drawing.Point pt = (System.Drawing.Point)converter.ConvertFromString(attrib.Value);
                            att.Write(new SlimDX.Vector2(pt.X, pt.Y));
                        }
                        else if (type.IsEnum)
                        {
                            att.Write(attrib.Value);
                        }
                        else if (type == typeof(SlimDX.Vector3))
                        {
                            att.Write(ConvertToVector3(attrib.Value));
                        }
                        else if (type == typeof(SlimDX.Vector4))
                        {
                            att.Write(ConvertToVector4(attrib.Value));
                        }
                        else if (type.GetInterface((typeof(IXndSaveLoadProxy)).FullName) != null)
                        {
                            att.Write(attrib.Value);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace.ToString());
                }
            }
            att.EndWrite();

            foreach (var node in xmlNode.GetNodes())
            {
                InitNewXNDNodeWithXMLNode(newXndNode, node, false);
            }
        }

        public static SlimDX.Vector3 ConvertToVector3(string value)
        {
            var stringValues = value.Split(',');
            if (stringValues.Length != 3)
                return SlimDX.Vector3.Zero;
            var xvalue = (stringValues[0]).Trim();
            var yvalue = (stringValues[1]).Trim();
            var zvalue = (stringValues[2]).Trim();
            float slimX = 0;
            float.TryParse(xvalue, out slimX);
            float slimY = 0;
            float.TryParse(yvalue, out slimY);
            float slimZ = 0;
            float.TryParse(zvalue, out slimZ);
            SlimDX.Vector3 res = new SlimDX.Vector3(slimX, slimY, slimZ);
            return res;
        }

        public static SlimDX.Vector4 ConvertToVector4(string value)
        {
            var stringValues = value.Split(',');
            if (stringValues.Length != 4)
                return SlimDX.Vector4.Zero;
            var xvalue = (stringValues[0]).Trim();
            var yvalue = (stringValues[1]).Trim();
            var zvalue = (stringValues[2]).Trim();
            var wvalue = (stringValues[3]).Trim();
            float slimX = 0;
            float.TryParse(xvalue, out slimX);
            float slimY = 0;
            float.TryParse(yvalue, out slimY);
            float slimZ = 0;
            float.TryParse(zvalue, out slimZ);
            float slimW = 0;
            float.TryParse(wvalue, out slimW);
            SlimDX.Vector4 res = new SlimDX.Vector4(slimX, slimY, slimZ, slimW);
            return res;
        }

        XmlHolder LoadXmlFile(string fileName)
        {
            if (fileName == "")
                return null;

            var holder = XmlHolder.LoadXML(fileName);
            return holder;
        }

        XndHolder LoadXndFile(string fileName)
        {
            if (fileName == "")
                return null;

            var holder = XndHolder.LoadXND(fileName);
            return holder;
        }

        private void Button_OpenImpotFileClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportFileName = fd.SelectedPath;
            }
        }

        void InitNewXMLNodeWithXNDNode(XmlNode xmlNode, XndNode xndNode, bool isRootNode)
        {
            XmlNode newXmlNode = null;
            if (isRootNode)
            {
                newXmlNode = xmlNode;
            }
            else
            {
                newXmlNode = xmlNode.AddNode(xndNode.GetName(), "", null);
            }
            foreach (var attrib in xndNode.GetAttribs())
            {
                attrib.BeginRead();
                
                //CSUtility.Support.XndSaveLoadProxy.Read(attrib);
                //attrib.Read()
                var att = newXmlNode.AddAttrib("");
                
                attrib.EndRead();
            }
        }
    }
}

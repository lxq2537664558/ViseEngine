using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace TemplateEditor
{
    public class ParentKeysInfo
    {
        public string Descrip;
        public string KeyName;
        public object value;
        public Type valueType;
        public string parentTableName;
    }


    public class ExportToExcel
    {
        //通过文件后缀打开文件
        const char cConnetTableString = '_';
        DataSet mDataset;
        string mExportFileName = "";
        //string mCurFileName = "";
        Dictionary<string, string> mTableNameToTypeName = new Dictionary<string, string>();   //<表名，类型名>
        Dictionary<string, string> mTableNameToDescrip = new Dictionary<string, string>();   //<表名，描述名字>
        Dictionary<string, string> mTableNameToOwnerTabel = new Dictionary<string, string>();   //<表名，拥有的表名字>
        //Dictionary<string, string> mTableNameToMainKey = new Dictionary<string, string>();   //<表名，主键>
        Dictionary<string, int> mRepeatTableName = new Dictionary<string, int>();   //<表名，主键>

        public void Clear()
        {
            mTableNameToTypeName.Clear();
            mTableNameToDescrip.Clear();
            mTableNameToOwnerTabel.Clear();
            mRepeatTableName.Clear();
        }

        public DataSet CreateDataSetFromFileBySuffix(TemplateEditorControl teCtrl, string suffix, string foldPath, out Type curType)
        {
            var files = System.IO.Directory.EnumerateFiles(foldPath);

            return _CreateDateSetFromFile(teCtrl, suffix, files, out curType);
        }

        public DataSet CreateDataSetFromFileBySuffix(TemplateEditorControl teCtrl, string suffix,string[] files, out Type curType)
        {

            return _CreateDateSetFromFile(teCtrl, suffix, files, out curType);
        }

        private DataSet _CreateDateSetFromFile(TemplateEditorControl teCtrl, string suffix/*后缀名*/, IEnumerable<string> files/*路径*/, out Type curType)
        {
            if (teCtrl == null)
            {
                curType = null;
                return new DataSet();
            }

            teCtrl.Dispatcher.Invoke(new Action(() =>
            {
                teCtrl.ListBox_Files.ItemsSource = files;
            }));

            curType = null;
            foreach (var type in CSUtility.Data.DataTemplateManagerAssist.Instance.DataTemplateManagerDictionary.Keys)
            {
                var atts = type.GetCustomAttributes(typeof(CSUtility.Data.DataTemplateAttribute), false);

                if (atts.Length > 0)
                {
                    var dea = atts[0] as CSUtility.Data.DataTemplateAttribute;
                    if (dea.FileExtension == suffix)
                    {
                        curType = type;
                        break;
                    }
                }
            }

            mDataset = new DataSet();
            mExportFileName = suffix.Substring(1);
            var rootTable = new DataTable("@_root");
            mDataset.Tables.Add(rootTable);
            if (curType == null)
            {
                return mDataset;
            }

            mTableNameToTypeName[rootTable.TableName] = curType.FullName;

            var column = new DataColumn();
            column.DataType = typeof(string);
            column.ColumnName = "@filename";
            column.Caption = "文件名";
            column.ReadOnly = false;
            column.Unique = false;
            rootTable.Columns.Add(column);


            _InitColumn(curType, rootTable,null);


            var keys = new Stack<ParentKeysInfo>();


            var props = curType.GetProperties();
            int idx = 1;
            foreach (var file in files)
            {
                teCtrl.Dispatcher.Invoke(new Action(() =>
                {
                    teCtrl.ExportState = "正在读取文件(" + idx + "/" + teCtrl.ListBox_Files.Items.Count + ") " + file;
                }));

                var dotIdx = file.LastIndexOf('.');
                if (dotIdx < 0)
                    continue;

                if (file.Substring(dotIdx) != suffix)
                    continue;

                var obj = System.Activator.CreateInstance(curType);
                CSUtility.Support.IConfigurator.FillProperty(obj, file);
                var row = rootTable.NewRow();
                row["@filename"] = file.Substring(file.LastIndexOf('\\') + 1);
                //mCurFileName = row["@filename"].ToString();

                ParentKeysInfo key = new ParentKeysInfo();
                key.KeyName = "@filename";
                key.Descrip = "文件名";
                key.value = row["@filename"];
                keys.Push(key);
                _FillRow(obj, row, props, -1, keys,null);
                rootTable.Rows.Add(row);
                keys.Pop();

                idx++;
            }

            return mDataset;
        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
        public void _InitColumn(Type curType, DataTable tb,string parentProName)
        {
            if (curType.IsSubclassOf(typeof(System.ValueType)))
            {
                var column = new DataColumn();
                column.DataType = curType;
                column.ColumnName = "@value";
                column.Caption = "值";
                column.ReadOnly = false;
                column.Unique = false;
                tb.Columns.Add(column);
                return;
            }

            var props = curType.GetProperties();
            foreach (var pro in props)
            {
                if (pro.CanWrite == false)
                    continue;
//                 if (pro.PropertyType.IsGenericType)
//                 {
//                     continue;
//                 }

                var attrs = pro.GetCustomAttributes(typeof(CSUtility.Support.DataValueAttribute), true);
                string displayname = "";
                if (pro.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true).Length > 0)
                {
                    displayname = (pro.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true)[0] as System.ComponentModel.DisplayNameAttribute).DisplayName;
                }
                if (attrs == null)
                    continue;
                if (attrs.Length <= 0)
                    continue;
                DataColumn column;
                if (pro.PropertyType.IsSubclassOf(typeof(System.ValueType))||pro.PropertyType==typeof(string) )
                {
                    column = new DataColumn();
                    column.DataType = pro.PropertyType;
                    if (parentProName!=null)
                        column.ColumnName = pro.Name + cConnetTableString + parentProName;
                    else
                        column.ColumnName = pro.Name;
                    column.Caption = displayname;
                    column.ReadOnly = false;
                    column.Unique = false;
                    tb.Columns.Add(column);
                }
                else
                {
                    _InitColumn(pro.PropertyType, tb, pro.Name);
                }
            }
        }

        public void _FillRow(object obj, DataRow row, System.Reflection.PropertyInfo[] props, int index, Stack<ParentKeysInfo> parentKeys,string parentFeildName)
        {
            foreach (var pro in props)
            {
                if (pro.CanWrite == false)
                    continue;
                if (pro.PropertyType.IsGenericType)
                {
                    var tbName = "@" + pro.Name;
                    var subType = pro.PropertyType.GetGenericArguments()[0];

                    string parentName = "";
                    if (mTableNameToOwnerTabel.ContainsKey(row.Table.TableName))
                    {
                        parentName = row.Table.TableName + cConnetTableString + mTableNameToOwnerTabel[row.Table.TableName];
                    }
                    else
                    {
                        parentName = row.Table.TableName;
                    }

                    DataTable subTb;
                    if (!mDataset.Tables.Contains(tbName))
                    {
                        subTb = _CreateDT(parentKeys, pro, tbName, subType, parentName);
                    }
                    else
                    {
                        if (mRepeatTableName.ContainsKey(tbName))
                        {//有重复的，那名字就不能简单判断
                            string reName = "";
                            foreach (string tn in mTableNameToOwnerTabel.Keys)
                            {
                                if (tbName.Length > tn.Length)
                                    continue;
                                if (tn.Substring(0, tbName.Length) != tbName)
                                    continue;

                                if (mTableNameToOwnerTabel[tn] == parentName)
                                {
                                    reName = tn;
                                    break;
                                }
                            }

                            if (reName == "")
                            {//说明这个父没出现过,是新的

                            }
                            else
                            {//已经搞过的表
                                tbName = reName;
                            }
                        }

                        if (mTableNameToOwnerTabel.ContainsKey(tbName) && mTableNameToOwnerTabel[tbName] == parentName)
                        {//表名和父级表明一致，代表已经创建过一样的表
                            subTb = mDataset.Tables[tbName];
                            if (subTb == null)
                            {
                                var parent = mTableNameToOwnerTabel[tbName];
                                if (mTableNameToOwnerTabel.ContainsKey(parentName) && mTableNameToOwnerTabel[parent] == mTableNameToOwnerTabel[parentName])
                                {
                                    if (mRepeatTableName.ContainsKey(tbName))
                                        mRepeatTableName[tbName] += 1;
                                    else
                                        mRepeatTableName[tbName] = 1;
                                    tbName += cConnetTableString + mRepeatTableName[tbName].ToString() + cConnetTableString + mRepeatTableName[tbName].ToString() + cConnetTableString + mRepeatTableName[tbName].ToString();    //加上区分
                                    subTb = _CreateDT(parentKeys, pro, tbName, subType, parentName);
                                }
                                else
                                {
                                    if (mRepeatTableName.ContainsKey(tbName))
                                        mRepeatTableName[tbName] += 1;
                                    else
                                        mRepeatTableName[tbName] = 1;
                                    tbName += cConnetTableString + mRepeatTableName[tbName].ToString() + cConnetTableString + mRepeatTableName[tbName].ToString();    //加上区分
                                    subTb = _CreateDT(parentKeys, pro, tbName, subType, parentName);
                                }

                            }
                        }
                        else
                        {//表名重复，但是父级表名不一样
                            if (mRepeatTableName.ContainsKey(tbName))
                                mRepeatTableName[tbName] += 1;
                            else
                                mRepeatTableName[tbName] = 1;
                            tbName += cConnetTableString + mRepeatTableName[tbName].ToString();    //加上区分
                            subTb = _CreateDT(parentKeys, pro, tbName, subType, parentName);
                        }

                    }

                    System.Reflection.PropertyInfo pi = pro.PropertyType.GetProperty("Count");
                    if (pi == null)
                        throw new ArgumentException(string.Format("类型错误,{0}", pro.PropertyType.ToString()));
                    System.Reflection.PropertyInfo itemPro = pro.PropertyType.GetProperty("Item");
                    if (itemPro == null)
                        throw new ArgumentException(string.Format("类型错误,{0}", pro.PropertyType.ToString()));
                    var listObj = pro.GetValue(obj, null);
                    int count = (int)pi.GetValue(listObj, null);

                    for (int i = 0; i < count; i++)
                    {
                        var subObj = itemPro.GetValue(listObj, new object[] { i });
                        var subRow = subTb.NewRow();
                        foreach (var pkey in parentKeys)
                        {
                            subRow[pkey.KeyName] = pkey.value;
                        }

                        if (subType.IsSubclassOf(typeof(System.ValueType)))
                        {
                            subRow["@value"] = subObj;
                        }

                        var parent = parentKeys.Peek();
                        ParentKeysInfo keyInfo = new ParentKeysInfo();
                        parentKeys.Push(keyInfo);
                        keyInfo.Descrip = subRow.Table.TableName + "_序号";
                        keyInfo.value = i;
                        keyInfo.KeyName = subRow.Table.TableName + "_index";
                        _FillRow(subObj, subRow, subType.GetProperties(), i, parentKeys,null);
                        subRow["@index"] = i;
                        subTb.Rows.Add(subRow);
                        parentKeys.Pop();
                    }
                    continue;
                }

                var attrs = pro.GetCustomAttributes(typeof(CSUtility.Support.DataValueAttribute), true);
                if (attrs == null)
                    continue;
                if (attrs.Length <= 0)
                    continue;

                if (pro.PropertyType.IsSubclassOf(typeof(System.ValueType)) || pro.PropertyType == typeof(string))
                {
                    object value = pro.GetValue(obj, null);
                    if (parentFeildName==null)
                        row[pro.Name] = value;
                    else
                        row[pro.Name + cConnetTableString + parentFeildName] = value;
                }
                else
                {
                    var sub = pro.GetValue(obj, null);
                    _FillRow(sub, row, pro.PropertyType.GetProperties(), index, parentKeys, pro.Name);
                }

            }
        }

        private DataTable _CreateDT(Stack<ParentKeysInfo> parentKeys, System.Reflection.PropertyInfo pro, string tbName, Type subType, string parentName)
        {
            DataTable subTb;
            subTb = new DataTable(tbName);
            mDataset.Tables.Add(subTb);

            mTableNameToOwnerTabel[tbName] = parentName;
            if (pro.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true).Length > 0)
                mTableNameToDescrip[tbName] = (pro.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true)[0] as System.ComponentModel.DisplayNameAttribute).DisplayName;
            else
                mTableNameToDescrip[tbName] = "";

            mTableNameToTypeName[tbName] = subType.ToString();

            DataColumn column;
            var ar = parentKeys.ToArray();
            for (int i = parentKeys.Count - 1; i >= 0; i--)
            {
                var pkey = ar[i];
                column = new DataColumn();
                column.DataType = pkey.value.GetType();
                column.ColumnName = pkey.KeyName;
                column.Caption = pkey.Descrip;
                column.ReadOnly = false;
                column.Unique = false;
                subTb.Columns.Add(column);
            }

            column = new DataColumn();
            column.DataType = typeof(Int32);
            column.ColumnName = "@index";
            column.Caption = "序号";
            column.ReadOnly = false;
            column.Unique = false;
            subTb.Columns.Add(column);

            _InitColumn(subType, subTb,null);
            return subTb;
        }

        //// <summary>
        /// 执行导出
        /// </summary>
        /// <param name="ds">要导出的DataSet</param>
        public void DatasetExportToExcel(TemplateEditorControl ctrl, DataSet ds, string path)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.DisplayAlerts = false;  //是否需要显示提示
            excel.AlertBeforeOverwriting = false;  //是否弹出提示覆盖
            excel.SheetsInNewWorkbook = ds.Tables.Count;    //设置有几个初始sheet

            Microsoft.Office.Interop.Excel.Workbook xlBook = excel.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet[] workSheet = new Microsoft.Office.Interop.Excel.Worksheet[ds.Tables.Count];
            int curSheetIndex = 0;
            foreach (DataTable table in ds.Tables)
            {
                int rowIndex = 4;
                int colIndex = 0;
                workSheet[curSheetIndex] = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.Add();
                workSheet[curSheetIndex].Name = ds.Tables[curSheetIndex].TableName;
                workSheet[curSheetIndex].Cells[1, 1] = mTableNameToTypeName[ds.Tables[curSheetIndex].TableName];
                //if (mTableNameToMainKey.ContainsKey(ds.Tables[curSheetIndex].TableName))
                //{
                //    workSheet[curSheetIndex].Cells[1, 2] = mTableNameToMainKey[ds.Tables[curSheetIndex].TableName];
                //}

                if (mTableNameToDescrip.ContainsKey(ds.Tables[curSheetIndex].TableName))
                {
                    workSheet[curSheetIndex].Cells[1, 8] = mTableNameToDescrip[ds.Tables[curSheetIndex].TableName];
                }

                if (mTableNameToOwnerTabel.ContainsKey(ds.Tables[curSheetIndex].TableName))
                {
                    workSheet[curSheetIndex].Cells[1, 10] = mTableNameToOwnerTabel[ds.Tables[curSheetIndex].TableName];
                }




                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.IsGenericType)
                    {

                        continue;
                    }
                    colIndex++;
                    workSheet[curSheetIndex].Cells[2, colIndex] = col.ColumnName;
                    workSheet[curSheetIndex].Cells[3, colIndex] = col.DataType.ToString();
                    workSheet[curSheetIndex].Cells[4, colIndex] = col.Caption;
                }

                foreach (DataRow row in table.Rows)
                {
                    rowIndex++;
                    colIndex = 0;
                    foreach (DataColumn col in table.Columns)
                    {
                        colIndex++;
                        var showVal = row[col.ColumnName].ToString();
                        if (col.DataType == typeof(System.Drawing.PointF))
                        {
                            var rangee = (Microsoft.Office.Interop.Excel.Range)workSheet[curSheetIndex].Cells[rowIndex, colIndex];
                            rangee.NumberFormatLocal = "@";
                            System.Drawing.PointF pt = (System.Drawing.PointF)row[col.ColumnName];
                            showVal = string.Format("{0},{1}", pt.X, pt.Y);
                        }
                        else if(col.DataType == typeof(System.Drawing.Point))
                        {
                            var rangee = (Microsoft.Office.Interop.Excel.Range)workSheet[curSheetIndex].Cells[rowIndex, colIndex];
                            rangee.NumberFormatLocal = "@";
                            System.Drawing.Point pt = (System.Drawing.Point)row[col.ColumnName];
                            showVal = string.Format("{0},{1}", pt.X, pt.Y);
                        }

                        workSheet[curSheetIndex].Cells[rowIndex, colIndex] = showVal;
                    }

                    ctrl.Dispatcher.Invoke(new Action(() =>
                    {
                        ctrl.ExportState = "正在导出到Excel 表(" + (curSheetIndex + 1) + "/" + ds.Tables.Count + ") " + table.TableName + " 行(" + rowIndex + "/" + table.Rows.Count + ")";
                    }));
                }
                curSheetIndex++;
            }
            excel.Visible = false;
            try
            {
                excel.ActiveWorkbook.SaveAs(string.Format("{1}\\{0}.xlsx", mExportFileName, path));
            }
            catch (System.Exception)
            {
                EditorCommon.MessageBox.Show("excel导出失败，可能目标文件已打开，无法保存，请重试");            	
            }


            excel.Workbooks.Close();
            excel.Quit();

            KillExcel(excel);
        }

        private static void KillExcel(Microsoft.Office.Interop.Excel.Application excel)
        {
            if (excel != null)
            {
                int lpdwProcessId;
                GetWindowThreadProcessId((IntPtr)excel.Hwnd, out lpdwProcessId);

                if (lpdwProcessId > 0)    //c-s方式
                {
                    System.Diagnostics.Process.GetProcessById(lpdwProcessId).Kill();
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        
        public bool FillObject(object obj, System.Data.DataRow row, Stack<ParentKeysInfo> parentKeys,string parentProName, TemplateEditorControl teCtrl)
        {
            if (teCtrl == null)
                return false;

            Type objType = obj.GetType();

            //if (objType.IsSubclassOf(typeof(System.ValueType)) || objType == typeof(string))
            //{
            //    GetObjValue(row, , objType, parentProName);
            //    _SetObjValue(obj, row, p, parentProName);
            //}
            //else
            {

                var props = objType.GetProperties();
                foreach (var p in props)
                {
                    if (p.PropertyType.IsGenericType)
                    {
                        if (row.Table.DataSet.Tables.IndexOf("@" + p.Name) < 0)
                            continue;
                        var subType = p.PropertyType.GetGenericArguments()[0];

                        System.Reflection.MethodInfo addPro = p.PropertyType.GetMethod("Add");
                        System.Reflection.MethodInfo clearMethod = p.PropertyType.GetMethod("Clear");
                        if (addPro == null || clearMethod == null)
                            throw new ArgumentException(string.Format("类型错误,{0}", p.PropertyType.ToString()));

                        string tbName = "@" + p.Name;

                        string parentTbName = "";
                        foreach(var k in parentKeys)
                        {
                            parentTbName += k.parentTableName + "_";
                        }
                        if(parentKeys.Count>0)
                            parentTbName = parentTbName.Remove(parentTbName.Length - 1);

                        tbName = _GetRepeatTableName(tbName, parentTbName);

                        var subTable = row.Table.DataSet.Tables[tbName];
                        //var mainkeyPro = objType.GetProperty(mTableNameToMainKey[row.Table.TableName]);

                        string sqlCondition = "";
                        bool needSql = true;
                        foreach (var info in parentKeys)
                        {
                            if (info.valueType == typeof(string))
                                sqlCondition += string.Format("{0}='{1}' and ", info.KeyName, info.value);
                            else
                                sqlCondition += string.Format("{0}='{1}' and ", info.KeyName, info.value);
                            if (subTable.Columns.IndexOf(info.KeyName)<0)
                            {
                                needSql = false;
                            }
                        }
                        if (needSql)
                        {
                            sqlCondition = sqlCondition.Remove(sqlCondition.LastIndexOf("and "));
                            DataRow[] subDataRows = subTable.Select(sqlCondition);
                            var listObj = p.GetValue(obj, null);
                            //clearMethod.Invoke(listObj, null);
                            foreach (var subrow in subDataRows)
                            {
                                object subObj;
                                if (subType.IsSubclassOf(typeof(System.ValueType)) || subType == typeof(string))
                                {
                                    subObj = GetObjValue(subrow, "@value", subType, parentProName);
                                }
                                else
                                {
                                    subObj = System.Activator.CreateInstance(subType);
                                    if (subObj == null && subType.IsSubclassOf(typeof(System.ValueType)))
                                    {
                                        _AddPropertyInvoke(subType, addPro, listObj, subrow);
                                        continue;
                                    }

                                    ParentKeysInfo key = new ParentKeysInfo();
                                    key.KeyName = subrow.Table.TableName + "_index";
                                    key.value = subrow["@index"];
                                    key.valueType = typeof(int);
                                    key.parentTableName = subrow.Table.TableName;
                                    parentKeys.Push(key);

                                    //if()
                                    FillObject(subObj, subrow, parentKeys, null, teCtrl);
                                    parentKeys.Pop();
                                }
                                addPro.Invoke(listObj, new object[] { subObj });
                            }
                        }

                    }

                    if (p.PropertyType.IsSubclassOf(typeof(System.ValueType)) || p.PropertyType == typeof(string))
                    {

                        _SetObjValue(obj, row, p, parentProName);
                    }
                    else
                    {
                        if (row.Table.Columns.IndexOf(p.Name) < 0)
                        {
                            List<string> pNames = new List<string>();
                            foreach(DataColumn col in row.Table.Columns)
                            {
                                var s = col.ColumnName.Split(cConnetTableString);
                                if (s.Length < 2)
                                    continue;
                                pNames.AddRange(s);
                                pNames.Remove(s[0]);
                            }
                            if (pNames.IndexOf(p.Name)<0)
                                continue;
                        }
                        var sub = p.GetValue(obj, null);
                        FillObject(sub, row, parentKeys, p.Name, teCtrl);
                    }
                }

            }
            return true;
        }

        private static void _AddPropertyInvoke(Type subType, System.Reflection.MethodInfo addPro, object listObj, DataRow subrow)
        {
            if (subType == typeof(string))
                addPro.Invoke(listObj, new object[] { subrow["@value"].ToString() });
            else if (subType == typeof(SByte))
                addPro.Invoke(listObj, new object[] { System.Convert.ToSByte(subrow["@value"]) });
            else if (subType == typeof(Int16))
                addPro.Invoke(listObj, new object[] { System.Convert.ToInt16(subrow["@value"]) });
            else if (subType == typeof(Int32))
                addPro.Invoke(listObj, new object[] { System.Convert.ToInt32(subrow["@value"]) });
            else if (subType == typeof(Int64))
                addPro.Invoke(listObj, new object[] { System.Convert.ToInt64(subrow["@value"]) });
            else if (subType == typeof(Byte))
                addPro.Invoke(listObj, new object[] { System.Convert.ToByte(subrow["@value"]) });
            else if (subType == typeof(UInt16))
                addPro.Invoke(listObj, new object[] { System.Convert.ToUInt16(subrow["@value"]) });
            else if (subType == typeof(UInt32))
                addPro.Invoke(listObj, new object[] { System.Convert.ToUInt32(subrow["@value"]) });
            else if (subType == typeof(UInt64))
                addPro.Invoke(listObj, new object[] { System.Convert.ToUInt64(subrow["@value"]) });
            else if (subType == typeof(Single))
                addPro.Invoke(listObj, new object[] { System.Convert.ToSingle(subrow["@value"]) });
            else if (subType == typeof(Double))
                addPro.Invoke(listObj, new object[] { System.Convert.ToDouble(subrow["@value"]) });
            else if (subType == typeof(Guid))
                addPro.Invoke(listObj, new object[] { Guid.Parse(subrow["@value"].ToString()) });
            else if (subType.IsEnum)
                addPro.Invoke(listObj, new object[] { System.Convert.ToInt32(subrow["@value"]) });
            else if (subType == typeof(SlimDX.Vector3))
                addPro.Invoke(listObj, new object[] { ConvertToVector3(subrow["@value"].ToString()) });
            else if (subType == typeof(SlimDX.Vector4))
                addPro.Invoke(listObj, new object[] { ConvertToVector4(subrow["@value"].ToString()) });
        }

        private static Object GetObjValue(System.Data.DataRow row, string colName, Type objType, string parentProName)
        {
            if (parentProName != null)
                colName += "-" + parentProName;

            if (row.Table.Columns.IndexOf(colName) < 0)
                return null;

            try
            {
                if (objType == typeof(string))
                    return row[colName].ToString();
                else if (objType == typeof(Guid))
                    return Guid.Parse(row[colName].ToString());
                else if (objType == typeof(SByte))
                    return System.Convert.ToSByte(row[colName]);
                else if (objType == typeof(Int16))
                    return System.Convert.ToInt16(row[colName]);
                else if (objType == typeof(Int32))
                    return System.Convert.ToInt32(row[colName]);
                else if (objType == typeof(Int64))
                    return System.Convert.ToInt64(row[colName]);
                else if (objType == typeof(Byte))
                    return System.Convert.ToByte(row[colName]);
                else if (objType == typeof(UInt16))
                    return System.Convert.ToUInt16(row[colName]);
                else if (objType == typeof(UInt32))
                    return System.Convert.ToUInt32(row[colName]);
                else if (objType == typeof(UInt64))
                    return System.Convert.ToUInt64(row[colName]);
                else if (objType == typeof(Single))
                    return System.Convert.ToSingle(row[colName]);
                else if (objType == typeof(Double))
                    return System.Convert.ToDouble(row[colName]);
                else if (objType == typeof(bool))
                {
                    string b = row[colName].ToString();
                    if (b.ToLower() == "true")
                        return true;
                    else
                        return false;
                }
                else if (objType == typeof(System.DateTime))
                    return DateTime.Parse(row[colName].ToString());
                else if (objType == typeof(SlimDX.Vector3))
                {
                    var strvalue = System.Convert.ToString(row[colName]);
                    return  ConvertToVector3(strvalue);
                }
                else if (objType == typeof(SlimDX.Vector4))
                {
                    var strvalue = System.Convert.ToString(row[colName]);
                    return ConvertToVector4(strvalue);
                }
                else if (objType == typeof(System.Drawing.PointF))
                {
                    var converter = new System.Drawing.PointConverter();
                    System.Drawing.Point pt = (System.Drawing.Point)converter.ConvertFromString(row[colName].ToString());
                    return new System.Drawing.PointF(pt.X, pt.Y);
                }
                else if (objType.IsEnum)
                    return System.Convert.ToInt32(row[colName]);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace.ToString());
            }

            return null;
        }

        private static void _SetObjValue(object obj, System.Data.DataRow row, System.Reflection.PropertyInfo p, string parentProName)
        {
            string colName = p.Name;
            if (parentProName != null)
                colName += "_" + parentProName;
            if (row.Table.Columns.IndexOf(colName) < 0)
                return;

            //if (p.PropertyType.IsEnum)
            //{
            //    p.SetValue(obj, System.Convert.ToInt32(row[colName]), null);
            //}            
            //else
            //{
            //    var converter = System.ComponentModel.TypeDescriptor.GetConverter(p.PropertyType);
            //    object cov = converter.ConvertFromString(row[colName].ToString());
            //    p.SetValue(obj, cov, null);
            //}            

            try
            {
                if (p.PropertyType == typeof(string))
                {
                    p.SetValue(obj, row[colName].ToString(), null);
                }
                else if (p.PropertyType == typeof(Guid))
                {
                    p.SetValue(obj, Guid.Parse(row[colName].ToString()), null);
                }
                else if (p.PropertyType == typeof(SByte))
                {
                    p.SetValue(obj, (SByte)System.Convert.ToSByte(row[colName]), null);
                }
                else if (p.PropertyType == typeof(Int16))
                {
                    p.SetValue(obj, (Int16)System.Convert.ToInt16(row[colName]), null);
                }
                else if (p.PropertyType == typeof(Int32))
                {
                    p.SetValue(obj, System.Convert.ToInt32(row[colName]), null);
                }
                else if (p.PropertyType == typeof(System.DateTime))
                {
                    p.SetValue(obj, DateTime.Parse(row[colName].ToString()), null);
                }
                else if (p.PropertyType == typeof(Int64))
                {
                    p.SetValue(obj, System.Convert.ToInt64(row[colName]), null);
                }
                else if (p.PropertyType == typeof(Byte))
                {
                    p.SetValue(obj, (Byte)System.Convert.ToByte(row[colName]), null);
                }
                else if (p.PropertyType == typeof(UInt16))
                {
                    p.SetValue(obj, (UInt16)System.Convert.ToUInt16(row[colName]), null);
                }
                else if (p.PropertyType == typeof(UInt32))
                {
                    p.SetValue(obj, System.Convert.ToUInt32(row[colName]), null);
                }
                else if (p.PropertyType == typeof(UInt64))
                {
                    p.SetValue(obj, System.Convert.ToUInt64(row[colName]), null);
                }
                else if (p.PropertyType == typeof(Single))
                {
                    p.SetValue(obj, System.Convert.ToSingle(row[colName]), null);
                }
                else if (p.PropertyType == typeof(Double))
                {
                    p.SetValue(obj, System.Convert.ToDouble(row[colName]), null);
                }
                else if (p.PropertyType == typeof(bool))
                {
                    string b = row[colName].ToString();
                    if (b.ToLower() == "true")
                        p.SetValue(obj, true, null);
                    else
                        p.SetValue(obj, false, null);
                }
                else if (p.PropertyType == typeof(System.Drawing.PointF))
                {
                    var converter = new System.Drawing.PointConverter();
                    System.Drawing.Point pt = (System.Drawing.Point)converter.ConvertFromString(row[colName].ToString());
                    p.SetValue(obj, new System.Drawing.PointF(pt.X, pt.Y), null);
                }
                else if (p.PropertyType.IsEnum)
                {
                    p.SetValue(obj, System.Convert.ToInt32(row[colName]), null);
                }
                else if(p.PropertyType ==typeof(SlimDX.Vector3))
                {
                    var strvalue = System.Convert.ToString(row[colName]);
                    p.SetValue(obj,  ConvertToVector3(strvalue), null);
                }
                else if (p.PropertyType == typeof(SlimDX.Vector4))
                {
                    var strvalue = System.Convert.ToString(row[colName]);
                    p.SetValue(obj, ConvertToVector4(strvalue), null);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace.ToString());
            }
        }

        public static SlimDX.Vector3 ConvertToVector3(string value)
        {
            var xIndex = value.IndexOf("X",System.StringComparison.OrdinalIgnoreCase);
            var yIndex = value.IndexOf("Y", System.StringComparison.OrdinalIgnoreCase);
            var zIndex = value.IndexOf("Z", System.StringComparison.OrdinalIgnoreCase);
            var xvalue = (value.Substring(xIndex + 2, yIndex - xIndex - 2)).Trim();
            var yvalue = (value.Substring(yIndex + 2, zIndex - yIndex - 2)).Trim();
            var zvalue = (value.Substring(zIndex +2)).Trim();
            float slimX =0;
            float.TryParse(xvalue,out slimX);
            float slimY = 0;
            float.TryParse(yvalue, out slimY);
            float slimZ = 0;
            float.TryParse(zvalue, out slimZ);
            SlimDX.Vector3 res = new SlimDX.Vector3(slimX,slimY,slimZ);
            return res;
        }

        public static SlimDX.Vector4 ConvertToVector4(string value)
        {
            var xIndex = value.IndexOf("X", System.StringComparison.OrdinalIgnoreCase);
            var yIndex = value.IndexOf("Y", System.StringComparison.OrdinalIgnoreCase);
            var zIndex = value.IndexOf("Z", System.StringComparison.OrdinalIgnoreCase);
            var wIndex = value.IndexOf("W", System.StringComparison.OrdinalIgnoreCase);
            var xvalue = (value.Substring(xIndex + 2, yIndex - xIndex - 2)).Trim();
            var yvalue = (value.Substring(yIndex + 2, zIndex - yIndex - 2)).Trim();

            var zvalue = (value.Substring(zIndex + 2, wIndex - zIndex - 2)).Trim();
            var wvalue = (value.Substring(wIndex + 2)).Trim();
            float slimX = 0;
            float.TryParse(xvalue, out slimX);
            float slimY = 0;
            float.TryParse(yvalue, out slimY);
            float slimZ = 0;
            float.TryParse(zvalue, out slimZ);
            float slimW = 0;
            float.TryParse(wvalue, out slimW);

            SlimDX.Vector4 res = new SlimDX.Vector4(slimX, slimY, slimZ,slimW);
            return res;
        }

        public class EditorWorkSheet
        {
            Microsoft.Office.Interop.Excel.Worksheet workSheet = null;
            TemplateEditorControl editorctrl = null;
            DataTable table = null;
            public bool HaveFinishedInit = false;
            public string Name
            {
                get { return workSheet.Name; }
            }

            public string Percent = "";
            public string threadPer = "";

            public void Initialize(ref Microsoft.Office.Interop.Excel.Worksheet sheet, ref DataTable dtable, TemplateEditorControl ctrl)
            {
                workSheet = sheet;
                table = dtable;
                editorctrl = ctrl;
               
                ProcessInitWorkSheet();
                //InitWorkSheetData();
            }

            public void ProcessInitWorkSheet()
            {
                Thread thread = new Thread(new ThreadStart(InitWorkSheetDataSingle));
                thread.IsBackground = true;
                thread.Start();
            }

            public void InitWorkSheetStruct()
            {
                int colIndex = 1;
                while (true)//第二行
                {
                    var c1 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[2, colIndex];
                    if (c1 == Type.Missing || c1.Value == null)
                    {
                        break;
                    }
                    string ColumnName = c1.Value2;

                    var col = new DataColumn();
                    col.ColumnName = ColumnName;
                    col.ReadOnly = false;
                    col.Unique = false;
                    table.Columns.Add(col);
                    colIndex++;
                }
            }


            int mMaxRowInOneThread = 600;
            int mMaxColInOneThread = 8;

            public void InitWorkSheetData()
            {
                var r1 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[5, mMaxColInOneThread];
                if (r1 == Type.Missing || r1.Value == null)
                {
                    var c1 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[mMaxRowInOneThread, 1];
                    if (c1 == Type.Missing || c1.Value == null)
                    {
                        InitWorkSheetDataSingle();
                        //ProcessInitWorkSheet();
                    }
                    else
                    {
                        InitWorkSheetDataThreed();
                    }
                }
                else
                {
                    mMaxRowInOneThread = 5;
                    InitWorkSheetDataThreed();
                }
            }

            public void InitWorkSheetDataSingle()
            {
                InitWorkSheetStruct();
                int rowIndex = 5;
                while (true)
                {
                    var c1 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1];

                    if (c1 == Type.Missing || c1.Value == null)
                    {
                        break;
                    }
                    var row = table.NewRow();
                    int colIndex = 1;
                    foreach (DataColumn col in table.Columns)
                    {
                        var c = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, colIndex];
                        row[col.ColumnName] = c.Value;
                        colIndex++;
                    }
                    table.Rows.Add(row);
                    rowIndex++;
                    Percent = rowIndex.ToString();
                }
                editorctrl.Dispatcher.Invoke(new Action(() =>
                {
                    editorctrl.ImportState = "表" + workSheet.Name + "导入完成";
                }));
                HaveFinishedInit = true;
            }

            public void InitWorkSheetDataThreed()
            {
                int rowIn = 5;
                List<TableThread> tablethreads = new List<TableThread>();
                while (true)
                {
                    var ctheed = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIn, 1];
                    if (ctheed == Type.Missing || ctheed.Value == null)
                    {
                        break;
                    }
                    else
                    {
                        TableThread mtablethread = new TableThread();
                        tablethreads.Add(mtablethread);
                        mtablethread.Init(ref workSheet, ref table, editorctrl, rowIn, mMaxRowInOneThread);
                    }
                    rowIn += mMaxRowInOneThread;
                }
                while(true)
                {
                    bool isbreak =true;
                    foreach(var thr in tablethreads)
                    {
                        if (thr.HaveFinishedInit ==false)
                        {
                            isbreak = false;
                        }
                        else
                        {
                            if (thr.DataHaveAdd ==false)
                            {
                                foreach (var row in thr.mReadyAddRows)
                                {
                                    table.Rows.Add(row);
                                }
                                thr.DataHaveAdd = true;
                            }
                        }
                    }
                    threadPer = "(table线程"+tablethreads.Count +")";
                    if (isbreak)
                    {
                        break;
                    }
                }
                HaveFinishedInit = true;
            }
        }

        public class TableThread
        {
            int beginrow = 5;
            int mMaxRowInOneThread = 600;
            Microsoft.Office.Interop.Excel.Worksheet workSheet = null;
            TemplateEditorControl editorctrl = null;
            DataTable table = null;
            public bool HaveFinishedInit = false;
            public bool DataHaveAdd = false;
            public string percent = "";
            public List<System.Data.DataRow> mReadyAddRows = new List<System.Data.DataRow>();

            public void Init(ref Microsoft.Office.Interop.Excel.Worksheet sheet, ref DataTable dtable, TemplateEditorControl ctrl,int currow , int maxrow)
            {
                workSheet = sheet;
                table = dtable;
                editorctrl = ctrl;
                beginrow = currow;
                mMaxRowInOneThread = maxrow;
                Thread thread = new Thread(new ThreadStart(InitWorkSheetData));
                thread.IsBackground = true;
                thread.Start();
            }

            public void InitWorkSheetData()
            {
                int rowIndex = beginrow;
                while (true)
                {
                    var c1 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1];

                    if (c1 == Type.Missing || c1.Value == null || beginrow - rowIndex >= mMaxRowInOneThread)
                    {
                        break;
                    }
                    var row = table.NewRow();
                    int colIndex = 1;
                    foreach (DataColumn col in table.Columns)
                    {
                        var c = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, colIndex];
                        row[col.ColumnName] = c.Value;
                        colIndex++;
                    }
//                     lock (table)
//                     {
//                         table.Rows.Add(row);
//                     }
                    mReadyAddRows.Add(row);
                    rowIndex++;
                    percent ="线程" +beginrow.ToString() + rowIndex.ToString();
                }
                HaveFinishedInit = true;

            }
        }


        public DataSet CreateDataSetFromExcel(string fileName, TemplateEditorControl ctrl)
        {
            mTableNameToDescrip.Clear();
            mTableNameToOwnerTabel.Clear();
            mTableNameToTypeName.Clear();
            mRepeatTableName.Clear();
            
            mDataset = new DataSet();
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlBook = excel.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            
            List<EditorWorkSheet> worksheeds =new List<EditorWorkSheet>();
            for (int tbi = 0; tbi < xlBook.Worksheets.Count; tbi++)
            {
                var workSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item(tbi + 1);
                if (workSheet.Name[0] != '@')
                    continue;

                DataTable table = new DataTable();
                mDataset.Tables.Add(table);
                table.TableName = workSheet.Name;
                mTableNameToTypeName[table.TableName] = ((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[1, 1]).Value;
                mTableNameToOwnerTabel[table.TableName] = ((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[1, 10]).Value;

                
                EditorWorkSheet editorWorksheet = new EditorWorkSheet();
                worksheeds.Add(editorWorksheet);
                editorWorksheet.Initialize(ref workSheet,ref table, ctrl);

//                 int colIndex = 1;
//                 while (true)//第二行
//                 {
//                     var c1 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[2, colIndex];
//                     if (c1 == Type.Missing || c1.Value == null)
//                     {
//                         break;
//                     }
//                     string ColumnName = c1.Value2;
// 
//                     var col = new DataColumn();
//                 //    var c2 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[3, colIndex];
//                     //col.DataType = Type.GetType(c2.Value2);
// 
//                     col.ColumnName = ColumnName;
//                     col.ReadOnly = false;
//                     col.Unique = false;
//                     table.Columns.Add(col);
//                     colIndex++;
// 
//                     ctrl.Dispatcher.Invoke(new Action(() =>
//                     {
//                         ctrl.ImportState = "正在生成表(" + (tbi + 1) + "/" + xlBook.Worksheets.Count + ") " + workSheet.Name + " 的第 " + colIndex + " 列!";
//                     }));
//                 }

//                 int rowIndex = 5;
//                 while (true)
//                 {
//                     var c1 = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, 1];
// 
//                     if (c1 == Type.Missing || c1.Value == null)
//                     {
//                         break;
//                     }
//                     var row = table.NewRow();
//                     int colIndex = 1;
//                     foreach (DataColumn col in table.Columns)
//                     {
//                         var c = (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[rowIndex, colIndex];
//                         row[col.ColumnName] = c.Value;
//                         colIndex++;
//                     }
//                     table.Rows.Add(row);
//                     rowIndex++;
// 
                ctrl.Dispatcher.Invoke(new Action(() =>
                {
                    ctrl.ImportState = "正在导入表(" + (tbi + 1) + "/" + xlBook.Worksheets.Count + ") " + workSheet.Name + " 的第 " /*+ rowIndex */+ " 行数据!";
                }));
//                 }

                //var percent = tbi * 1.0 / xlBook.Worksheets.Count;
                //ctrl.Dispatcher.Invoke(new Action(() => 
                //{
                //    ctrl.ImportPercent = percent;
                //}));
            }


            while(true)
            {
                bool ifbreak =true;
                int finishcount = 0;
                List<string> unFinishName = new List<string>();
                foreach (var sheet in worksheeds)
                {
                    if (sheet.HaveFinishedInit ==false)
                    {
                        string info = sheet.Name + sheet.Percent + sheet.threadPer;
                        unFinishName.Add(info);
                        ifbreak = false;
                    }
                    else
                    {
                        finishcount++;
                    }
                }
                ctrl.Dispatcher.Invoke(new Action(() =>
                {
                    if (worksheeds.Count -finishcount >=10)
                    {
                        ctrl.ImportState = "----完成度（" + finishcount + "/" + worksheeds.Count + ")";
                    }
                    else
                    {
                        string showname = "";
                        foreach (var na in unFinishName)
                        {
                            showname += ","+na ;
                        }
                        ctrl.ImportState = "----完成度（" + finishcount + "/" + worksheeds.Count + ")" +"未完成：" +showname;
                    }
                    
                }));

                finishcount =0;
                if (ifbreak)
                {
                    worksheeds.Clear();
                    break;
                }
            }

            excel.Visible = false;
            excel.Workbooks.Close();
            excel.Quit();
            KillExcel(excel);
            return mDataset;
        }

        public string _GetRepeatTableName(string nativeName,string parentName)
        {
            string reName = nativeName;
            foreach (string tn in mTableNameToOwnerTabel.Keys)
            {
                if (nativeName.Length > tn.Length)
                    continue;
                if (tn.Substring(0, nativeName.Length) != nativeName)
                    continue;
                if (nativeName.Length < tn.Length)
                {
                    if (tn.Substring(nativeName.Length, 1) !="_")
                    {
                        continue;
                    }
//                     if (!BoolTableNameIsNumber(tn.Substring(nativeName.Length, 1)))//去除名称部分相同的
//                         continue;
                }
                if (mTableNameToOwnerTabel[tn] == parentName)
                {
                    reName = tn;
                    break;
                }
            }
            return reName;
        }

//         bool BoolTableNameIsNumber(string name)
//         {
//             try
//             {
//                 var num =System.Convert.ToInt32(name);
//                 return true;
//             }
//             catch (System.Exception ex)
//             {
//                 return false;
//             }
//         }

        public void DatasetExportToTemplateFile(string templatePath, TemplateEditorControl ctrl)
        {
            var parentKeys = new Stack<ParentKeysInfo>();

            int idx = 1;
            foreach (DataRow fileRow in mDataset.Tables["@_root"].Rows)
            {
                var type = CSUtility.Program.GetTypeFromTypeFullName(mTableNameToTypeName["@_root"]);
                if (type == null)
                    continue;
                object obj = System.Activator.CreateInstance(type);
                //mCurFileName = fileRow["@filename"].ToString();
                ParentKeysInfo key = new ParentKeysInfo();
                key.KeyName = "@filename";
                key.value = fileRow["@filename"];
                key.valueType = typeof(string);
                key.parentTableName = fileRow.Table.TableName;
                parentKeys.Push(key);
                FillObject(obj, fileRow, parentKeys,null, ctrl);
                parentKeys.Pop();
                string OpendFileName = templatePath + "\\" + fileRow["@filename"];
                var relFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(OpendFileName);
                CSUtility.Support.IConfigurator.SaveProperty(obj, obj.GetType().ToString(), relFileName);

                ctrl.Dispatcher.Invoke(new Action(() =>
                {
                    ctrl.ImportState = "正在保存文件(" + idx + "/" + mDataset.Tables["@_root"].Rows + ") " + relFileName;
                }));

                idx++;
            }
 
        }
    }
}


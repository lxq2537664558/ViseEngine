using System;
using System.Collections.Generic;
using System.Linq;

namespace SCore.DB
{
    public class DBConnect
    {
        #region 数据库基础操作
        const string EndLine = "\r\n";
        System.Data.SqlClient.SqlConnection mConnection;
        System.Data.SqlClient.SqlDataAdapter mDataAdapter;

        public string DBConnectStr = System.String.Format("server={0};database=zeus;uid=ZeusServer;pwd=123imgod;MultipleActiveResultSets=True", "192.168.1.139");

        string mDBName;
        public bool OpenConnect(string dbIp, string dbName, string uid = "ZeusServer", string psw = "123imgod")
        {
            mDBName = dbName;
            string name;
            //if (IDataServer.Instance != null)
                name = System.String.Format("server={0};database={1};uid={2};pwd={3};MultipleActiveResultSets=True", dbIp, dbName, uid, psw);
            //else
                //name = DBConnectStr;

            try
            {
                //name = "server=192.168.2.71;database=Kiang;uid=KiangServer;pwd=123imgod";
                mConnection = new System.Data.SqlClient.SqlConnection(name);
                mConnection.Open();
                mDataAdapter = new System.Data.SqlClient.SqlDataAdapter();

                //mLoginLoader.StartThread(this);
                //mRoleLoadder.StartThread(this);
                //StartThreadPool(2);
            }
            catch (System.Exception exp)
            {
                System.Diagnostics.Debug.Write(exp.ToString());
                //System.Windows.Forms.MessageBox.Show("DB Connect Exception(" + name + "):" + exp.ToString());
                return false;
            }

            return true;
        }
        public void CloseConnect()
        {
            try
            {
                if (mConnection != null)
                    mConnection.Close();
            }
            catch (System.Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
            }
        }
        public bool IsValidConnect()
        {
            if (mConnection.State != System.Data.ConnectionState.Open)
                return false;
            return true;
        }

        public void ReOpen()
        {
            mConnection.Open();
        }

        //后面都是产生code后，如果update,insert都会保存在一个队列里面，每个玩家一个sqlcode保存队列
        //如果sqlcode队列在另外线程执行空后，才能从删除列表彻底把对象释放，以后再需要的时候从数据库读取
        //如果执行队列没有为空，说明还有存储数据没有完成，玩家不能彻底卸载，万一他在这过程中上来了，那么
        //直接从内存里面把他的数据取出来用就好了

        public static string SqlSafeString(string str)
        {
            string result = str.Replace("\'", "\'\'");
            //result = result.Replace("\"", "\"\"");
            //result = result.Replace("[", "[[]");//这个可能不需要，这是用like语法的时候的通配符的问题，我们没有用到like语句
            //result = result.Replace("%", "[%]");//这个可能不需要
            //result = result.Replace("_", "[_]");//这个可能不需要
            return result;
        }
        #endregion

        public static string Guid2SqlString(Guid id)
        {
            return "convert(uniqueidentifier,\'" + id.ToString() + "\')";
        }
        
        //存储过程
        public int CallSqlSP(string spName, params System.Object[] args)
        {
            System.Data.DataSet result = new System.Data.DataSet();            
            mDataAdapter.SelectCommand = new System.Data.SqlClient.SqlCommand("select * from sys.parameters where object_id=object_id('" + spName + "')", mConnection);
            System.Data.SqlClient.SqlCommandBuilder myCB = new System.Data.SqlClient.SqlCommandBuilder(mDataAdapter);
            mDataAdapter.Fill(result, "SP");
            List<string> list = new List<string>();
            foreach (System.Data.DataRow r in result.Tables[0].Rows)
            {
                list.Add(r["name"].ToString());
            }
            if (args.Count() != list.Count())
            {  //参数不符
                System.Diagnostics.Debug.WriteLine("运行存储过程失败:参数不符");
                return -1;           
            }                        
            int reNum = -1;
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(spName, mConnection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            for (int i = 0; i < list.Count(); i++)
            {
                cmd.Parameters.AddWithValue(list[i], args[i].ToString());
            }

            try
            {                
                reNum = cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine("CallSP:{0}=>{1}", spName, ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
            	return -1;
            }                        
            return reNum;
        }

        #region Sql Select

        public static bool FillObject(object obj, System.Data.DataRow row)
        {
            Type objType = obj.GetType();
            object[] propsTab = objType.GetCustomAttributes(typeof(CSUtility.DBBindTable), false);
            if (propsTab == null)
                return false;
            System.Reflection.PropertyInfo[] props = objType.GetProperties();
            foreach (System.Reflection.PropertyInfo p in props)
            {
                object[] attrs = p.GetCustomAttributes(typeof(CSUtility.DBBindField), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                CSUtility.DBBindField dbBind = attrs[0] as CSUtility.DBBindField;
                if (dbBind == null)
                    continue;

                try
                {
                    if (row[dbBind.Field] is System.DBNull)
                        continue;

                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(obj, row[dbBind.Field].ToString(), null);
                    }
                    else if (p.PropertyType == typeof(Guid))
                    {
                        /*string guidStr = row[dbBind.Field].ToString();
                        if (guidStr == "0")
                            p.SetValue(obj, Guid.NewGuid(), null);
                        else
                            p.SetValue(obj, Guid.Parse(guidStr), null);*/

                        //CONVERT(uniqueidentifier,'313A857C-7251-45A0-A6A7-8482AC5D9B1D')

                        p.SetValue(obj, (Guid)row[dbBind.Field], null);
                    }
                    else if (p.PropertyType == typeof(byte[]))
                    {
                        p.SetValue(obj, (byte[])row[dbBind.Field], null);
                    }
                    else if (p.PropertyType == typeof(SByte))
                    {
                        p.SetValue(obj, (SByte)System.Convert.ToInt32(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(Int16))
                    {
                        p.SetValue(obj, (Int16)System.Convert.ToInt32(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(Int32))
                    {
                        p.SetValue(obj, System.Convert.ToInt32(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(System.DateTime))
                    {
                        p.SetValue(obj, row[dbBind.Field], null);
                    }
                    else if (p.PropertyType == typeof(Int64))
                    {
                        p.SetValue(obj, System.Convert.ToInt64(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(Byte))
                    {
                        p.SetValue(obj, (Byte)System.Convert.ToUInt32(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(UInt16))
                    {
                        p.SetValue(obj, (UInt16)System.Convert.ToUInt32(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(UInt32))
                    {
                        p.SetValue(obj, System.Convert.ToUInt32(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(UInt64))
                    {
                        p.SetValue(obj, System.Convert.ToUInt64(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(Single))
                    {
                        p.SetValue(obj, System.Convert.ToSingle(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(Double))
                    {
                        p.SetValue(obj, System.Convert.ToDouble(row[dbBind.Field]), null);
                    }
                    else if (p.PropertyType == typeof(System.DateTime))
                    {
                        p.SetValue(obj, row[dbBind.Field], null);
                    }
                    else if (p.PropertyType.IsEnum)
                    {
                        p.SetValue(obj, System.Convert.ToInt32(row[dbBind.Field]), null);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                }
            }
            return true;
        }

        public System.Data.DataTable _ExecuteSelect(DBOperator dbOp,string tabName)
        {
            System.Data.DataSet result = new System.Data.DataSet();
            try
            {
                dbOp.SqlCode = "use " + mDBName + EndLine + dbOp.SqlCode;
                mDataAdapter.SelectCommand = dbOp.ToSqlCommand(mConnection);
                //System.Data.SqlClient.SqlCommandBuilder myCB = new System.Data.SqlClient.SqlCommandBuilder(mDataAdapter);
                //int iRet = mDataAdapter.SelectCommand.ExecuteReader()

                mDataAdapter.Fill(result, tabName);
                mDataAdapter.SelectCommand = null;
            }
            catch (System.Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
                Log.FileLog.WriteLine(e.StackTrace.ToString());
                Log.FileLog.WriteLine(dbOp.SqlCode);

                return null;
            }
            return result.Tables[0];
        }

        public static DBOperator SelectData(string condition, object obj, string prefix)
        {
            DBOperator dbOp = new DBOperator();
            dbOp.ExeType = SqlExeType.Select;
            string result = "";
            Type objType = obj.GetType();
            object[] propsTab = objType.GetCustomAttributes(typeof(CSUtility.DBBindTable), false);
            if (propsTab == null)
                return null;
            CSUtility.DBBindTable bindTab = propsTab[0] as CSUtility.DBBindTable;
            System.Reflection.PropertyInfo[] props = objType.GetProperties();
            bool first = true;
            foreach (System.Reflection.PropertyInfo p in props)
            {
                object[] attrs = p.GetCustomAttributes(typeof(CSUtility.DBBindField), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                CSUtility.DBBindField dbBind = attrs[0] as CSUtility.DBBindField;
                if (dbBind == null)
                    continue;
                if (first)
                {
                    result += dbBind.Field;
                    first = false;
                }
                else
                {
                    result += "," + dbBind.Field;
                }
            }
            if (string.IsNullOrEmpty(condition))
                //return "select " + prefix + result + " from " + bindTab.Table + " where Deleted=0" + EndLine;
                dbOp.SqlCode = "select " + prefix + result + " from " + bindTab.Table + " where Deleted=0" + EndLine;
            else
                //return "select " + prefix + result + " from " + bindTab.Table + " where Deleted=0 and " + condition + EndLine;
                dbOp.SqlCode = "select " + prefix + result + " from " + bindTab.Table + " where Deleted=0 and " + condition + EndLine;

            return dbOp;
        }

        public UInt64 GetDataCount(string tabName)
        {
            try
            {
                mDataAdapter.SelectCommand = new System.Data.SqlClient.SqlCommand("select count(*) totalCount from " + tabName, mConnection);
                //mConnection.Open();
                UInt64 retNum = Convert.ToUInt64(mDataAdapter.SelectCommand.ExecuteScalar());
                //mConnection.Close();
                return retNum;
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                Log.FileLog.WriteLine(ex.StackTrace.ToString());
                return 0;
            }
        }

        #endregion

        #region Sql Update

        public bool _ExecuteUpdate(DBOperator dbOp)
        {
            try
            {
                dbOp.SqlCode = "use " + mDBName + EndLine + dbOp.SqlCode;
                mDataAdapter.UpdateCommand = dbOp.ToSqlCommand(mConnection);
                
                mDataAdapter.UpdateCommand.ExecuteNonQuery();
                mDataAdapter.UpdateCommand = null;
            }
            catch (System.TimeoutException)
            {
                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
                Log.FileLog.WriteLine(dbOp.SqlCode);

                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                    return false;
                }
                //System.Windows.Forms.MessageBox.Show(e.ToString(), "SqlError");
                //System.Windows.Forms.MessageBox.Show(dbOp.SqlCode, "SqlError");
            }
            return true;
        }

        private static bool IsEqualByteArray(byte[] lh, byte[] rh)
        {
            if (lh.Length != rh.Length)
                return false;
            for (int i = 0; i < lh.Length; i++)
            {
                if (lh[i] != rh[i])
                    return false;
            }
            return true;
        }
        public static DBOperator UpdateData(string condition, object obj, object templateobj)
        {
            DBOperator dbOp = new DBOperator();
            dbOp.ExeType = SqlExeType.Update;

            if (templateobj != null && obj.GetType() != templateobj.GetType())
                return null;
            Type objType = obj.GetType();
            object[] propsTab = objType.GetCustomAttributes(typeof(CSUtility.DBBindTable), false);
            if (propsTab == null)
                return null;
            CSUtility.DBBindTable bindTab = propsTab[0] as CSUtility.DBBindTable;
            System.Reflection.PropertyInfo[] props = objType.GetProperties();
            bool first = true;
            string result = "";
            int fieldCount = 0;
            foreach (System.Reflection.PropertyInfo p in props)
            {
                object[] attrs = p.GetCustomAttributes(typeof(CSUtility.DBBindField), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                CSUtility.DBBindField dbBind = attrs[0] as CSUtility.DBBindField;
                if (dbBind == null)
                    continue;

                try
                {
                    object tpl = null;
                    object fv = p.GetValue(obj, null);
                    if (fv == null)
                    {
                        Log.FileLog.WriteLine(string.Format("DB.UpdateData GetValue[{0}.{1}] = null", objType.FullName, p.Name));
                        continue;
                    }
                    if (templateobj != null)
                    {
                        tpl = p.GetValue(templateobj, null);
                        if (fv.Equals(tpl))
                            continue;
                    }

                    fieldCount++;
                    string valueSql;
                    bool needStringFlag = true;
                    if (p.PropertyType.IsEnum)
                    {
                        valueSql = System.Convert.ToInt32(fv).ToString();
                    }
                    else if (p.PropertyType == typeof(System.DateTime))
                    {
                        valueSql = System.String.Format("convert(DateTime,\'{0}\')", fv.ToString());
                        needStringFlag = false;
                    }
                    else if (p.PropertyType == typeof(System.Guid))
                    {
                        valueSql = System.String.Format("convert(uniqueidentifier,\'{0}\')", fv.ToString());
                        needStringFlag = false;
                    }
                    else if (p.PropertyType == typeof(byte[]))
                    {
                        if (tpl != null && IsEqualByteArray(fv as byte[], tpl as byte[]))
                        {
                            fieldCount--;
                            continue;
                        }

                        valueSql = System.String.Format("@{0}", p.Name);
                        needStringFlag = false;

                        //string sql = "update T_Employee set ImageLogo=@ImageLogo where EmpId=@EmpId"; 
                        //byte[] imgSourse = new byte[100];
                        var param = new System.Data.SqlClient.SqlParameter(valueSql, fv);
                        //mDataAdapter.UpdateCommand.Parameters.Add(param);
                        dbOp.SqlParameters.Add(param);
                    }
                    else
                    {
                        valueSql = fv.ToString();
                        //防止SQL注入处理
                        valueSql = SqlSafeString(valueSql);
                    }
                    if (first)
                    {
                        if (needStringFlag)
                            result += "    set " + dbBind.Field + " = \'" + valueSql + "\'";
                        else
                            result += "    set " + dbBind.Field + " = " + valueSql;
                        first = false;
                    }
                    else
                    {
                        if (needStringFlag)
                            result += "," + dbBind.Field + " = \'" + valueSql + "\'";
                        else
                            result += "," + dbBind.Field + " = " + valueSql;
                    }
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
            if (result == "" || fieldCount==0)
                return null;

            dbOp.SqlCode = "update " + bindTab.Table + "\r\n" + result + "\r\n where " + condition;
            //return "update " + bindTab.Table + "\r\n" + result + "\r\n where " + condition;
            return dbOp;
        }

        #endregion
        
        #region Sql Insert
        public bool _ExecuteInsert(DBOperator dbOp)
        {
            try
            {
                dbOp.SqlCode = "use " + mDBName + EndLine + dbOp.SqlCode;
                mDataAdapter.InsertCommand = dbOp.ToSqlCommand(mConnection);
                mDataAdapter.InsertCommand.ExecuteNonQuery();
                mDataAdapter.InsertCommand = null;
            }
            catch (System.TimeoutException)
            {
                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
                Log.FileLog.WriteLine(dbOp.SqlCode);
                //System.Windows.Forms.MessageBox.Show(e.ToString(), "SqlError");
                //System.Windows.Forms.MessageBox.Show(dbOp.SqlCode, "SqlError");

                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                }

                return false;
            }
            return true;
        }
        public static DBOperator InsertData(string keyCondition, object obj, bool existUpdate)
        {
            DBOperator dbOp = new DBOperator();
            dbOp.ExeType = SqlExeType.Insert;

            Type objType = obj.GetType();
            object[] propsTab = objType.GetCustomAttributes(typeof(CSUtility.DBBindTable), false);
            if (propsTab == null)
                return null;
            CSUtility.DBBindTable bindTab = propsTab[0] as CSUtility.DBBindTable;
            System.Reflection.PropertyInfo[] props = objType.GetProperties();
            string fieldStr = "";
            string valueStr = "";
            string setStr = "";
            bool first = true;
            foreach (System.Reflection.PropertyInfo p in props)
            {
                object[] attrs = p.GetCustomAttributes(typeof(CSUtility.DBBindField), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                CSUtility.DBBindField dbBind = attrs[0] as CSUtility.DBBindField;
                if (dbBind == null)
                    continue;
                
                object v = p.GetValue(obj, null);
                string valueSql;
                bool needStringFlag = true;
                if (p.PropertyType.IsEnum)
                {
                    valueSql = System.Convert.ToInt32(v).ToString();
                }
                else if (p.PropertyType == typeof(System.DateTime))
                {
                    valueSql = System.String.Format("convert(DateTime,\'{0}\')", v.ToString());
                    needStringFlag = false;
                }
                else if (p.PropertyType == typeof(System.Guid))
                {
                    valueSql = System.String.Format("convert(uniqueidentifier,\'{0}\')", v.ToString());
                    needStringFlag = false;
                }
                else if (p.PropertyType == typeof(byte[]))
                {
                    valueSql = System.String.Format("@{0}", p.Name);
                    needStringFlag = false;

                    var param = new System.Data.SqlClient.SqlParameter(valueSql, v);
                    dbOp.SqlParameters.Add(param);
                }
                else
                {
                    if (v != null)
                        valueSql = v.ToString();
                    else
                        valueSql = "";
                    //valueSql这个地方要处理数据库攻击，SQL注入
                    valueSql = SqlSafeString(valueSql);
                }
                if (first)
                {
                    fieldStr += dbBind.Field;
                    if (needStringFlag)
                        valueStr += "\'" + valueSql + "\'";
                    else
                        valueStr += valueSql;
                    if (needStringFlag)
                        setStr += " set " + dbBind.Field + "= \'" + valueSql + "\'";
                    else
                        setStr += " set " + dbBind.Field + "= " + valueSql;
                    first = false;
                }
                else
                {
                    fieldStr += "," + dbBind.Field;
                    if (needStringFlag)
                        valueStr += ",\'" + valueSql + "\'";
                    else
                        valueStr += "," + valueSql;
                    if (needStringFlag)
                        setStr += "," + dbBind.Field + "=\'" + valueSql + "\'";
                    else
                        setStr += "," + dbBind.Field + "=" + valueSql;
                }
            }
            if (existUpdate)
            {
                string finalStr = "if exists(select* from " + bindTab.Table + " where " + keyCondition + ")\r\n";
                finalStr += "begin\r\n";
                finalStr += "   update " + bindTab.Table + setStr + " where " + keyCondition + "\r\n";
                finalStr += "end\r\n";
                finalStr += "else\r\n";
                finalStr += "begin\r\n";
                finalStr += "   insert into " + bindTab.Table + " (" + fieldStr + ") values (" + valueStr + ")\r\n";
                finalStr += "end\r\n";

                dbOp.SqlCode = finalStr;
                return dbOp;
            }
            else
            {
                dbOp.SqlCode = "insert into " + bindTab.Table + " (" + fieldStr + ") values (" + valueStr + ")\r\n";
                return dbOp;
                //return "insert into " + bindTab.Table + " (" + fieldStr + ") values (" + valueStr + ")\r\n";
            }
        }
        #endregion
        
        #region Sql Del
        public static DBOperator DelData(string condition, object obj)
        {
            DBOperator dbOp = new DBOperator();
            dbOp.ExeType = SqlExeType.Delete;
            Type objType = obj.GetType();
            object[] propsTab = objType.GetCustomAttributes(typeof(CSUtility.DBBindTable), false);
            if (propsTab == null)
                return null;
            CSUtility.DBBindTable bindTab = propsTab[0] as CSUtility.DBBindTable;
            dbOp.SqlCode = "update " + bindTab.Table + " set Deleted=1 where " + condition;
            //return "update " + bindTab.Table + " set Deleted=1 where " + condition;
            return dbOp;
        }
        public bool _ExecuteDelete(DBOperator dbOp)
        {
            try
            {
                dbOp.SqlCode = "use " + mDBName + EndLine + dbOp.SqlCode;
                mDataAdapter.UpdateCommand = dbOp.ToSqlCommand(mConnection);
                mDataAdapter.UpdateCommand.ExecuteNonQuery();
                mDataAdapter.UpdateCommand = null;
            }
            catch(System.TimeoutException)
            {
                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
                Log.FileLog.WriteLine(dbOp.SqlCode);

                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                    return false;
                }
            }
            return true;
        }

        public static DBOperator DestroyData(string condition, object obj)
        {
            DBOperator dbOp = new DBOperator();
            dbOp.ExeType = SqlExeType.Destroy;
            Type objType = obj.GetType();
            object[] propsTab = objType.GetCustomAttributes(typeof(CSUtility.DBBindTable), false);
            if (propsTab == null)
                return null;
            CSUtility.DBBindTable bindTab = propsTab[0] as CSUtility.DBBindTable;
            dbOp.SqlCode = "delete from " + bindTab.Table + " where " + condition;
            return dbOp;
            //return "delete from " + bindTab.Table + " where " + condition;
        }
        public bool _ExecuteDestroy(DBOperator dbOp)
        {
            try
            {
                mDataAdapter.DeleteCommand = dbOp.ToSqlCommand(mConnection);
                mDataAdapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (System.TimeoutException)
            {
                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
                Log.FileLog.WriteLine(dbOp.SqlCode);
                if (mConnection.State == System.Data.ConnectionState.Closed)
                {
                    this.ReOpen();
                    return false;
                }
            }
            return true;
        }
        #endregion
        
        
    }
}

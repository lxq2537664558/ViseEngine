using System;

namespace RPC
{
    public class IAutoSLField : IComparable
	{
		public IAutoSLField()
		{
			Level = 0;
			IgnoreWhenSingle = false;
		}
		public System.Reflection.PropertyInfo	Property;
		public int									Level;
		public bool								IgnoreWhenSingle;

        #region 实现比较接口的CompareTo方法
        public int CompareTo(object obj)
        {
            try
            {
                IAutoSLField sObj = (IAutoSLField)obj;
                return Property.Name.CompareTo(sObj.Property.Name);
            }
            catch (Exception ex)
            {
                throw new Exception("比较异常", ex.InnerException);
            }
        }
        #endregion
	}

	public class IAutoSLClassDesc
	{
        public System.Collections.Generic.List<IAutoSLField> Fields = new System.Collections.Generic.List<IAutoSLField>();
        public System.Collections.Generic.List<IAutoSLField> ExtraFields = new System.Collections.Generic.List<IAutoSLField>();

        public void CreateDesc(System.Type klassType)
	    {
		    System.Type objType = klassType;
		    System.Reflection.PropertyInfo[] fields = objType.GetProperties();

            Fields.Clear();
		    foreach(System.Reflection.PropertyInfo i in fields)
		    {
			    System.Object[] attrs = i.GetCustomAttributes(typeof(FieldDontAutoSaveLoadAttribute),true);
                if (attrs.Length != 0)
                {
                    IAutoSLField f = new IAutoSLField();
                    f.Property = i;
                    f.IgnoreWhenSingle = true;

                    ExtraFields.Add(f);
                    continue;
                }
                else
                {
                    IAutoSLField f = new IAutoSLField();
                    f.Property = i;
                    attrs = i.GetCustomAttributes(typeof(FieldAutoSaveLoadAttribute), true);
                    if (attrs.Length != 0)
                    {
                        FieldAutoSaveLoadAttribute atb = (FieldAutoSaveLoadAttribute)attrs[0];
                        if (atb != null)
                        {
                            f.Level = atb.Level;
                        }
                    }

                    attrs = i.GetCustomAttributes(typeof(FieldDontAutoSingleSaveLoadAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        f.IgnoreWhenSingle = true;
                    }

                    Fields.Add(f);
                }
		    }
            Fields.Sort();
	    }
	}

	public class IAutoSLClassDescManager
	{
		System.Collections.Generic.Dictionary<System.Type,IAutoSLClassDesc> mKlasses= new System.Collections.Generic.Dictionary<System.Type,IAutoSLClassDesc>();
		static IAutoSLClassDescManager smInstance = new IAutoSLClassDescManager();
	
		public static IAutoSLClassDescManager Instance
        {
            get
            {
			    return smInstance;
            }
		}

		public IAutoSLClassDesc GetClassDesc(System.Type klassType)
	    {
            lock (this)
            {
                IAutoSLClassDesc result;
                if (mKlasses.TryGetValue(klassType, out result))
                    return result;

                result = new IAutoSLClassDesc();
                result.CreateDesc(klassType);

                mKlasses.Add(klassType, result);
                return result;
            }
	    }
	}

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
	public class IAutoSaveAndLoad
	{
        public virtual void PackageWrite(PackageWriter pkg, System.Type sType=null)
	    {
		    if( pkg.IsSinglePkg )
		    {
			    PackageWriteSingle(pkg, sType);
		    }
		    else
		    {
                PackageWriteFull(pkg, sType);
		    }
	    }
        public virtual void PackageRead(PackageProxy pkg, System.Type sType = null)
	    {
		    if( pkg.IsSinglePkg )
		    {
			    PackageReadSingle(pkg,sType);
		    }
		    else
		    {
                PackageReadFull(pkg, sType);
		    }
	    }

        #region Read&Write Utility
        protected bool WritePkg(PackageWriter pkg,System.Reflection.PropertyInfo i)
        {
            var propType = i.PropertyType;
            if (propType == typeof(System.SByte))
            {
                pkg.Write( System.Convert.ToSByte(i.GetValue(this,null)));
            }
            else if(propType==typeof(System.Int16))
		    {
		        pkg.Write( System.Convert.ToInt16(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.Int32))
		    {
		        pkg.Write( System.Convert.ToInt32(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.Int64))
		    {
		        pkg.Write( System.Convert.ToInt64(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.Byte))
		    {
		        pkg.Write( System.Convert.ToByte(i.GetValue(this,null)));
	        }
            else if (propType == typeof(System.Boolean))
            {
                var value = (bool)i.GetValue(this, null);
                pkg.Write(value);
            }
		    else if(propType==typeof(System.UInt16))
		    {
		        pkg.Write( System.Convert.ToUInt16(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.UInt32))
		    {
		        pkg.Write( System.Convert.ToUInt32(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.UInt64))
		    {
		        pkg.Write( System.Convert.ToUInt64(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.Single))
		    {
		        pkg.Write( System.Convert.ToSingle(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.Double))
		    {
		        pkg.Write( System.Convert.ToDouble(i.GetValue(this,null)));
	        }
		    else if(propType==typeof(System.Guid))
		    {
		        System.Guid id = (System.Guid)(i.GetValue(this,null));
		        pkg.Write( id );
	        }
            else if (propType == typeof(byte[]))
            {
                var datas = (byte[])(i.GetValue(this, null));
                pkg.Write(datas);
            }
            else if (propType == typeof(SlimDX.Vector2))
            {
                SlimDX.Vector2 id = (SlimDX.Vector2)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(SlimDX.Vector3))
            {
                SlimDX.Vector3 id = (SlimDX.Vector3)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(SlimDX.Vector4))
            {
                SlimDX.Vector4 id = (SlimDX.Vector4)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(SlimDX.Quaternion))
            {
                SlimDX.Quaternion id = (SlimDX.Quaternion)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(SlimDX.Matrix))
            {
                SlimDX.Matrix id = (SlimDX.Matrix)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(System.DateTime))
            {
                System.DateTime id = (System.DateTime)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(System.String))
            {
                System.String str = (System.String)(i.GetValue(this, null));
                if (str == null)
                    str = "";
                var attrs = i.GetCustomAttributes(typeof(RPC.FixedSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.FixedSizeAttribute;
                    if (str.Length >= esatt.FixedSize)
                    {
                        str = str.Substring(0, esatt.FixedSize);
                        pkg.Write(str);
                        Log.FileLog.WriteLine(string.Format("RPC Write String [{0}]的时候出现FixesSize小于字符换长度的情况，请检查", i.ToString()));
                    }
                    else
                    {
                        pkg.Write(str);
                        var remain = esatt.FixedSize - str.Length;
                        var zeroBuffer = new byte[remain*sizeof(System.Char)];
                        pkg.Write(zeroBuffer,zeroBuffer.Length);
                    }
                }
                else
                {   
                    pkg.Write(str);
                }
            }
            else if (propType.IsEnum)
            {
                var attrs = i.GetCustomAttributes(typeof(RPC.EnumSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.EnumSizeAttribute;

                    if (esatt.SizeType == typeof(System.SByte))
                    {
                        pkg.Write(System.Convert.ToSByte(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Int16))
                    {
                        pkg.Write(System.Convert.ToInt16(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Int32))
                    {
                        pkg.Write(System.Convert.ToInt32(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Int64))
                    {
                        pkg.Write(System.Convert.ToInt64(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Byte))
                    {
                        pkg.Write(System.Convert.ToByte(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.UInt16))
                    {
                        pkg.Write(System.Convert.ToUInt16(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.UInt32))
                    {
                        pkg.Write(System.Convert.ToUInt32(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.UInt64))
                    {
                        pkg.Write(System.Convert.ToUInt64(i.GetValue(this, null)));
                    }
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        protected bool ReadPkg(PackageProxy pkg, System.Reflection.PropertyInfo i)
        {
            var propType = i.PropertyType;
            if(propType==typeof(System.SByte))
            {
		        System.SByte value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.Int16))
		    {
		        System.Int16 value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.Int32))
		    {
		        System.Int32 value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.Int64))
		    {
		        System.Int64 value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.Byte))
		    {
		        System.Byte value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
            else if (propType == typeof(System.Boolean))
            {
                bool value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
		    else if(propType==typeof(System.UInt16))
		    {
		        System.UInt16 value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.UInt32))
		    {
		        System.UInt32 value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.UInt64))
		    {
		        System.UInt64 value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.Single))
		    {
		        System.Single value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.Double))
		    {
		        System.Double value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.Guid))
		    {
		        System.Guid value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
            else if (propType == typeof(byte[]))
            {
                byte[] value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
		    else if(propType==typeof(SlimDX.Vector3))
		    {
		        SlimDX.Vector3 value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(SlimDX.Matrix))
		    {
		        SlimDX.Matrix value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.DateTime))
		    {
		        System.DateTime value;
		        pkg.Read(out value);
		        i.SetValue(this,value,null);
	        }
		    else if(propType==typeof(System.String))
		    {
                System.String value = "";
                pkg.Read(out value);
                var attrs = i.GetCustomAttributes(typeof(RPC.FixedSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.FixedSizeAttribute;
                    if (value.Length < esatt.FixedSize)
                    {
                        var remain = esatt.FixedSize - value.Length;
                        byte[] zeroBuffer = new byte[remain * sizeof(System.Char)];
                        pkg.Read(zeroBuffer, zeroBuffer.Length);
                    }
                }

                i.SetValue(this, value, null);
	        }
            else if (propType.IsEnum)
            {
                var attrs = i.GetCustomAttributes(typeof(RPC.EnumSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.EnumSizeAttribute;
                    if (esatt.SizeType == typeof(System.SByte))
                    {
                        System.SByte value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Int16))
                    {
                        System.Int16 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Int32))
                    {
                        System.Int32 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Int64))
                    {
                        System.Int64 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Byte))
                    {
                        System.Byte value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.UInt16))
                    {
                        System.UInt16 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.UInt32))
                    {
                        System.UInt32 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.UInt64))
                    {
                        System.UInt64 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        protected bool WriteData(DataWriter pkg, System.Reflection.PropertyInfo i)
        {
            var propType = i.PropertyType;
            if (propType == typeof(System.SByte))
            {
                pkg.Write(System.Convert.ToSByte(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Int16))
            {
                pkg.Write(System.Convert.ToInt16(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Int32))
            {
                pkg.Write(System.Convert.ToInt32(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Int64))
            {
                pkg.Write(System.Convert.ToInt64(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Byte))
            {
                pkg.Write(System.Convert.ToByte(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Boolean))
            {
                var value = (bool)i.GetValue(this, null);
                pkg.Write(value);
            }
            else if (propType == typeof(System.UInt16))
            {
                pkg.Write(System.Convert.ToUInt16(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.UInt32))
            {
                pkg.Write(System.Convert.ToUInt32(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.UInt64))
            {
                pkg.Write(System.Convert.ToUInt64(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Single))
            {
                pkg.Write(System.Convert.ToSingle(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Double))
            {
                pkg.Write(System.Convert.ToDouble(i.GetValue(this, null)));
            }
            else if (propType == typeof(System.Guid))
            {
                System.Guid id = (System.Guid)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(byte[]))
            {
                var datas = (byte[])(i.GetValue(this, null));
                pkg.Write(datas);
            }
            else if (propType == typeof(SlimDX.Vector3))
            {
                SlimDX.Vector3 id = (SlimDX.Vector3)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(SlimDX.Matrix))
            {
                SlimDX.Matrix id = (SlimDX.Matrix)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(System.DateTime))
            {
                System.DateTime id = (System.DateTime)(i.GetValue(this, null));
                pkg.Write(id);
            }
            else if (propType == typeof(System.String))
            {
                System.String str = (System.String)(i.GetValue(this, null));
                if (str == null)
                    str = "";
                var attrs = i.GetCustomAttributes(typeof(RPC.FixedSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.FixedSizeAttribute;
                    if (str.Length >= esatt.FixedSize)
                    {
                        str = str.Substring(0, esatt.FixedSize);
                        pkg.Write(str);
                        Log.FileLog.WriteLine(string.Format("RPC Write String [{0}]的时候出现FixesSize小于字符换长度的情况，请检查", i.ToString()));
                    }
                    else
                    {
                        pkg.Write(str);
                        var remain = esatt.FixedSize - str.Length;
                        var zeroBuffer = new byte[remain * sizeof(System.Char)];
                        pkg.Write(zeroBuffer, zeroBuffer.Length);
                    }
                }
                else
                {
                    pkg.Write(str);
                }
            }
            else if (propType.IsEnum)
            {
                var attrs = i.GetCustomAttributes(typeof(RPC.EnumSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.EnumSizeAttribute;

                    if (esatt.SizeType == typeof(System.SByte))
                    {
                        pkg.Write(System.Convert.ToSByte(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Int16))
                    {
                        pkg.Write(System.Convert.ToInt16(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Int32))
                    {
                        pkg.Write(System.Convert.ToInt32(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Int64))
                    {
                        pkg.Write(System.Convert.ToInt64(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.Byte))
                    {
                        pkg.Write(System.Convert.ToByte(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.UInt16))
                    {
                        pkg.Write(System.Convert.ToUInt16(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.UInt32))
                    {
                        pkg.Write(System.Convert.ToUInt32(i.GetValue(this, null)));
                    }
                    else if (esatt.SizeType == typeof(System.UInt64))
                    {
                        pkg.Write(System.Convert.ToUInt64(i.GetValue(this, null)));
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        protected bool ReadData(DataReader pkg, System.Reflection.PropertyInfo i)
        {
            var propType = i.PropertyType;
            if (propType == typeof(System.SByte))
            {
                System.SByte value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Int16))
            {
                System.Int16 value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Int32))
            {
                System.Int32 value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Int64))
            {
                System.Int64 value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Byte))
            {
                System.Byte value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Boolean))
            {
                bool value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.UInt16))
            {
                System.UInt16 value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.UInt32))
            {
                System.UInt32 value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.UInt64))
            {
                System.UInt64 value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Single))
            {
                System.Single value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Double))
            {
                System.Double value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.Guid))
            {
                System.Guid value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(byte[]))
            {
                byte[] value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(SlimDX.Vector3))
            {
                SlimDX.Vector3 value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(SlimDX.Matrix))
            {
                SlimDX.Matrix value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.DateTime))
            {
                System.DateTime value;
                pkg.Read(out value);
                i.SetValue(this, value, null);
            }
            else if (propType == typeof(System.String))
            {
                System.String value = "";
                pkg.Read(out value);
                var attrs = i.GetCustomAttributes(typeof(RPC.FixedSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.FixedSizeAttribute;
                    if (value.Length < esatt.FixedSize)
                    {
                        var remain = esatt.FixedSize - value.Length;
                        byte[] zeroBuffer = new byte[remain * sizeof(System.Char)];
                        pkg.Read(zeroBuffer, zeroBuffer.Length);
                    }
                }

                i.SetValue(this, value, null);
            }
            else if (propType.IsEnum)
            {
                var attrs = i.GetCustomAttributes(typeof(RPC.EnumSizeAttribute), true);
                if (attrs.Length > 0)
                {
                    var esatt = attrs[0] as RPC.EnumSizeAttribute;
                    if (esatt.SizeType == typeof(System.SByte))
                    {
                        System.SByte value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Int16))
                    {
                        System.Int16 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Int32))
                    {
                        System.Int32 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Int64))
                    {
                        System.Int64 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.Byte))
                    {
                        System.Byte value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.UInt16))
                    {
                        System.UInt16 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.UInt32))
                    {
                        System.UInt32 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else if (esatt.SizeType == typeof(System.UInt64))
                    {
                        System.UInt64 value;
                        pkg.Read(out value);
                        i.SetValue(this, value, null);
                    }
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        #endregion

        public virtual void PackageWriteFull(PackageWriter pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    var i = f.Property;

                if (WritePkg(pkg, i))
                {
                }
                else if(i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var obj = (IAutoSaveAndLoad)(i.GetValue(this,null));
					pkg.Write( obj );
                }
		    }
	    }
        public virtual void PackageReadFull(PackageProxy pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
                var i = f.Property;

                if (ReadPkg(pkg, i))
                {
                }
                else if (i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var value = (RPC.IAutoSaveAndLoad)System.Activator.CreateInstance(i.PropertyType);
					pkg.Read(value);
					i.SetValue(this,value,null);
                }
		    }
	    }

        public virtual void PackageWriteSingle(PackageWriter pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    if(f.IgnoreWhenSingle)
				    continue;

			    var i = f.Property;

                if (WritePkg(pkg, i))
                {
                }
                else if (i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var obj = (IAutoSaveAndLoad)(i.GetValue(this, null));
                    pkg.Write(obj);
                }
		    }
	    }
        public virtual void PackageReadSingle(PackageProxy pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    if(f.IgnoreWhenSingle)
				    continue;

			    var i = f.Property;

                if (ReadPkg(pkg, i))
                {
                }
                else if (i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var value = (RPC.IAutoSaveAndLoad)System.Activator.CreateInstance(i.PropertyType);
                    pkg.Read(value);
                    i.SetValue(this, value, null);
                }
		    }
	    }
        public virtual void DataWrite(DataWriter pkg, bool bToClient, System.Type sType=null)
	    {
		    if( bToClient )
		    {
                DataWriteSingle(pkg, sType);
		    }
		    else
		    {
                DataWriteFull(pkg, sType);
		    }
	    }
        public virtual void DataRead(DataReader pkg, bool bToClient, System.Type sType=null)
	    {
		    if( bToClient )
		    {
                DataReadSingle(pkg, sType);
		    }
		    else
		    {
			    DataReadFull(pkg, sType);
		    }
	    }

        public virtual void DataWriteFull(DataWriter pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    var i = f.Property;

                if (WriteData(pkg, i))
                {
                }
                else if (i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var obj = (IAutoSaveAndLoad)(i.GetValue(this, null));
                    pkg.Write(obj,false);
                }
		    }
	    }
        public virtual void DataReadFull(DataReader pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    var i = f.Property;

                if (ReadData(pkg, i))
                {
                }
                else if (i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var value = (RPC.IAutoSaveAndLoad)System.Activator.CreateInstance(i.PropertyType);
                    pkg.Read(value,false);
                    i.SetValue(this, value, null);
                }
		    }
	    }

        public virtual void DataWriteSingle(DataWriter pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    if(f.IgnoreWhenSingle)
				    continue;

			    var i = f.Property;

                if (WriteData(pkg, i))
                {
                }
                else if (i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var obj = (IAutoSaveAndLoad)(i.GetValue(this, null));
                    pkg.Write(obj, true);
                }
		    }
	    }
        public virtual void DataReadSingle(DataReader pkg, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    if(f.IgnoreWhenSingle)
				    continue;

			    var i = f.Property;

                if (ReadData(pkg, i))
                {
                }
                else if (i.PropertyType.IsSubclassOf(typeof(IAutoSaveAndLoad)))
                {
                    var value = (RPC.IAutoSaveAndLoad)System.Activator.CreateInstance(i.PropertyType);
                    pkg.Read(value, true);
                    i.SetValue(this, value, null);
                }
		    }
	    }

        public virtual void DataWrite(DataWriter pkg, int minLevel, int maxLevel, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    if(f.Level>maxLevel || f.Level<minLevel)
				    continue;

			    var i = f.Property;

                WriteData(pkg, i);
		    }
	    }
        public virtual void DataRead(DataReader pkg, int minLevel, int maxLevel, System.Type sType)
	    {
            if (sType == null)
                sType = this.GetType();
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(sType);
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    if(f.Level>maxLevel || f.Level<minLevel)
				    continue;

			    var i = f.Property;

                ReadData(pkg, i);
		    }
	    }

	    public virtual IAutoSaveAndLoad CloneObject()
	    {
		    IAutoSaveAndLoad result = (IAutoSaveAndLoad)System.Activator.CreateInstance(this.GetType());

		    var desc = IAutoSLClassDescManager.Instance.GetClassDesc( this.GetType() );
		    foreach(IAutoSLField f in desc.Fields)
		    {
			    var i = f.Property;

			    System.Type type = i.PropertyType;
                try
                {
                    var fGet = i.GetGetMethod();
                    var fSet = i.GetSetMethod();
                    if (fGet == null || fSet==null)
                        continue;
                    i.SetValue(result, i.GetValue(this, null), null);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(string.Format("IAutoSaveAndLoad.CloneObject {0}:{1}", this.GetType().FullName ,i.Name));
                }
		    }
            foreach (IAutoSLField f in desc.ExtraFields)
		    {
			    var i = f.Property;

			    System.Type type = i.PropertyType;
                try
                {
                    var fGet = i.GetGetMethod();
                    var fSet = i.GetSetMethod();
                    if (fGet == null || fSet==null)
                        continue;
                    i.SetValue(result, i.GetValue(this, null), null);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(string.Format("IAutoSaveAndLoad.CloneObject {0}:{1}", this.GetType().FullName ,i.Name));
                }
		    }
            
		    return result;
	    }

        public virtual void CopyObject(IAutoSaveAndLoad target)
        {
            var desc = IAutoSLClassDescManager.Instance.GetClassDesc(this.GetType());
            foreach (IAutoSLField f in desc.Fields)
            {
                var i = f.Property;

                System.Type type = i.PropertyType;
                try
                {
                    var fGet = i.GetGetMethod();
                    var fSet = i.GetSetMethod();
                    if (fGet == null || fSet == null)
                        continue;
                    i.SetValue(target, i.GetValue(this, null), null);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(string.Format("IAutoSaveAndLoad.CloneObject {0}:{1}", this.GetType().FullName, i.Name));
                }
            }
            foreach (IAutoSLField f in desc.ExtraFields)
            {
                var i = f.Property;

                System.Type type = i.PropertyType;
                try
                {
                    var fGet = i.GetGetMethod();
                    var fSet = i.GetSetMethod();
                    if (fGet == null || fSet == null)
                        continue;
                    i.SetValue(target, i.GetValue(this, null), null);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(string.Format("IAutoSaveAndLoad.CloneObject {0}:{1}", this.GetType().FullName, i.Name));
                }
            }
        }


        public static byte[] CompressArray(byte[] data, byte[] tpl, UInt32 length)
        {
            if (length > data.Length || length > tpl.Length)
                return null;
            for (UInt32 i = 0; i < length; i++)
            {
                data[i] -= tpl[i];
            }

            byte[] result = null;
            unsafe
            {
                fixed(byte* inData = &data[0])
                {
                    var outLength = CSUtility.DllImportAPI.vfxCompressRLE((IntPtr)inData, length, IntPtr.Zero);
                    result = new byte[outLength];
                    fixed(byte* outData = &result[0])
                    {
                        CSUtility.DllImportAPI.vfxCompressRLE((IntPtr)inData, length, (IntPtr)outData);
                    }
                }
            }
            //var inDatahandle = System.Runtime.InteropServices.GCHandle.Alloc(data, System.Runtime.InteropServices.GCHandleType.Pinned);
            //IntPtr inData = inDatahandle.AddrOfPinnedObject();
            //var outLength = IDllImportAPI.vfxCompressRLE(inData, length, IntPtr.Zero);
            //result = new byte[outLength];

            //var outDatahandle = System.Runtime.InteropServices.GCHandle.Alloc(result, System.Runtime.InteropServices.GCHandleType.Pinned);
            //IntPtr outData = outDatahandle.AddrOfPinnedObject();
            //IDllImportAPI.vfxCompressRLE(inData, length, outData);

            //if (outDatahandle.IsAllocated)
            //    outDatahandle.Free();
            //if (inDatahandle.IsAllocated)
            //    inDatahandle.Free();

            return result;
        }

        public static byte[] UnCompressArray(byte[] data, byte[] tpl, UInt32 length)
        {
            if (length > tpl.Length)
                return null;

            byte[] result = null;

            unsafe
            {
                fixed (byte* inData = &data[0])
                {
                    var outLength = CSUtility.DllImportAPI.vfxUnCompressRLE((IntPtr)inData, (UInt32)data.Length, IntPtr.Zero);
                    if (outLength != length)
                        return null;
                    result = new byte[outLength];
                    fixed (byte* outData = &result[0])
                    {
                        CSUtility.DllImportAPI.vfxUnCompressRLE((IntPtr)inData, (UInt32)data.Length, (IntPtr)outData);
                    }
                }
            }
            //var inDatahandle = System.Runtime.InteropServices.GCHandle.Alloc(data, System.Runtime.InteropServices.GCHandleType.Pinned);
            //IntPtr inData = inDatahandle.AddrOfPinnedObject();
            //var outLength = IDllImportAPI.vfxUnCompressRLE(inData, (UInt32)data.Length, IntPtr.Zero);
            //if (outLength != length)
            //    return null;
            //result = new byte[outLength];

            //var outDatahandle = System.Runtime.InteropServices.GCHandle.Alloc(result, System.Runtime.InteropServices.GCHandleType.Pinned);
            //IntPtr outData = outDatahandle.AddrOfPinnedObject();
            //IDllImportAPI.vfxUnCompressRLE(inData, (UInt32)data.Length, outData);

            for (UInt32 i = 0; i < length; i++)
            {
                result[i] += tpl[i];
            }

            //if (outDatahandle.IsAllocated)
            //    outDatahandle.Free();
            //if (inDatahandle.IsAllocated)
            //    inDatahandle.Free();

            return result;
        }

        byte[] vfxRLE_Ecode(byte[] data,int length)
        {
	        byte[] result = new byte[length*2];
            int index = 0;
		    for(int r = 0;r<length;)
		    {
			    if(data[r] == data[+1])
			    {
                    var count = System.Math.Min(length-r,127);
                    byte c=1;
				    for(;c<count;c++)
                    {
                        if(data[r]!=data[r+c])
                        {
                            break;
                        }
                    }

				    result[index] = c;
				    ++index;
				    result[index] = data[r];
                    ++index;
				    r += c;
			    }
			    else
			    {
                    var count = System.Math.Min(length-r,127);
                    byte c=1;
				    for(;c<count;c++)
                    {
                        if(data[r]==data[r+c])
                        {
                            break;
                        }
                    }
				    result[index] = (byte)(c + 127);
				    ++index;
                    for(byte i=0;i<c;i++)
                    {
                        result[index+i] = data[r+i];
                    }
                    index += c;
                    r += c;
			    }
		    }
            return result;
        }
	}
}

using System;
using System.Collections.Generic;

namespace CSUtility.Support
{
    public class PropertyInfo
	{
		public int mHashCode;
        public UInt32 mNewHashCode;
		public System.String mName;
        public System.String mTypeString = "";
		public System.Type mType;
        public System.Reflection.PropertyInfo ProInfo = null;
	}

	public class ClassInfo
	{
	    private System.Type mClassType;
        public System.Type ClassType
        {
            get{ return mClassType; }
        }
		public System.DateTime mLastUseTime;
		public int mHashCode = 0;
        public UInt32 mNewHashCode = 0;
		public System.Collections.Generic.Dictionary<UInt32, PropertyInfo> mPropertyInfoDic;

        public ClassInfo(System.Type classType)
		{
			mClassType = classType;
            mPropertyInfoDic = new System.Collections.Generic.Dictionary<UInt32, PropertyInfo>();
		}
		public System.String GetHashString()
		{
			System.String typesString = mClassType.ToString();
			foreach (PropertyInfo property in mPropertyInfoDic.Values)
			{
                string typeName = "";
                if (property.mType != null)
                    typeName = property.mType.ToString();
				typesString += typeName + property.mName;
			}

			return typesString;
		}
	}
    // 记录对象的MetaData
    public class ClassInfoManager
    {
		static ClassInfoManager mInstance = new ClassInfoManager();
        public static ClassInfoManager Instance
        {
            get{return mInstance;}
        }
		
		private Dictionary<System.Type, ClassInfo> mLastVersionClassInfoDic = new Dictionary<System.Type, ClassInfo>();
		// 将需要保存的对象放入此列表中，以免没改变的重复保存
		private List<System.Type> mNeedSaveClassTypeInfos = new List<System.Type>();

        List<string> mErrorClasses = new List<string>();
        Dictionary<int, ClassInfo> mClassInfoDic = new Dictionary<int, ClassInfo>();
        Dictionary<UInt32, ClassInfo> mClassInfoWithNewHashCodeDic = new Dictionary<UInt32, ClassInfo>();
        Dictionary<System.Type, List<ClassInfo>> mClassTypeDic = new Dictionary<System.Type, List<ClassInfo>>();
        public Dictionary<System.Type, List<ClassInfo>> ClassTypeDic
        {
            get { return mClassTypeDic; }
        }

        public System.Type GetType(System.String typeName, System.String assemblyFullName)
		{
			if(System.String.IsNullOrEmpty(assemblyFullName))
			{
				System.Type retType = System.Type.GetType(typeName);
				if(retType != null)
					return retType;
			}
			else
			{
				try
				{
					System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assemblyFullName);
					if(assembly != null)
						return assembly.GetType(typeName);
				}
				catch (System.Exception e)
				{
					Log.FileLog.WriteLine(e.ToString());
					return null;
				}
			}
			//System.Diagnostics.Debug.Assert(false, "在GetType中 类型 " + typeName + " 找不到!");
			return null;
		}

        public void Load(bool bSecurity)
		{
            mErrorClasses.Clear();
			mClassInfoDic.Clear();
            mClassInfoWithNewHashCodeDic.Clear();
			mClassTypeDic.Clear();
			mLastVersionClassInfoDic.Clear();
			mNeedSaveClassTypeInfos.Clear();

            var metaDataDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.MetaDataDirectory;
            if (!System.IO.Directory.Exists(metaDataDir))
                return;

			foreach (System.String file in System.IO.Directory.GetFiles(metaDataDir))
			{
				System.String fileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(file);
				System.String fileExt = CSUtility.Support.IFileManager.Instance.GetFileExtension(fileName);

				CSUtility.Support.XmlHolder xmlHolder = null;
				if(fileExt == "xml" && !bSecurity)
				{
					xmlHolder = CSUtility.Support.XmlHolder.LoadXML(fileName);
                    if (xmlHolder == null)
                    {
                        var name = CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(fileName);
                        mErrorClasses.Add(name.Remove(name.LastIndexOf('.')));
                    }
				}
                else if(fileExt == "dat" && bSecurity)
                {
                    CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.LoadXND(file);
                    var att = holder.Node.FindAttrib("DataAtt");
                    if (att == null)
                        continue;

                    att.BeginRead();

                    UInt32 keyVer;
                    att.Read(out keyVer);
                    string str;
                    att.Read(out str);

                    att.EndRead();

                    System.String xmlString = CSUtility.Crypt.Crypt_DES.DesDecrypt(str, CSUtility.Crypt.Crypt_DES.GetDesKey(keyVer));
                    xmlHolder = CSUtility.Support.XmlHolder.ParseXML(xmlString);
                }

				if(xmlHolder == null || xmlHolder.RootNode == null)
					continue;
                
                int version = 0;
                var verAtt = xmlHolder.RootNode.FindAttrib("Ver");
                if (verAtt != null)
                    version = System.Convert.ToInt32(verAtt.Value);

                switch (version)
                {
                    case 0:
                        {
                            LoadClassInfo(xmlHolder, file);
                        }
                        break;
                }


				
			}
		}

        private void LoadClassInfo(CSUtility.Support.XmlHolder xmlHolder, string file)
        {
            CSUtility.Support.XmlAttrib att = xmlHolder.RootNode.FindAttrib("ClassType");
            if (att == null)
                return;

            var classType = Program.GetTypeFromSaveString(att.Value);
            if (classType == null)
                return;

            List<ClassInfo> classInfoList = new List<ClassInfo>();
            foreach (CSUtility.Support.XmlNode classNode in xmlHolder.RootNode.FindNodes("Class"))
            {
                ClassInfo classInfo = new ClassInfo(classType);

                att = classNode.FindAttrib("HashCode");
                if (att != null)
                {
                    classInfo.mHashCode = System.Convert.ToInt32(att.Value);
                    classInfo.mNewHashCode = (UInt32)(System.Convert.ToInt32(att.Value));
                }
                att = classNode.FindAttrib("LastUseTime");
                if (att != null)
                {
                    classInfo.mLastUseTime = System.DateTime.Parse(att.Value);
                }

                foreach (CSUtility.Support.XmlNode propertyNode in classNode.FindNodes("Property"))
                {
                    PropertyInfo proInfo = new PropertyInfo();

                    att = propertyNode.FindAttrib("HashCode");
                    if (att != null)
                    {
                        proInfo.mHashCode = System.Convert.ToInt32(att.Value);
                        proInfo.mNewHashCode = (UInt32)(System.Convert.ToInt32(att.Value));
                    }
                    att = propertyNode.FindAttrib("Name");
                    if (att != null)
                        proInfo.mName = att.Value;
                    att = propertyNode.FindAttrib("Type");
                    if (att != null)
                    {
                        //System.String[] splits1 = att.Value.Split('|');
                        //if (splits1.Length != 2)
                        //{
                        //    System.Diagnostics.Debug.Assert(false, "Load \r\n属性类型错误(\r\n" +
                        //        file + "\r\n HashCode=" + classInfo.mHashCode
                        //        + "\r\nPropertyName=" + proInfo.mName);
                        //}

                        proInfo.mTypeString = att.Value;

                        //proInfo.mType = GetType(splits1[1], splits1[0]);
                        proInfo.mType = Program.GetTypeFromSaveString(proInfo.mTypeString);
                    }
                    
                    if(proInfo.mType != null && !string.IsNullOrEmpty(proInfo.mName))
                        proInfo.ProInfo = classType.GetProperty(proInfo.mName, proInfo.mType);

                    classInfo.mPropertyInfoDic[proInfo.mNewHashCode] = proInfo;
                }

                ClassInfo lastVersionClassInfo = null;
                if (!mLastVersionClassInfoDic.TryGetValue(classType, out lastVersionClassInfo))
                {
                    mLastVersionClassInfoDic[classType] = classInfo;
                }
                else
                {
                    if (lastVersionClassInfo.mLastUseTime < classInfo.mLastUseTime)
                    {
                        mLastVersionClassInfoDic[classType] = classInfo;
                    }
                }

                if (mClassInfoWithNewHashCodeDic.ContainsKey(classInfo.mNewHashCode))
                {
                    classInfoList.Remove(mClassInfoWithNewHashCodeDic[classInfo.mNewHashCode]);
                }

                mClassInfoDic[classInfo.mHashCode] = classInfo;
                mClassInfoWithNewHashCodeDic[classInfo.mNewHashCode] = classInfo;
                classInfoList.Add(classInfo);
            }

            mClassTypeDic[classType] = classInfoList;
        }

        public List<System.String> Save(bool bSecurity, bool bForce)
		{
			List<System.String> retList = new List<System.String>();

            List<System.Type> saveList = new List<System.Type>();
            if (bForce)
                saveList = new List<System.Type>(mClassTypeDic.Keys);
            else
                saveList = mNeedSaveClassTypeInfos;

			//foreach (System.Collections.Generic.KeyValuePair<System.Type, System.Collections.Generic.List<ClassInfo>> value in mClassTypeDic)
			//foreach (System.Type typeValue in mNeedSaveClassTypeInfos)
            foreach (var typeValue in saveList)
			{
				System.Collections.Generic.List<ClassInfo> classInfoList;
				if(!mClassTypeDic.TryGetValue(typeValue, out classInfoList))
					continue;

				CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder(typeValue.FullName, "");
				xmlHolder.RootNode.AddAttrib("ClassType", Program.GetTypeSaveString(typeValue));//typeValue.Assembly.FullName + "|" + typeValue.ToString() );
                xmlHolder.RootNode.AddAttrib("Ver", "0");

				foreach (ClassInfo classInfo in classInfoList)
				{
					CSUtility.Support.XmlNode classNode = xmlHolder.RootNode.AddNode("Class", "", xmlHolder);

					//classNode.AddAttrib("HashCode", classInfo.mHashCode.ToString());
                    classNode.AddAttrib("HashCode", ((int)(classInfo.mNewHashCode)).ToString());
					classNode.AddAttrib("LastUseTime", classInfo.mLastUseTime.ToString());

					foreach (System.Collections.Generic.KeyValuePair<UInt32, PropertyInfo> propertyValue in classInfo.mPropertyInfoDic)
					{
						CSUtility.Support.XmlNode propertyNode = classNode.AddNode("Property", "", xmlHolder);

						propertyNode.AddAttrib("HashCode", ((int)(propertyValue.Key)).ToString());
						propertyNode.AddAttrib("Type", propertyValue.Value.mTypeString);//propertyValue.Value.mType.Assembly.FullName + "|" + propertyValue.Value.mType.FullName);
						propertyNode.AddAttrib("Name", propertyValue.Value.mName); 
					}
				}

				if(bSecurity)
				{
                    //System.String fileName = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.MetaDataDirectory + "\\" + typeValue.ToString().GetHashCode() + ".dat";
                    //System.String xmlString = "";
                    //CSUtility.Support.IXmlHolder.GetXMLString(ref xmlString, xmlHolder);
                    //System.Byte[] dataToSave = Encrypt_AES(xmlString, aesKey, aesIV);//Encrypt_RSA(xmlPublicKey, xmlString);

                    //System.IO.BinaryWriter binWriter = new System.IO.BinaryWriter(System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate));
                    //binWriter.Write(dataToSave.Length);
                    //binWriter.Write(dataToSave);
                    //binWriter.Close();
				}
				else
				{
                    var fileName = CSUtility.Support.IFileConfig.MetaDataDirectory + "\\" + GetValidTypeFileName(typeValue.ToString()) + ".xml";
					CSUtility.Support.XmlHolder.SaveXML(fileName, xmlHolder, true);
					retList.Add(CSUtility.Support.IFileManager.Instance.Root + fileName);
				}
			}

			mNeedSaveClassTypeInfos.Clear();

			return retList;
		}

        private string GetValidTypeFileName(string typeName)
        {
            return typeName.Replace('+', '$');
        }

        public bool HasEqualClass(int hashCode)
		{
			ClassInfo cInfo;
			if(mClassInfoDic.TryGetValue(hashCode, out cInfo))
				return true;

			return false;
		}
        public bool HasEqualClass_New(UInt32 hashCode)
        {
			ClassInfo cInfo;
            if (mClassInfoWithNewHashCodeDic.TryGetValue(hashCode, out cInfo))
                return true;

			return false;
        }
        public bool HasEqualClass(System.Type classType)
		{
            int hashCode = Program.GetClassHash(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute));
            ClassInfo cInfo;
            if (mClassInfoDic.TryGetValue(hashCode, out cInfo))
            {
                if (cInfo.GetHashString() == Program.GetClassHashString(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute)))
                    return true;
            }

            UInt32 newHashCode = Program.GetNewClassHash(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute));
            if (mClassInfoWithNewHashCodeDic.TryGetValue(newHashCode, out cInfo))
            {
                if (cInfo.GetHashString() == Program.GetClassHashString(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute)))
                    return true;
            }

			return false;
		}
        public bool AddClass(System.Type classType)
		{
            if (mErrorClasses.Contains(classType.FullName))
                return false;

			if(HasEqualClass(classType))
			{
				// 更新Class的时间戳
				ClassInfo info1 = GetClassInfoWithType(classType);
				info1.mLastUseTime = System.DateTime.Now;
				mLastVersionClassInfoDic[classType] = info1;

				return false;
			}

            int hashCode = Program.GetClassHash(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute));
            var newHashCode = Program.GetNewClassHash(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute));
			ClassInfo info = new ClassInfo(classType);
			info.mHashCode = hashCode;
            info.mNewHashCode = newHashCode;
			info.mLastUseTime = System.DateTime.Now;

			List<System.Reflection.PropertyInfo> propertys = new List<System.Reflection.PropertyInfo>(classType.GetProperties());
			propertys.Sort(new System.Comparison<System.Reflection.PropertyInfo>(ComparePropertyByName));
			foreach (System.Reflection.PropertyInfo property in propertys)
			{
				System.Object[] proAtts = property.GetCustomAttributes(typeof(CSUtility.Support.AutoSaveLoadAttribute), false);
				if(proAtts.Length == 0)
					continue;

				PropertyInfo pInfo = new PropertyInfo();
				pInfo.mType = property.PropertyType;
                if (pInfo.mType != null)
                {
                    pInfo.mTypeString = Program.GetTypeSaveString(property.PropertyType);
                }
				pInfo.mName = property.Name;
                var hashString = (property.PropertyType.ToString() + property.Name);
                pInfo.mHashCode = hashString.GetHashCode();
                pInfo.mNewHashCode = CSUtility.Support.UniHash.DefaultHash(hashString);
				info.mPropertyInfoDic[pInfo.mNewHashCode] = pInfo;
			}

			mClassInfoDic[hashCode] = info;
            mClassInfoWithNewHashCodeDic[newHashCode] = info;

			List<ClassInfo> infos;
			if(!mClassTypeDic.TryGetValue(classType, out infos))
			{
				infos = new List<ClassInfo>();
				mClassTypeDic[classType] = infos;
			}
			infos.Add(info);

			mLastVersionClassInfoDic[classType] = info;

			if(!mNeedSaveClassTypeInfos.Contains(classType))
				mNeedSaveClassTypeInfos.Add(classType);

			return true;
		}

        public int ComparePropertyByName(System.Reflection.PropertyInfo info1, System.Reflection.PropertyInfo info2)
		{
			uint hash1 = CSUtility.Support.UniHash.DefaultHash(info1.Name);//.GetHashCode();
			uint hash2 = CSUtility.Support.UniHash.DefaultHash(info2.Name);//.GetHashCode();

			if(hash1 > hash2)
				return 1;
			else if(hash1 < hash2)
				return -1;
			else
				return 0; 
		}
        //public System.String GetClassHashString(System.Type classType)
        //{
        //    if(classType == null)
        //        return "";

        //    List<System.Reflection.PropertyInfo> propertys = new List<System.Reflection.PropertyInfo>(classType.GetProperties());

        //    propertys.Sort(new System.Comparison<System.Reflection.PropertyInfo>(ComparePropertyByName));
        //    // 计算类的哈希值
        //    System.String typesString = classType.ToString();
        //    foreach (System.Reflection.PropertyInfo property in propertys)
        //    {
        //        System.Object[] proAtts = property.GetCustomAttributes(typeof(CSUtility.Support.AutoSaveLoadAttribute), false);
        //        if(proAtts.Length == 0)
        //            continue;

        //        string propertyTypeString = property.PropertyType.ToString();
        //        //if (property.PropertyType.IsEnum)
        //        //{
        //        //    foreach (var name in System.Enum.GetNames(property.PropertyType))
        //        //    {
        //        //        propertyTypeString += "_" + name;
        //        //    }
        //        //}

        //        typesString += propertyTypeString + property.Name;
        //    }

        //    return typesString;
        //}
        //public int GetClassHash(System.Type classType)
        //{
        //    if(classType == null)
        //        return 0;

        //    return GetClassHashString(classType).GetHashCode();
        //}

        public ClassInfo GetClassInfoWithType(System.Type classType)
		{
			if(classType == null)
				return null;

            int hashCode = Program.GetClassHash(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute));

			ClassInfo retValue = null;
			if(mClassInfoDic.TryGetValue(hashCode, out retValue))
				return retValue;

            var newHashCode = Program.GetNewClassHash(classType, typeof(CSUtility.Support.AutoSaveLoadAttribute));
            if (mClassInfoWithNewHashCodeDic.TryGetValue(newHashCode, out retValue))
                return retValue;

			//System.Collections.Generic.List<ClassInfo> infos;
			//if(mClassTypeDic.TryGetValue(classType, infos))
			//{
			//	if(infos.Count > 0)
			//		return infos[infos.Count - 1];
			//}

			return null;
		}

        public ClassInfo GetLastVersionClassInfo(System.Type classType)
		{
			if(classType == null)
				return null;

			ClassInfo lastVersionClassInfo = null;
			if(mLastVersionClassInfoDic.TryGetValue(classType, out lastVersionClassInfo))
				return lastVersionClassInfo;
			else
			{
				List<ClassInfo> infos;
				if(mClassTypeDic.TryGetValue(classType, out infos))
				{
					if(infos.Count == 0)
						return null;

					lastVersionClassInfo = infos[0];
					System.DateTime lastTime = infos[0].mLastUseTime;

					for(int i=1; i<infos.Count; i++)
					{
						if(lastTime < infos[i].mLastUseTime)
						{
							lastTime = infos[i].mLastUseTime;
							lastVersionClassInfo = infos[i];
						}
					}

					mLastVersionClassInfoDic[classType] = lastVersionClassInfo;
					return lastVersionClassInfo;
				}
			}

			return null;
		}

        public ClassInfo GetClassInfoWithHashCode(int hashCode)
		{
			ClassInfo retClass = null;
			if(mClassInfoDic.TryGetValue(hashCode, out retClass))
			{
				return retClass;
			}

			return null;
		}

        public ClassInfo GetClassInfoWithHashCode(UInt32 hashCode)
        {
			ClassInfo retClass = null;
			if(mClassInfoWithNewHashCodeDic.TryGetValue(hashCode, out retClass))
			{
				return retClass;
			}

			return null;
        }

		// 删除某个时间点之前的类参数
        public void DeleteClassInfosBeforeDate(System.DateTime time)
		{
			foreach (var infoValue in mClassTypeDic)
			{
				// 列表里至少要保留最后一个版本
				for(int i=0; i<infoValue.Value.Count - 1; i++)
				{
					ClassInfo classInfo = infoValue.Value[i];
					if((classInfo.mLastUseTime - time).TotalSeconds < 0)
					{
						infoValue.Value.RemoveAt(i);
						mClassInfoDic.Remove(classInfo.mHashCode);
                        mClassInfoWithNewHashCodeDic.Remove(classInfo.mNewHashCode);
					}
				}
			}
		}

		// 更新类的时间戳
        public void UpdateClassInfoTime(System.Type classType)
		{
			ClassInfo classInfo = GetClassInfoWithType(classType);
			if(classInfo != null)
			{
				classInfo.mLastUseTime = System.DateTime.Now;
				mLastVersionClassInfoDic[classType] = classInfo;

				if(!mNeedSaveClassTypeInfos.Contains(classType))
					mNeedSaveClassTypeInfos.Add(classType);
			}
		}

        public void GenerateClassInfoFromAssembly(System.Reflection.Assembly assembly, ref bool needSave)
        {
            if (assembly == null)
                return;

            try
            {
                foreach (var typeValue in assembly.GetTypes())
                {
                    bool bFindPropertyNeedSave = false;
                    List<System.Reflection.PropertyInfo> propertys = new List<System.Reflection.PropertyInfo>(typeValue.GetProperties());
                    foreach (var property in propertys)
                    {
                        var atts = property.GetCustomAttributes(typeof(CSUtility.Support.AutoSaveLoadAttribute), true);
                        if (atts.Length > 0)
                        {
                            bFindPropertyNeedSave = true;
                            break;
                        }
                    }

                    if (!bFindPropertyNeedSave)
                        continue;

                    // 没有相同的Hash说明类被改变过，需要重新存一个版本
                    if (HasEqualClass(typeValue))
                    {
                        var classInfo = GetLastVersionClassInfo(typeValue);
                        if (classInfo == null)
                        {
                            AddClass(typeValue);
                            needSave = true;
                        }
                        else if (classInfo.GetHashString() != Program.GetClassHashString(typeValue, typeof(CSUtility.Support.AutoSaveLoadAttribute)))
                        {
                            UpdateClassInfoTime(typeValue);
                            needSave = true;
                        }
                    }
                    else
                    {
                        AddClass(typeValue);
                        needSave = true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.FileLog.WriteLine(e.ToString());
            }
        }


    }
}

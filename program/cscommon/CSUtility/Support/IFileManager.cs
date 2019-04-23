using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSUtility
{
    public enum EFileType : int
    {
        Unknown,
        Texture,
        Mesh,
        DataTabel,
        Material,
        Xml,
        Xnd,
    }
}

namespace CSUtility.Support
{

    // 此值与VFile中OpenFlags一致，修改请同时修改两处
    enum OpenFlags
    {
        modeRead = 0x0000,
        modeWrite = 0x0001,
        modeReadWrite = 0x0002,
        shareCompat = 0x0000,
        shareExclusive = 0x0010,
        shareDenyWrite = 0x0020,
        shareDenyRead = 0x0030,
        shareDenyNone = 0x0040,
        modeNoInherit = 0x0080,
        modeCreate = 0x1000,
        modeNoTruncate = 0x2000,
        typeText = 0x4000, // typeText and typeBinary are used in
        typeBinary = (Int32)0x8000 // derived classes only
    };

    public class IFileManager
    {
        string mRoot;
        public string Root
        {
            get { return mRoot; }
        }
        string mBin;
        public string Bin
        {
            get { return mBin; }
        }

        class F2M
        {
            public IntPtr mResource = IntPtr.Zero;
            public EFileType mType = EFileType.Unknown;

            public F2M(IntPtr res)
            {
                if (res != IntPtr.Zero)
                    DllImportAPI.VFile2Memory_AddRef(res);
                mResource = res;
            }

            ~F2M()
            {
                Cleanup();
            }

            public void Cleanup()
            {
                if (mResource != IntPtr.Zero)
                {
                    DllImportAPI.VFile2Memory_Release(mResource);
                    mResource = IntPtr.Zero;
                }
            }
        }

        Dictionary<string, F2M> mReadOnlyFiles = new Dictionary<string, F2M>();

        static IFileManager mInstance = new IFileManager();
        public static IFileManager Instance
        {
            get { return mInstance; }
        }

        IFileManager()
        {
#if WIN
            mBin = AppDomain.CurrentDomain.BaseDirectory;//System.Environment.CurrentDirectory + "/";
            //mBin = Application.StartupPath;//System.Environment.CurrentDirectory + "/";
            mBin = mBin.Remove(mBin.Length - 1);
            mBin += "/";
            mRoot = mBin.Substring(0, mBin.LastIndexOf('\\') + 1);
            mRoot = mRoot.Replace("\\", "/");
            mBin = mBin.Replace("\\", "/");
#else
            mBin = "/sdcard/";
            //mBin = "/data/data/";
            //mBin = "/mnt/shell/emulated/legacy/android/data/data/game.android/files/";
            mRoot = mBin;
#endif

        }
        ~IFileManager()
        {
            Cleanup();
        }
        
        public void Initialize(string androidPackageName)
        {
#if WIN
#else
            mRoot = "/sdcard/" + androidPackageName + "/";
            mBin = mRoot + CSUtility.Support.IFileConfig.Client_Directory + "/";
#endif
        }

        public void Cleanup()
        {
            mReadOnlyFiles.Clear();
        }

        public string GetPathFromFullName(string file)
        {
            if (string.IsNullOrEmpty(file))
                return "";

            var _file = file.Replace("\\", "/");
            int pos = _file.LastIndexOf('/');
            
            if(System.IO.Path.IsPathRooted(file))	// 全路径，包含盘符
                return _file.Substring(0, pos + 1);
            else							// 相对路径
            {
                var retStr = mRoot + _file.Substring(0, pos + 1);
                return retStr.Replace("//", "/");
            }
        }

        public string GetPureFileFromFullName(string file)
		{
            if (string.IsNullOrEmpty(file))
                return "";

			var _file = file.Replace("\\","/");
			int pos = _file.LastIndexOf('/');
			return _file.Substring(pos+1);//,file->Length-pos-1);
		}

		public string GetFileExtension(string file)
		{
            if (string.IsNullOrEmpty(file))
                return "";

			var _file = file.Replace("\\", "/");
			int pos = _file.LastIndexOf('.');
			return _file.Substring(pos+1);
		}

        public bool IsFileEqual(string file1, string file2)
		{
            if (string.Equals(file1, file2))
                return true;

            if (string.IsNullOrEmpty(file1) || string.IsNullOrEmpty(file2))
                return false;

			var f1 = file1.Replace("\\", "/");
			var f2 = file2.Replace("\\", "/");
			return f1 == f2;
		}
		public bool IsDirectoryEqual(string dir1, string dir2)
		{
			string d1 = dir1.Replace("\\", "/");
			string d2 = dir2.Replace("\\", "/");
			if(d1[d1.Length - 1] == '/')
				d1 = d1.Remove(d1.Length - 1);
			if(d2[d2.Length - 1] == '/')
				d2 = d2.Remove(d2.Length - 1);
			return d1 == d2;
		}
        
        public string _GetRelativePathFromAbsPath(string file)
        {
            return _GetRelativePathFromAbsPath(file, "");
        //    pin_ptr<const System::Char> rootPath = PtrToStringChars(mRoot);
        //    pin_ptr<const System::Char> absPath = PtrToStringChars(file);
        //    VString str = _vfxFileGetRelativePath( rootPath , absPath );
        //    return gcnew System::String(str);
        }

		public string _GetRelativePathFromAbsPath(string file, string cPath = "")
		{
            if(string.IsNullOrEmpty(cPath))
                cPath = mRoot;

            string strPath2 = _GetAbsPathFromRelativePath(file);
            string strPath1 = _GetAbsPathFromRelativePath(cPath);

            strPath1 = strPath1.Replace("\\", "/");
            strPath2 = strPath2.Replace("\\", "/");

            if (strPath1 == strPath2)
                return "";

            if(!strPath1.EndsWith("/"))
                strPath1 += "/";
            int intIndex = -1;
            int intPos = strPath1.IndexOf("/");

            while(intPos >= 0)
            {
                intPos++;

                if(string.Compare(strPath1, 0, strPath2, 0, intPos, true) != 0)
                    break;

                intIndex = intPos;
                intPos = strPath1.IndexOf("/", intPos);
            }

            if(intIndex >= 0)
            {
                strPath2 = strPath2.Substring(intIndex);
                intPos = strPath1.IndexOf("/", intIndex);

                while(intPos >= 0)
                {
                    strPath2 = "../" + strPath2;
                    intPos = strPath1.IndexOf("/", intPos + 1);
                }
            }

            return strPath2;
		}

		public string _GetAbsPathFromRelativePath(string file)
		{
            if (string.IsNullOrEmpty(file))
                return "";

            if (System.IO.Path.IsPathRooted(file))
                return file.Replace("\\", "/");

            return System.IO.Path.Combine(mRoot, file).Replace("\\", "/");
            //pin_ptr<const System::Char> rootPath = PtrToStringChars(mRoot);
            //pin_ptr<const System::Char> absPath = PtrToStringChars(file);
            //VString str = _vfxFileGetFullPath( rootPath , absPath );
            //return gcnew System::String(str);
		}

        public IntPtr NewDownloadRes2Memory(string file, EFileType type, bool bHold)
        {
            F2M f2m;
            if (mReadOnlyFiles.TryGetValue(file, out f2m))
            {
                unsafe
                {
                    DllImportAPI.VFile2Memory_AddRef(f2m.mResource);
                }
                return f2m.mResource;
            }
            string finalStr = mRoot + file;
            var res = DllImportAPI.VFile2Memory_DownloadAndF2M(finalStr);
            if (res == IntPtr.Zero)
                return IntPtr.Zero;
            if (bHold)
            {
                f2m = new F2M(res);
                f2m.mType = type;
                mReadOnlyFiles.Add(file, f2m);
            }
            return res;
        }
		public IntPtr NewRes2Memory( string file , EFileType type , bool bHold )
		{
			return OpenFileForRead(file,type,bHold);
		}
		public void ReleaseRes2Memory(IntPtr res)
		{
            unsafe
            {
                DllImportAPI.VFile2Memory_Release(res);
            }
            //VFile2Memory* res2Mem = (VFile2Memory*)res.ToPointer();
            //if(res2Mem)
            //{
            //    res2Mem.Release();
            //}
		}

		public IntPtr NewFileWriter( string file , EFileType type )
		{
			return OpenFileForWrite(file,type);
		}
		public void ReleaseFileWriter(IntPtr res)
		{
            unsafe
            {
                DllImportAPI.VFile_Close(res);
                DllImportAPI.VFile_Delete(res);
            }
            //VFile* file = (VFile*)res.ToPointer();
            //Cli_Safe_Delete(file);
		}

		public IntPtr OpenFileForRead( string file , EFileType type , bool bHold )
		{
			F2M f2m;
			if( mReadOnlyFiles.TryGetValue(file, out f2m) )
			{
                unsafe
                {
                    DllImportAPI.VFile2Memory_AddRef(f2m.mResource);
                }
				return f2m.mResource;
			}
			string finalStr = _GetAbsPathFromRelativePath(file);
			if(System.IO.File.Exists( finalStr + ".reloader" ) )
			{
				string target = finalStr +".reloader";
                //pin_ptr<const System::Char> pTarget = PtrToStringChars(target);
                //pin_ptr<const System::Char> pSource = PtrToStringChars(finalStr);
                //::MoveFileEx( pTarget , pSource , MOVEFILE_REPLACE_EXISTING );
                if(System.IO.File.Exists(finalStr))
                    System.IO.File.Delete(finalStr);
                System.IO.File.Move(target, finalStr);
			}
			//pin_ptr<const System::Char> pChar = PtrToStringChars( finalStr );
			var res = DllImportAPI.VFile2Memory_F2M( finalStr , false );
			if(res==IntPtr.Zero)
				return IntPtr.Zero;
			if(bHold)
			{
				f2m = new F2M(res);
				f2m.mType = type;
				mReadOnlyFiles.Add(file,f2m);
			}
			return res;
		}

		public void CloseFileForRead( string file )
		{
			F2M f2m;
			if( mReadOnlyFiles.TryGetValue(file, out f2m) )
			{
				f2m.Cleanup();
				f2m = null;
				mReadOnlyFiles.Remove( file );
			}
		}

		public IntPtr OpenFileForWrite( string file , EFileType type )
		{
            unsafe
            {
                IntPtr io = DllImportAPI.VFile_New();

                var fileName = _GetAbsPathFromRelativePath(file);
                if(DllImportAPI.VFile_Open(io, fileName, (UInt32)(OpenFlags.modeWrite | OpenFlags.modeCreate)) == 0)
                {
                    fileName = _GetAbsPathFromRelativePath(file) + ".reloader";
                    if(DllImportAPI.VFile_Open(io, fileName, (UInt32)(OpenFlags.modeWrite | OpenFlags.modeCreate)) == 0)
                    {
                        DllImportAPI.VFile_Delete(io);
                        return IntPtr.Zero;
                    }
                }

                return io;
            }

            //pin_ptr<const System::Char> pChar = PtrToStringChars( mRoot+"/"+file );
            //VFile* io = new VFile();

            //if( io.Open(pChar,VFile::modeWrite|VFile::modeCreate)==FALSE )
            //{
            //    pChar = PtrToStringChars( mRoot+"/"+file+".reloader" );
            //    if( io.Open(pChar,VFile::modeWrite|VFile::modeCreate)==FALSE )
            //    {
            //        delete io;
            //        return NULL;
            //    }
            //}

            //return io;
		}

		public void QueryNewFileFromServer(string file)
		{
			//类似Task,Item,Role这样的template，我们要做的事，是从服务器发下来Guid的同时，发下来一个MD5文件校验，如果客户端本地没有这个文件，Query
			//如果客户端有这个文件，就做一次文件的MD5校验，确认是否需要Query

			//类似Mesh,Action等纯数据文件，我们每次启动客户端的时候，根据客户端的最后更新时间点，从服务器拉下来一个ChangeList（只有改变，不包括增加）
			//检查更新文件列表MD5，需要更新的，开始Query，缺少的不用管，一切都是异步的，不要等待文件好了才工作，这样需要一个资源文件需求蜕化的列表
			//如果需要的文件还没Download Ready，那么用一个替代模型
		}
        
        public static System.Text.Encoding GetEncoding(string absFileName)
        {
            if (System.IO.File.Exists(absFileName))
                return System.Text.Encoding.Default;
            var stream = new System.IO.FileStream(absFileName, System.IO.FileMode.Open);

            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM   
            Encoding reVal = Encoding.Default;
 
            BinaryReader r = new BinaryReader(stream, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(4);
            if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            else
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    reVal = Encoding.UTF8;
                }
                else
                {
                    int i;
                    int.TryParse(stream.Length.ToString(), out i);
                    ss = r.ReadBytes(i);
 
                    if (IsUTF8Bytes(ss))
                        reVal = Encoding.UTF8;
                }
            }
            r.Close();
            return reVal;
 
        }

        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数   
            byte curByte; //当前分析的字节.   
            for (int i = 0; i<data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前   
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　   
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1   
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式!");
            }
            return true;
        }

    }
}

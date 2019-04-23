using System;
using System.Collections.Generic;

namespace CSUtility.Compress.SevZip
{
    internal class CompressContext_SevenZip
    {
        public delegate void Delegate_Debug(string file);
        //public Delegate_Debug OnDebug;

        static CompressContext_SevenZip smInstance = new CompressContext_SevenZip();
        public static CompressContext_SevenZip Instance
        {
            get { return smInstance; }
        }

        class FileInfo
        {
            public Int64 FileSize = 0;              // 文件大小
            public Int64 CompressedSize = 0;        // 压缩后文件大小
            public string FileNameInfo = "";        // 文件名，包含相对路径
            public Byte IsFolder = 0;               // 是否是文件夹
            public Int64 StreamPos = 0;             // 在Stream中的位置
        }

        // 压缩参数
        Int32 mDictionary = 1 << 20;
        Int32 mPosStateBits = 2;
        Int32 mLitContextBits = 3; // for normal files
        // UInt32 mLitContextBits = 0; // for 32-bit data
        Int32 mLitPosBits = 0;
        // UInt32 mLitPosBits = 2; // for 32-bit data
        Int32 mAlgorithm = 2;
        Int32 mNumFastBytes = 128;
        string mMf = "bt4";
        bool mEos = false;// parser[(int)Key.EOS].ThereIs || stdInMode;

        // 固定文件头
        string mSignatureStr = "viccp";
        // 文件版本号
        string mVersion = "0.1";

        // 实例：inputFiles("aa.txt")(相对路径), rootDir="d:/xxx/" zipFileName = "d:/xxx.zip"
        public void CompressFile(string inputFile, string rootDir, string zipFileName)
        {
            CompressFile(new string[] { inputFile }, rootDir, zipFileName);
        }

        // 实例：inputFiles("aa.txt","cc/bb.txt")(相对路径), rootDir="d:/xxx/" zipFileName = "d:/xxx.zip"
        public void CompressFile(string[] inputFiles, string rootDir, string zipFileName)
        {
            using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {

                CSUtility.Compress.SevZip.CoderPropID[] propIDs = 
				    {
					    CSUtility.Compress.SevZip.CoderPropID.DictionarySize,
					    CSUtility.Compress.SevZip.CoderPropID.PosStateBits,
					    CSUtility.Compress.SevZip.CoderPropID.LitContextBits,
					    CSUtility.Compress.SevZip.CoderPropID.LitPosBits,
					    CSUtility.Compress.SevZip.CoderPropID.Algorithm,
					    CSUtility.Compress.SevZip.CoderPropID.NumFastBytes,
					    CSUtility.Compress.SevZip.CoderPropID.MatchFinder,
					    CSUtility.Compress.SevZip.CoderPropID.EndMarker
				    };
                object[] properties = 
				    {
					    (Int32)(mDictionary),
					    (Int32)(mPosStateBits),
					    (Int32)(mLitContextBits),
					    (Int32)(mLitPosBits),
					    (Int32)(mAlgorithm),
					    (Int32)(mNumFastBytes),
					    mMf,
					    mEos
				    };

                // 设置压缩参数
                var encoder = new SevenZip.Compression.LZMA.Encoder();// SevenZip.Compression.LZMA.Encoder();
                encoder.SetCoderProperties(propIDs, properties);
                encoder.WriteCoderProperties(mem);

                List<FileInfo> fileInfos = new List<FileInfo>();

                foreach (var file in inputFiles)
                {
                    var fileName = rootDir + file;

                    var fileInfo = new FileInfo();
                    fileInfo.FileNameInfo = file;

                    if (System.IO.Directory.Exists(fileName))
                        fileInfo.IsFolder = 1;
                    else if (System.IO.File.Exists(fileName))
                    {
                        fileInfo.IsFolder = 0;

                        using (var inStream = new System.IO.FileStream(rootDir + file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            // 因为Length转int的限制，这里不支持大于4个G的文件
                            var length = (int)(inStream.Length);

                            fileInfo.FileSize = inStream.Length;
                            fileInfo.StreamPos = mem.Position;

                            using (System.IO.MemoryStream compressedStream = new System.IO.MemoryStream())
                            {
                                CompressStream(inStream, compressedStream, encoder);
                                fileInfo.CompressedSize = compressedStream.Length;

                                byte[] content = new byte[compressedStream.Length];
                                compressedStream.Position = 0;
                                compressedStream.Read(content, 0, content.Length);
                                mem.Write(content, 0, content.Length);

                            }

                        }
                    }
                    else
                        continue;

                    fileInfos.Add(fileInfo);
                }


                CSUtility.Support.XndHolder xndHolder = CSUtility.Support.XndHolder.NewXNDHolder();

                // 文件签名
                var att = xndHolder.Node.AddAttrib("Sig");
                att.BeginWrite();
                att.Write(mSignatureStr);
                att.EndWrite();

                // 文件版本
                att = xndHolder.Node.AddAttrib("Ver");
                att.BeginWrite();
                att.Write(mVersion);
                att.EndWrite();
            
                att = xndHolder.Node.AddAttrib("Header");
                att.BeginWrite();
                att.Write(fileInfos.Count);
                foreach (var fileInfo in fileInfos)
                {
                    att.Write(fileInfo.FileSize);
                    att.Write(fileInfo.CompressedSize);
                    att.Write(fileInfo.FileNameInfo);
                    att.Write(fileInfo.IsFolder);
                    att.Write(fileInfo.StreamPos);
                }
                att.EndWrite();

                att = xndHolder.Node.AddAttrib("Data");
                att.BeginWrite();
                int memLen = (int)(mem.Length);
                att.Write(memLen);
                var memBytes = new byte[memLen];
                mem.Position = 0;
                mem.Read(memBytes, 0, memLen);
                att.Write(memBytes);
                att.EndWrite();

                CSUtility.Support.XndHolder.SaveXND(zipFileName, xndHolder);
            }

        }

        private void CompressStream(System.IO.Stream inStream, System.IO.Stream outStream, SevenZip.Compression.LZMA.Encoder encoder)
        {
            //////var inStream = new System.IO.FileStream(rootDir + inputFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //////var outStream = new System.IO.FileStream(zipFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

            //encoder.WriteCoderProperties(outStream);

            //Int64 fileSize = inStream.Length;
            //for (int i = 0; i < 8; i++)
            //    outStream.WriteByte((Byte)(fileSize >> (8 * i)));

            encoder.Code(inStream, outStream, -1, -1, null);  


            //// 记录文件个数
            //int fileCount = 1;
            //for (int i = 0; i < 4; i++)
            //    outStream.WriteByte((Byte)(fileCount >> (4 * i)));
            //// 记录文件名
            //int fileNameLength = inputFile.Length;
            //for (int i = 0; i < 4; i++)
            //    outStream.WriteByte((Byte)(fileNameLength >> (4 * i)));
            //var strBytes = System.Text.Encoding.Default.GetBytes(inputFile);
            //outStream.Write(strBytes, 0, strBytes.Length);

            //// 记录文件大小
            //Int64 fileSize;
            //fileSize = inStream.Length;
            //for (int i = 0; i < 8; i++)
            //    outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            //encoder.Code(inStream, outStream, -1, -1, null);

            //inStream.Close();
            //outStream.Close();
        }

        // 实例： zipFileName = "d:/xxx.zip"  targetFolder="d:/xxx/"
        public bool UnCompressFile(string zipFileName, string targetFolder)
        {
            CSUtility.Support.XndHolder xndHolder = CSUtility.Support.XndHolder.LoadXND(zipFileName);
            if (xndHolder == null)
                return false;

            //try
            {
                //zipFileName = zipFileName.Replace("\\", "/");

                var att = xndHolder.Node.FindAttrib("Sig");
                if (att == null)
                {
                    xndHolder.Node.TryReleaseHolder();
                    return false;
                }

                // 签名
                string sig = "";
                att.BeginRead();
                att.Read(out sig);
                att.EndRead();
                if (sig != mSignatureStr)
                {
                    xndHolder.Node.TryReleaseHolder();
                    return false;
                }

                // 版本号
                string ver = "";
                att = xndHolder.Node.FindAttrib("Ver");
                if (att == null)
                {
                    xndHolder.Node.TryReleaseHolder();
                    return false;
                } 
                att.BeginRead();
                att.Read(out ver);
                att.EndRead();

                switch (ver)
                {
                    case "0.1":
                        {
                            att = xndHolder.Node.FindAttrib("Header");
                            if (att == null)
                            {
                                xndHolder.Node.TryReleaseHolder();
                                return false;
                            }
                            
                            int fileCount = 0;
                            att.BeginRead();
                            att.Read(out fileCount);
                            FileInfo[] fileInfos = new FileInfo[fileCount];
                            for (int i = 0; i < fileCount; i++)
                            {
                                var fileInfo = new FileInfo();
                                att.Read(out fileInfo.FileSize);
                                att.Read(out fileInfo.CompressedSize);
                                att.Read(out fileInfo.FileNameInfo);
                                att.Read(out fileInfo.IsFolder);
                                att.Read(out fileInfo.StreamPos);

                                fileInfos[i] = fileInfo;
                            }

                            att.EndRead();

                            att = xndHolder.Node.FindAttrib("Data");
                            if (att == null)
                            {
                                xndHolder.Node.TryReleaseHolder();
                                return false;
                            }

                            att.BeginRead();
                            int len = 0;
                            att.Read(out len);
                            byte[] membytes;
                            att.Read(out membytes, len);
                            att.EndRead();
                            
                            // 压缩后的所有数据
                            System.IO.MemoryStream compressedStream = new System.IO.MemoryStream(membytes);
                            byte[] properties = new byte[5];
                            if(compressedStream.Read(properties, 0, 5) != 5)
                                throw (new Exception("input param is too short"));

                            var decoder = new SevenZip.Compression.LZMA.Decoder();
                            decoder.SetDecoderProperties(properties);

                            foreach (var info in fileInfos)
                            {
                                if(info.IsFolder == 1)
                                {
                                    // 文件夹
                                    System.IO.Directory.CreateDirectory(targetFolder + info.FileNameInfo);
                                }
                                else
                                {
                                    var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(targetFolder + info.FileNameInfo);
                                    if (!System.IO.Directory.Exists(path))
                                        System.IO.Directory.CreateDirectory(path);

                                    // 文件
                                    var targetFile = targetFolder + info.FileNameInfo;
                                    var fileStream = new System.IO.FileStream(targetFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                                    var fileCompressBytes = new byte[info.CompressedSize];

                                    compressedStream.Position = info.StreamPos;
                                    compressedStream.Read(fileCompressBytes, 0, fileCompressBytes.Length);

                                    var fileCompressStream = new System.IO.MemoryStream(fileCompressBytes);

                                    decoder.Code(fileCompressStream, fileStream, fileCompressStream.Length, info.FileSize, null);

                                    fileStream.Close();

                                }
                            }
                        }
                        break;
                }

                xndHolder.Node.TryReleaseHolder();
                return true;
            }
            //catch (System.Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex.ToString());
            //}

            //xndHolder.Node.TryReleaseHolder();

            //return false;
        }
    }
}

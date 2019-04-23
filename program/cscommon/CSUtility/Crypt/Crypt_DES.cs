using System;
using System.Text;

namespace CSUtility.Crypt
{
    public class Crypt_DES
    {
        #region 加密解密

        //private System.String xmlKey = "<RSAKeyValue><Modulus>8XAFPbsnWl0VwHph0YUnIvpzd+BUwOKcP+6KOGKd2eZy0wyE1ymr4zpPorW1xuq6UH/l/4wMMDMf+Cx/dbn8Ar1OtEp5+riMjFf5I3SJu3Y3Z11qDkmGvHKncQR+d6TsauW7ShNVSqyFfj5KvZ6w2CjdKNjs7+wDUkhTAhwt2rc=</Modulus><Exponent>AQAB</Exponent><P>/LcoONwfXKJDpl4DUTApTRpW66iUgZmCzz0cHdA9KKm6oyKvYrzmjKsY25POnQqyAnjBLEWzJvmCbv00p31SwQ==</P><Q>9JNW2LDrHr/0wwYkjIysadcItOkHXSwigAgeYF1DiPb/4asrOkkNo8GnZCk03g4/21+o3LEZEjv2wqRc8Zwjdw==</Q><DP>53AJmwEki55jHBKPMSRNPrp3jfxmfMYMwSQzAq65w+qk7VAPlPpwnbT33+feWX35BkB2kMgISRJKPMERHq6sQQ==</DP><DQ>tykAiRc1fbnT25nmFTvkgv7/DjzVvvkmfSuXVCOESDZyOtatcOD0zgZhDamuZf3V5LwnAd+/HuC5U9szn92G0Q==</DQ><InverseQ>iidJkbJj5QcVS/oHlqknW1C7zzy7eb4UCIaQb4hbXanJGmNYCyE/adIuZmCy1+yxcbTPhnfP9YTrg5ryGtUCmQ==</InverseQ><D>WnbKv/KcMDOortIsTMFDhMuq8EPR/ePq/9l1WOM6mibK52FNdQFcMmvq4uCLF9ljrj3+A96JwpWlaOvjZk0lq3bGLNsDjJCeh4xhrlM2uu8MYmHUCCsP5oePiTaSp7VJqvDmhlkiKRmee7IhkJk9lUCOqqBpbb7Ven4SRkIXJgE=</D></RSAKeyValue>";
        //private System.String xmlPublicKey = "<RSAKeyValue><Modulus>8XAFPbsnWl0VwHph0YUnIvpzd+BUwOKcP+6KOGKd2eZy0wyE1ymr4zpPorW1xuq6UH/l/4wMMDMf+Cx/dbn8Ar1OtEp5+riMjFf5I3SJu3Y3Z11qDkmGvHKncQR+d6TsauW7ShNVSqyFfj5KvZ6w2CjdKNjs7+wDUkhTAhwt2rc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public static UInt32 CurrentVersion
        {
            get { return 0; }
        }

        #region DES

        public static System.String GetDesKey(UInt32 version)
        {
            switch (version)
            {
                case 0:
                    return CSUtility.Program.GetTempKey(version) + "di.q9";
            }

            return "dko298vb";
        }

        /// <summary> 
        /// Encrypt the string 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="strText">string</param> 
        /// <param name="strEncrKey">key</param> 
        /// <returns></returns> 
        public static string DesEncrypt(string strText, string strEncrKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));
            var des = new System.Security.Cryptography.DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
            var ms = new System.IO.MemoryStream();
            var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(byKey, IV), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        /// <summary> 
        /// Encrypt the string 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="inputByteArray">byteToEncrypt</param> 
        /// <param name="strEncrKey">key</param> 
        /// <returns></returns> 
        public static string DesEncrypt(byte[] inputByteArray, string strEncrKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));
            var des = new System.Security.Cryptography.DESCryptoServiceProvider();
            var ms = new System.IO.MemoryStream();
            var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(byKey, IV), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary> 
        /// Decrypt string 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="strText">Decrypt string</param> 
        /// <param name="sDecrKey">key</param> 
        /// <returns>output string</returns> 
        public static string DesDecrypt(string strText, string sDecrKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] inputByteArray = new Byte[strText.Length];

            byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
            var des = new System.Security.Cryptography.DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(strText);
            var ms = new System.IO.MemoryStream();
            var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(byKey, IV), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            Encoding encoding = new UTF8Encoding();
            return encoding.GetString(ms.ToArray());
        }
        /// <summary> 
        /// Decrypt string 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="inputByteArray">Decrypt byte arrary</param> 
        /// <param name="sDecrKey">key</param> 
        /// <returns>output string</returns> 
        public static string DesDecrypt(byte[] inputByteArray, string sDecrKey)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
            var des = new System.Security.Cryptography.DESCryptoServiceProvider();
            var ms = new System.IO.MemoryStream();
            var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(byKey, IV), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            Encoding encoding = new UTF8Encoding();
            return encoding.GetString(ms.ToArray());
        }
        /// <summary> 
        /// Encrypt files 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="m_InFilePath">Encrypt file path</param> 
        /// <param name="m_OutFilePath">output file</param> 
        /// <param name="strEncrKey">key</param> 
        public static void DesEncrypt(string m_InFilePath, string m_OutFilePath, string strEncrKey)
        {
            if (!System.IO.File.Exists(m_InFilePath))
                return;

            System.IO.FileInfo info = new System.IO.FileInfo(m_OutFilePath);
            if (!info.Directory.Exists)
                info.Directory.Create();

            byte[] byKey = null;
            byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));

            System.IO.BinaryReader binReader = new System.IO.BinaryReader(System.IO.File.Open(m_InFilePath, System.IO.FileMode.Open));

            System.Byte[] data = binReader.ReadBytes((int)binReader.BaseStream.Length);
            //var str = binReader.ReadString();
            binReader.Close();

            var str = DesEncrypt(data, strEncrKey);

            var holder = CSUtility.Support.XndHolder.NewXNDHolder();
            var att = holder.Node.AddAttrib("DataAtt");
            att.BeginWrite();
            att.Write(CurrentVersion);
            att.Write(str);
            att.EndWrite();

            CSUtility.Support.XndHolder.SaveXND(m_OutFilePath, holder);
        }

        /// <summary> 
        /// Decrypt files 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="m_InFilePath">Decrypt filepath</param> 
        /// <param name="m_OutFilePath">output filepath</param> 
        /// <param name="sDecrKey">key</param> 
        public static void DesDecrypt(string m_InFilePath, string m_OutFilePath, string sDecrKey)
        {
            if (!System.IO.File.Exists(m_InFilePath))
                return;

            System.IO.FileInfo info = new System.IO.FileInfo(m_OutFilePath);
            if (!info.Directory.Exists)
                info.Directory.Create();

            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));

            CSUtility.Support.XndHolder holder = CSUtility.Support.XndHolder.LoadXND(m_InFilePath);
            var att = holder.Node.FindAttrib("DataAtt");
            if (att == null)
                return;

            att.BeginRead();

            UInt32 keyVer;
            att.Read(out keyVer);
            string str;
            att.Read(out str);

            att.EndRead();

            var decryptStr = DesDecrypt(str, GetDesKey(keyVer));
            var strBytes = Encoding.GetEncoding("GB2312").GetBytes(decryptStr);

            using (var fout = new System.IO.FileStream(m_OutFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
            {
                fout.SetLength(0);
                fout.Write(strBytes, 0, strBytes.Length);
            }
        }

        #endregion

        #endregion
    }
}

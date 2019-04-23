namespace CSUtility.Crypt
{
    public class Crypt_RSA
    {
        #region RSA
        // 加密
        public System.Byte[] Encrypt_RSA(System.String xmlPublicKey, System.String strEncryptString)
        {
            try
            {
                System.Byte[] plainTextBArray;
                System.Byte[] cypherTextBArray;
                //System.String result;
                System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPublicKey);
                System.Text.UnicodeEncoding unicodeEncoding = new System.Text.UnicodeEncoding();
                plainTextBArray = unicodeEncoding.GetBytes(strEncryptString);
                cypherTextBArray = rsa.Encrypt(plainTextBArray, false);
                //result = System.Convert.ToBase64String(cypherTextBArray);
                //return result;
                return cypherTextBArray;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public System.String Decrypt_RSA(System.String xmlPrivateKey, System.Byte[] decryptArray)
        {
            try
            {
                //array<System.Byte> plainTextBArray;
                System.Byte[] dypherTextBArray;
                System.String result;
                System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPrivateKey);
                //plainTextBArray = System.Convert.FromBase64String(strDecryptString);

                dypherTextBArray = rsa.Decrypt(decryptArray, false);
                System.Text.UnicodeEncoding unicodeEncoding = new System.Text.UnicodeEncoding();
                result = unicodeEncoding.GetString(dypherTextBArray);
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

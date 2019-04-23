namespace CSUtility.Crypt
{
    public class Crypt_AES
    {
        #region AES

        private System.Byte[] aesKey = new System.Byte[] { 0x10, 0x11, 0x31, 0xAA, 0x54, 0x77, 0x29, 0xCE, 0xA0, 0xB1, 0xFF, 0x10, 0x43, 0x23, 0x49, 0x38, 0x10, 0x98, 0x81, 0xE3, 0xA8, 0xAA, 0xB3, 0xF3, 0x83, 0xB1, 0xD9, 0xF3, 0x18, 0x38, 0x10, 0x47 };
        private System.Byte[] aesIV = new System.Byte[] { 0xA2, 0xB3, 0xC4, 0xD5, 0xE6, 0xF7, 0x32, 0x01, 0x78, 0x45, 0x73, 0x19, 0x10, 0x91, 0x75, 0x23 };

        //public System.Byte[] Encrypt_AES(System.String plainText, System.Byte[] key, System.Byte[] iV)
        //{
        //    if(System.String.IsNullOrEmpty(plainText))
        //        throw new System.ArgumentNullException("Encrypt_AES plainText is illegal");
        //    if(key == null || key.Length <= 0)
        //        throw new System.ArgumentNullException("Encrypt_AES key is illegal");
        //    if(iV == null || iV.Length <= 0)
        //        throw new System.ArgumentNullException("Encrypt_AES iV is illegal");

        //    System.Byte[] encrypted;
        //    System.Security.Cryptography.Aes aesAlg = System.Security.Cryptography.Aes.Create();
        //    aesAlg.Key = key;
        //    aesAlg.IV = iV;

        //    System.Security.Cryptography.ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        //    System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream();
        //    System.Security.Cryptography.CryptoStream csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write);

        //    System.IO.StreamWriter swEncrypt = new System.IO.StreamWriter(csEncrypt);
        //    swEncrypt.Write(plainText);			
        //    swEncrypt.Close();
        //    encrypted = msEncrypt.ToArray();

        //    csEncrypt.Clear();
        //    csEncrypt.Close();
        //    msEncrypt.Close();
        //    aesAlg.Clear();

        //    return encrypted;
        //}
        //public System.String Decrypt_AES(System.Byte[] cipherText, System.Byte[] key, System.Byte[] iV)
        //{
        //    if(cipherText == null || cipherText.Length <= 0)
        //        throw new System.ArgumentNullException("Decrypt_AES cipherText is illegal");
        //    if(key == null || key.Length <= 0)
        //        throw new System.ArgumentNullException("Encrypt_AES key is illegal");
        //    if(iV == null || iV.Length <= 0)
        //        throw new System.ArgumentNullException("Encrypt_AES iV is illegal");

        //    System.String plainText = null;

        //    System.Security.Cryptography.Aes aesAlg = System.Security.Cryptography.Aes.Create();
        //    aesAlg.Key = key;
        //    aesAlg.IV = iV;

        //    System.Security.Cryptography.ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        //    System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(cipherText);
        //    System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read);

        //    System.IO.StreamReader srDecrypt = new System.IO.StreamReader(csDecrypt);
        //    plainText = srDecrypt.ReadToEnd();
        //    srDecrypt.Close();

        //    csDecrypt.Clear();
        //    csDecrypt.Close();
        //    msDecrypt.Close();

        //    aesAlg.Clear();

        //    return plainText;
        //}
        #endregion
    }
}

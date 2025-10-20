﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Crypter.Core
{
    public class Encryption
    {
        public static byte[] Encrypt(byte[] payload, EncryptionType encryption, string key)
        {
            byte[] encrypted;

            if(encryption == EncryptionType.AES)
            {
                encrypted = AES_Encrypt(payload, key);
            }
            else //if (encryption == EncryptionType.XOR)
            {
                encrypted = XOR_Encrypt(payload, key);
            }
            /*else
            {
                encrypted = RSA_Encrypt(payload, key);
            }*/

            return encrypted;
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, string encKey)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 026, 020, 202, 234, 136, 123, 069, 047 };
            using (MemoryStream ms = new MemoryStream())              
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var passwordBytes = Encoding.UTF8.GetBytes(encKey);
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private static byte[] XOR_Encrypt(byte[] bytesToBeEncrypted, string key)
        {
            byte[] encryptedBytes = new byte[bytesToBeEncrypted.Length];

            for(int i = 0; i < bytesToBeEncrypted.Length; i++)
            {
                encryptedBytes[i] = ((byte)(bytesToBeEncrypted[i] ^ key[i % key.Length]));
            }

            return encryptedBytes;
        }

        private static byte[] RSA_Encrypt(byte[] bytesToBeEncrypted, string publicKey)
        {
            byte[] encryptedBytes;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                encryptedBytes = rsa.Encrypt(bytesToBeEncrypted, true);
            }
            return encryptedBytes;
        }
    }
}

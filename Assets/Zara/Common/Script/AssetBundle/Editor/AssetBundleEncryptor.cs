using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Zara.Common.ExAssetBundle.Internal;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class AssetBundleEncryptor
    {
        public string PlaneAssetBundleDirPath { get; }
        public string EncryptedAssetBundleDirPath { get; }
        ICryptoTransform encryptor;

        public AssetBundleEncryptor(string planeAssetBundleDirPath, string encryptedAssetBundleDirPath, IEncryptionKey encryptionKey)
        {
            this.PlaneAssetBundleDirPath = planeAssetBundleDirPath;
            this.EncryptedAssetBundleDirPath = encryptedAssetBundleDirPath;

            RijndaelManaged rijndael = Decryptor.CreateRijndael(encryptionKey);
            encryptor = rijndael.CreateEncryptor();
        }

        public string Encrypt(string assetBundleName, bool shouldOverwrite)
        {
            string planeAssetBundlePath = $"{PlaneAssetBundleDirPath}{assetBundleName}";
            return Encrypt(planeAssetBundlePath, assetBundleName, shouldOverwrite);
        }

        // return value = encrypted asset bundle path
        public string Encrypt(string planeAssetBundlePath, string assetBundleName, bool shouldOverwrite)
        {
            string encryptedAssetBundlePath = $"{EncryptedAssetBundleDirPath}{assetBundleName}";
            if (!shouldOverwrite && File.Exists(encryptedAssetBundlePath))
                return encryptedAssetBundlePath;

            try
            {
                byte[] planeBytes = File.ReadAllBytes(planeAssetBundlePath);
                byte[] encryptedBytes;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(planeBytes, 0, planeBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        encryptedBytes = memoryStream.ToArray();
                    }
                }

                var dirPath = Path.GetDirectoryName(encryptedAssetBundlePath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                File.WriteAllBytes(encryptedAssetBundlePath, encryptedBytes);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return encryptedAssetBundlePath;
        }
    }
}
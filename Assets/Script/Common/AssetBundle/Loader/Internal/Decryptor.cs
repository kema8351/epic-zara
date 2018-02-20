using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Zara.Common.ExAssetBundle.Internal
{
    public class Decryptor
    {
        const int KeySize = 128;
        const int BufferSize = 64;

        ICryptoTransform decryptor;

        public Decryptor(IEncryptionKey encryptionKey)
        {
            RijndaelManaged rijndael = CreateRijndael(encryptionKey);
            decryptor = rijndael.CreateDecryptor();
        }

        public static RijndaelManaged CreateRijndael(IEncryptionKey encryptionKey)
        {
            return new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                BlockSize = 128,
                KeySize = KeySize,
                Padding = PaddingMode.PKCS7,
                Key = encryptionKey.GetKeyBytes(KeySize),
                IV = encryptionKey.GetIvBytes(),
            };
        }

        public byte[] Decrypt(byte[] bytes)
        {
            byte[] result;

            using (MemoryStream inputMemoryStream = new MemoryStream(bytes))
            {
                using (MemoryStream outputMemoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(inputMemoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(cryptoStream))
                        {
                            byte[] buffer = new byte[BufferSize];
                            while (true)
                            {
                                int count = binaryReader.Read(buffer, 0, buffer.Length);
                                if (count == 0)
                                    break;
                                outputMemoryStream.Write(buffer, 0, count);
                            }

                            result = outputMemoryStream.ToArray();
                        }
                    }
                }
            }

            return result;
        }
    }
}
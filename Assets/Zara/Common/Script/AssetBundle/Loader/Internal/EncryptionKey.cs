using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


namespace Zara.Common.ExAssetBundle.Internal
{
    public class EncryptionKey : IEncryptionKey
    {
        public byte[] GetIvBytes()
        {
            string initialVector = "0123456789abcdef";
            return Encoding.ASCII.GetBytes(initialVector);
        }

        public byte[] GetKeyBytes(int keySize)
        {
            string key = "0123456789abcdef";
            string saltKey = "0123456789abcdef";
            return new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(saltKey)).GetBytes(keySize / 8);
        }
    }

    public interface IEncryptionKey
    {
        byte[] GetIvBytes();
        byte[] GetKeyBytes(int keySize);
    }
}
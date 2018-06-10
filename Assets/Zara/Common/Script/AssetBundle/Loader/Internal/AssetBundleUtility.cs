using System.IO;
using UnityEngine;

namespace Zara.Common.ExAssetBundle.Internal
{
    public static class AssetBundleUtility
    {
        public static string CheckEndSlash(string str)
        {
            if (str.EndsWith("/"))
                return str;
            else
                return $"{str}/";
        }

        static readonly string LocalRootDirPath = CheckEndSlash(
#if UNITY_IOS
                Application.temporaryCachePath
#else
                Application.persistentDataPath
#endif
            );
        static readonly string LocalDirPath = $"{LocalRootDirPath}{ConstAssetBundle.LocalDirName}/";

        public static string GetLocalStoragePath(string assetBundleName)
        {
            return $"{LocalDirPath}{assetBundleName}";
        }

        public static string GetSceneName(string path)
        {
            string sceneName = Path.GetFileNameWithoutExtension(path);
            return sceneName;
        }
    }
}
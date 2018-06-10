using System;
using UnityEngine;

namespace Zara.Common.ExAssetBundle
{
    public interface IAssetEntry
    {
        void Load<T>(string assetEntryKey, Action<T> onLoaded) where T : UnityEngine.Object;
        void LoadMultipleAssets<T>(string assetEntryKey, Action<T[]> onLoaded) where T : UnityEngine.Object;
        void LoadPrefab<T>(string assetEntryKey, Action<T> onLoaded) where T : Component;
        void LoadTable(bool forciblyDownload);
        void LoadScene(string assetEntryKey);
        void LoadSceneAdditive(string assetEntryKey);
        bool IsTableLoaded { get; }
        AssetBundleTable Table { get; }
    }
}
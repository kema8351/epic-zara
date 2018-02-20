using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Internal
{
    public static class LinkerFuncs
    {
        public static class Single<T> where T : UnityEngine.Object
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AssetBundleRequest> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestSingle<T>;
            public static readonly Func<AssetBundleRequest, T> GetAssetFunc = LinkerFuncs.GetAssetSingle<T>;
        }

        public static class Multiple<T> where T : UnityEngine.Object
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AssetBundleRequest> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestMultiple<T>;
            public static readonly Func<AssetBundleRequest, T[]> GetAssetFunc = LinkerFuncs.GetAssetMultiple<T>;
        }

        public static class Prefab<T> where T : Component
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AssetBundleRequest> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestSingle<GameObject>;
            public static readonly Func<AssetBundleRequest, T> GetAssetFunc = LinkerFuncs.GetAssetPrefab<T>;
        }

        public static class Table
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AssetBundleRequest> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestTable;
            public static readonly Func<AssetBundleRequest, AssetBundleTable> GetAssetFunc = LinkerFuncs.GetAssetSingle<AssetBundleTable>;
        }

        static AssetBundleRequest GetAssetBundleRequestSingle<T>(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord) where T : UnityEngine.Object
        {
            string assetName = assetEntryRecord.AssetName;
            return assetBundle.LoadAssetAsync<T>(assetName);
        }

        static AssetBundleRequest GetAssetBundleRequestMultiple<T>(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord) where T : UnityEngine.Object
        {
            string assetName = assetEntryRecord.AssetName;
            return assetBundle.LoadAssetWithSubAssetsAsync<T>(assetName);
        }

        static AssetBundleRequest GetAssetBundleRequestTable(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord)
        {
            string assetName = assetBundle.GetAllAssetNames().FirstOrDefault();
            return assetBundle.LoadAssetAsync(assetName);
        }

        static T GetAssetSingle<T>(AssetBundleRequest assetBundleRequest) where T : UnityEngine.Object
        {
            T asset = assetBundleRequest.asset as T;

            if (asset == null && typeof(T).IsSubclassOf(typeof(Component)))
                Debug.LogError($"must use LoadPrefab<T> instead of Load<T>: {typeof(T).Name}");

            return asset;
        }

        static T[] GetAssetMultiple<T>(AssetBundleRequest assetBundleRequest) where T : UnityEngine.Object
        {
            T[] assets = assetBundleRequest.allAssets.Cast<T>().ToArray();
            if (assets.All(asset => asset != null))
                return assets;
            else
                return null;
        }

        static T GetAssetPrefab<T>(AssetBundleRequest assetBundleRequest) where T : Component
        {
            GameObject obj = assetBundleRequest.asset as GameObject;
            T t = obj?.GetComponent<T>();

            if (obj != null && t == null)
                Debug.LogError($"can not get component: {typeof(T).Name}");

            return t;
        }
    }
}
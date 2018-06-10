using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zara.Common.ExAssetBundle.Internal
{
    public static class LinkerFuncs
    {
        public static class Single<T> where T : UnityEngine.Object
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestSingle<T>;
            public static readonly Func<AssetBundleRequest, T> GetAssetFunc = LinkerFuncs.GetAssetSingle<T>;
        }

        public static class Multiple<T> where T : UnityEngine.Object
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestMultiple<T>;
            public static readonly Func<AssetBundleRequest, T[]> GetAssetFunc = LinkerFuncs.GetAssetMultiple<T>;
        }

        public static class Prefab<T> where T : Component
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestSingle<GameObject>;
            public static readonly Func<AssetBundleRequest, T> GetAssetFunc = LinkerFuncs.GetAssetPrefab<T>;
        }

        public static class Table
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestTable;
            public static readonly Func<AssetBundleRequest, AssetBundleTable> GetAssetFunc = LinkerFuncs.GetAssetSingle<AssetBundleTable>;
        }

        public static class Scene
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestScene;
            public static readonly Func<AssetBundleRequest, UnityEngine.Object> GetAssetFunc = LinkerFuncs.GetAssetScene;
        }

        public static class SceneAdditive
        {
            public static readonly Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> GetAssetBundleRequestFunc = LinkerFuncs.GetAssetBundleRequestSceneAdditive;
            public static readonly Func<AssetBundleRequest, UnityEngine.Object> GetAssetFunc = LinkerFuncs.GetAssetScene;
        }

        static AsyncOperationSet GetAssetBundleRequestSingle<T>(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord) where T : UnityEngine.Object
        {
            string assetName = assetEntryRecord.AssetName;
            AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(assetName);
            return AsyncOperationSet.CreateByAssetBundleRequest(request);
        }

        static AsyncOperationSet GetAssetBundleRequestMultiple<T>(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord) where T : UnityEngine.Object
        {
            string assetName = assetEntryRecord.AssetName;
            AssetBundleRequest request = assetBundle.LoadAssetWithSubAssetsAsync<T>(assetName);
            return AsyncOperationSet.CreateByAssetBundleRequest(request);
        }

        static AsyncOperationSet GetAssetBundleRequestTable(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord)
        {
            string assetName = assetBundle.GetAllAssetNames().FirstOrDefault();
            AssetBundleRequest request = assetBundle.LoadAssetAsync(assetName);
            return AsyncOperationSet.CreateByAssetBundleRequest(request);
        }

        static AsyncOperationSet GetAssetBundleRequestScene(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord)
        {
            string sceneName = AssetBundleUtility.GetSceneName(assetEntryRecord.AssetName);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            return AsyncOperationSet.CreateByAsyncOperation(operation);
        }

        static AsyncOperationSet GetAssetBundleRequestSceneAdditive(AssetBundle assetBundle, AssetEntryRecord assetEntryRecord)
        {
            string sceneName = AssetBundleUtility.GetSceneName(assetEntryRecord.AssetName);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            return AsyncOperationSet.CreateByAsyncOperation(operation);
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

        static UnityEngine.Object GetAssetScene(AssetBundleRequest assetBundleRequest)
        {
            return null;
        }
    }

    public struct AsyncOperationSet
    {
        public AssetBundleRequest AssetBundleRequest { get; private set; }
        public AsyncOperation AsyncOperation { get; private set; }

        public static AsyncOperationSet CreateByAssetBundleRequest(AssetBundleRequest request)
        {
            return new AsyncOperationSet(request, request);
        }

        public static AsyncOperationSet CreateByAsyncOperation(AsyncOperation operation)
        {
            return new AsyncOperationSet(null, operation);
        }

        AsyncOperationSet(AssetBundleRequest request, AsyncOperation operation)
        {
            this.AssetBundleRequest = request;
            this.AsyncOperation = operation;
        }
    }
}
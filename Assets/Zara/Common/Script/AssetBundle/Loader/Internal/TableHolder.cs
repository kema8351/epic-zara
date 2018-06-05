using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Internal
{
    public class TableHolder : IAssetEntry
    {
        string tablePath;
        Linker linker;
        NullOnlyCoroutineOwner coroutineOwner;
        IErrorReceiver errorReceiver;

        public AssetBundleTable Table { get; private set; }
        public bool IsTableLoaded => this.Table != null;

        bool isTableLoading = false;

        public TableHolder(string tablePath, Linker linker, NullOnlyCoroutineOwner coroutineOwner, IErrorReceiver errorReceiver)
        {
            this.tablePath = tablePath;
            this.linker = linker;
            this.coroutineOwner = coroutineOwner;
            this.errorReceiver = errorReceiver;
        }

        #region table

        public void LoadTable(bool forciblyDownload)
        {
            if (IsTableLoaded)
            {
                errorReceiver.OnError(AssetBundleErrorCode.LoadedTable, "has already loaded asset bundle table");
                return;
            }

            if (isTableLoading)
            {
                errorReceiver.OnError(AssetBundleErrorCode.LoadingTable, "is loading asset bundle table");
                return;
            }

            coroutineOwner.Run(LoadTableAsync(forciblyDownload));
        }

        IEnumerator LoadTableAsync(bool forciblyDownload)
        {
            isTableLoading = true;

#if UNITY_EDITOR
            if (localMode)
            {
                string localFilePath = GetLocalFilePath(tablePath);
                ResourceRequest resourceRequest = Resources.LoadAsync(localFilePath);
                while (!resourceRequest.isDone)
                    yield return null;

                var localAsset = resourceRequest.asset as UnityEngine.Object;
                if (localAsset != null)
                {
                    while (linker.IsPaused || TimeUtility.DropFrameExists())
                        yield return null;

                    OnTableAssetBundleLoaded(localAsset);
                    yield break;
                }
            }
#endif

            if (forciblyDownload)
            {
                // delete table cache
                string localTablePath = AssetBundleUtility.GetLocalStoragePath(tablePath);

                try
                {
                    if (File.Exists(localTablePath))
                        File.Delete(localTablePath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    errorReceiver.OnError(AssetBundleErrorCode.FailureToDeleteTable, $"cannot delete table: {tablePath}");
                    yield break;
                }
            }

            linker.LoadTableAsset(tablePath, OnTableAssetBundleLoaded);
        }


        void OnTableAssetBundleLoaded(UnityEngine.Object asset)
        {
            this.Table = asset as AssetBundleTable;
            isTableLoading = false;
        }

        #endregion

        #region loading

        public void Load<T>(string assetEntryKey, Action<T> onLoaded) where T : UnityEngine.Object
        {
            coroutineOwner.Run(LoadAsync<T>(assetEntryKey, onLoaded));
        }

        IEnumerator LoadAsync<T>(string assetEntryKey, Action<T> onLoaded) where T : UnityEngine.Object
        {
            while (TimeUtility.DropFrameExists())
                yield return null;

#if UNITY_EDITOR
            if (localMode)
            {
                if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    string message = $"must use LoadPrefab<T> instead of Load<T>: {assetEntryKey}";
                    errorReceiver.OnError(AssetBundleErrorCode.InvalidType, message);
                    yield break;
                }

                string localFilePath = GetLocalFilePath(assetEntryKey);
                ResourceRequest resourceRequest = Resources.LoadAsync<T>(localFilePath);
                while (!resourceRequest.isDone)
                    yield return null;

                var localAsset = resourceRequest.asset as T;
                if (localAsset != null)
                {
                    while (linker.IsPaused || TimeUtility.DropFrameExists())
                        yield return null;

                    onLoaded?.Invoke(localAsset);
                    yield break;
                }
            }
#endif
            string loweredAssetEntryKey = assetEntryKey.ToLower();
            AssetEntryRecord assetEntryRecord = GetAssetEntryRecord(loweredAssetEntryKey, onLoaded);
            if (assetEntryRecord == null)
                yield break;

            linker.LoadAsset<T>(assetEntryRecord, this.Table.GetNecessaryAssetBundleRecords(loweredAssetEntryKey), onLoaded);
        }

        public void LoadMultipleAssets<T>(string assetEntryKey, Action<T[]> onLoaded) where T : UnityEngine.Object
        {
            coroutineOwner.Run(LoadMultipleAssetsAsync<T>(assetEntryKey, onLoaded));
        }

        IEnumerator LoadMultipleAssetsAsync<T>(string assetEntryKey, Action<T[]> onLoaded) where T : UnityEngine.Object
        {
            while (TimeUtility.DropFrameExists())
                yield return null;

#if UNITY_EDITOR
            if (localMode)
            {
                string localFilePath = GetLocalFilePath(assetEntryKey);

                T[] localAsset = Resources.LoadAll<T>(localFilePath);
                if (localAsset != null)
                {
                    while (linker.IsPaused || TimeUtility.DropFrameExists())
                        yield return null;

                    onLoaded?.Invoke(localAsset);
                    yield break;
                }
            }
#endif
            string loweredAssetEntryKey = assetEntryKey.ToLower();
            AssetEntryRecord assetEntryRecord = GetAssetEntryRecord(loweredAssetEntryKey, onLoaded);
            if (assetEntryRecord == null)
                yield break;

            linker.LoadMultipleAssets<T>(assetEntryRecord, this.Table.GetNecessaryAssetBundleRecords(loweredAssetEntryKey), onLoaded);
        }

        public void LoadPrefab<T>(string assetEntryKey, Action<T> onLoaded) where T : Component
        {
            coroutineOwner.Run(LoadPrefabAsync<T>(assetEntryKey, onLoaded));
        }

        IEnumerator LoadPrefabAsync<T>(string assetEntryKey, Action<T> onLoaded) where T : Component
        {
            while (TimeUtility.DropFrameExists())
                yield return null;

#if UNITY_EDITOR
            if (localMode)
            {
                string localFilePath = GetLocalFilePath(assetEntryKey);
                ResourceRequest resourceRequest = Resources.LoadAsync<T>(localFilePath);
                while (!resourceRequest.isDone)
                    yield return null;

                T localAsset = resourceRequest.asset as T;
                if (localAsset != null)
                {
                    while (linker.IsPaused || TimeUtility.DropFrameExists())
                        yield return null;

                    onLoaded?.Invoke(localAsset);
                    yield break;
                }
            }
#endif
            string loweredAssetEntryKey = assetEntryKey.ToLower();
            AssetEntryRecord assetEntryRecord = GetAssetEntryRecord(loweredAssetEntryKey, onLoaded);
            if (assetEntryRecord == null)
                yield break;

            linker.LoadPrefab<T>(assetEntryRecord, this.Table.GetNecessaryAssetBundleRecords(loweredAssetEntryKey), onLoaded);
        }

        public void LoadScene(string assetEntryKey)
        {
            coroutineOwner.Run(LoadSceneAsync(assetEntryKey));
        }

        IEnumerator LoadSceneAsync(string assetEntryKey)
        {
            while (TimeUtility.DropFrameExists())
                yield return null;

#if UNITY_EDITOR
            if (localMode)
            {
                string sceneName = AssetBundleUtility.GetSceneName(assetEntryKey);
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                while (!asyncOperation.isDone)
                    yield return null;
                yield break;
            }
#endif
            string loweredAssetEntryKey = assetEntryKey.ToLower();
            AssetEntryRecord assetEntryRecord = GetAssetEntryRecord<UnityEngine.Object>(loweredAssetEntryKey, null);
            if (assetEntryRecord == null)
                yield break;

            linker.LoadScene(assetEntryRecord, this.Table.GetNecessaryAssetBundleRecords(loweredAssetEntryKey));
        }

        public void LoadSceneAdditive(string assetEntryKey)
        {
            coroutineOwner.Run(LoadSceneAdditiveAsync(assetEntryKey));
        }

        IEnumerator LoadSceneAdditiveAsync(string assetEntryKey)
        {
            while (TimeUtility.DropFrameExists())
                yield return null;

#if UNITY_EDITOR
            if (localMode)
            {
                string sceneName = AssetBundleUtility.GetSceneName(assetEntryKey);
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncOperation.isDone)
                    yield return null;
                yield break;
            }
#endif
            string loweredAssetEntryKey = assetEntryKey.ToLower();
            AssetEntryRecord assetEntryRecord = GetAssetEntryRecord<UnityEngine.Object>(loweredAssetEntryKey, null);
            if (assetEntryRecord == null)
                yield break;

            linker.LoadSceneAdditive(assetEntryRecord, this.Table.GetNecessaryAssetBundleRecords(loweredAssetEntryKey));
        }

        AssetEntryRecord GetAssetEntryRecord<T>(string assetEntryKey, Action<T> onLoaded) where T : class
        {
            if (!IsTableLoaded)
            {
                errorReceiver.OnError(AssetBundleErrorCode.TableNotFound, "has not loaded asset bundle table");
                return null;
            }

            AssetEntryRecord assetEntryRecord = this.Table.AssetEntryRecordMap.Get(assetEntryKey);
            if (assetEntryRecord == null)
            {
                errorReceiver.OnNotFoundError(AssetBundleErrorCode.EntryKeyNotFound, $"cannot find asset entry key: {assetEntryKey}", onLoaded);
                return null;
            }

            return assetEntryRecord;
        }

        #endregion

        #region local mode

#if UNITY_EDITOR
        bool localMode = false;

        string GetLocalFilePath(string assetEntryKey)
        {
            int periodIndex = assetEntryKey.LastIndexOf('.');
            string localFilePath = periodIndex < 0 ? assetEntryKey : assetEntryKey.Substring(0, periodIndex);
            return localFilePath;
        }
#endif
        public void SetLocalMode(bool localMode)
        {
#if UNITY_EDITOR
            this.localMode = localMode;
#endif
        }

        #endregion
    }
}
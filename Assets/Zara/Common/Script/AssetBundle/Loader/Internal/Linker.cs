﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Internal
{
    public class Linker
    {
        // key = assetBundleName
        Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();

        // value = assetBundleName
        Dictionary<string, AssetBundleRecord> loadingAssetBundleRecords = new Dictionary<string, AssetBundleRecord>();

        // key = assetBundleName, item of value = loadingTaskId
        Dictionary<string, HashSet<int>> loadingTaskIdSets = new Dictionary<string, HashSet<int>>();

        // key = loadingTaskId
        Dictionary<int, LoadingTask> loadingTasks = new Dictionary<int, LoadingTask>();

        StorageChecker storageChecker;
        Decryptor decryptor;
        NullOnlyCoroutineOwner coroutineOwner;
        IErrorReceiver errorReceiver;

        IdGenerator loadingTaskIdGenerator = new IdGenerator();
        Action<LoaderResult> onAssetBundleLoaded;

        public Linker(StorageChecker storageChecker, Decryptor decryptor, NullOnlyCoroutineOwner coroutineOwner, IErrorReceiver errorReceiver)
        {
            this.storageChecker = storageChecker;
            this.decryptor = decryptor;
            this.coroutineOwner = coroutineOwner;
            this.errorReceiver = errorReceiver;

            this.onAssetBundleLoaded = OnAssetBundleLoaded;
        }

        public void LoadAsset<T>(
            AssetEntryRecord assetEntryRecord,
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords,
            Action<T> onLoaded) where T : UnityEngine.Object
        {
            AddLodingTask(
                assetEntryRecord,
                necessaryAssetBundleRecords,
                onLoaded,
                LinkerFuncs.Single<T>.GetAssetBundleRequestFunc,
                LinkerFuncs.Single<T>.GetAssetFunc);
        }

        public void LoadMultipleAssets<T>(
            AssetEntryRecord assetEntryRecord,
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords,
            Action<T[]> onLoaded) where T : UnityEngine.Object
        {
            AddLodingTask(
                assetEntryRecord,
                necessaryAssetBundleRecords,
                onLoaded,
                LinkerFuncs.Multiple<T>.GetAssetBundleRequestFunc,
                LinkerFuncs.Multiple<T>.GetAssetFunc);
        }

        public void LoadPrefab<T>(
            AssetEntryRecord assetEntryRecord,
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords,
            Action<T> onLoaded) where T : Component
        {
            AddLodingTask(
                assetEntryRecord,
                necessaryAssetBundleRecords,
                onLoaded,
                LinkerFuncs.Prefab<T>.GetAssetBundleRequestFunc,
                LinkerFuncs.Prefab<T>.GetAssetFunc);
        }


        public void LoadTableAsset(string tablePath, Action<AssetBundleTable> onLoaded)
        {
            AddLodingTask(
                new AssetEntryRecord(tablePath, tablePath, null),
                new List<AssetBundleRecord>() { new AssetBundleRecord(tablePath) },
                onLoaded,
                LinkerFuncs.Table.GetAssetBundleRequestFunc,
                LinkerFuncs.Table.GetAssetFunc);
        }

        public void LoadScene(
            AssetEntryRecord assetEntryRecord,
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords)
        {
            AddLodingTask(
                assetEntryRecord,
                necessaryAssetBundleRecords,
                null,
                LinkerFuncs.Scene.GetAssetBundleRequestFunc,
                LinkerFuncs.Scene.GetAssetFunc);
        }

        public void LoadSceneAdditive(
            AssetEntryRecord assetEntryRecord,
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords)
        {
            AddLodingTask(
                assetEntryRecord,
                necessaryAssetBundleRecords,
                null,
                LinkerFuncs.SceneAdditive.GetAssetBundleRequestFunc,
                LinkerFuncs.SceneAdditive.GetAssetFunc);
        }

        void AddLodingTask<T>(
            AssetEntryRecord assetEntryRecord,
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords,
            Action<T> onLoaded,
            Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> getAssetBundleRequestFunc,
            Func<AssetBundleRequest, T> getAssetFunc)
        {
            if (isDestructed)
                return;

            int loadingTaskId = loadingTaskIdGenerator.Get();
            IEnumerator enumeratorToGetAsset = GetAsset(
                loadingTaskId,
                assetEntryRecord,
                necessaryAssetBundleRecords,
                onLoaded,
                getAssetBundleRequestFunc,
                getAssetFunc);
            var loadingTask = new LoadingTask(necessaryAssetBundleRecords, enumeratorToGetAsset);
            loadingTasks.Add(loadingTaskId, loadingTask);

            foreach (AssetBundleRecord assetBundleRecord in necessaryAssetBundleRecords)
            {
                string assetBundleName = assetBundleRecord.AssetBundleName;
                HashSet<int> taskIds;
                if (!loadingTaskIdSets.TryGetValue(assetBundleName, out taskIds))
                {
                    taskIds = HashSetPool<int>.Pool.Get();
                    loadingTaskIdSets.Add(assetBundleName, taskIds);
                }

                taskIds.Add(loadingTaskId);
            }

            if (loadingTask.NecessaryAssetBundleRecords.All(r => loadedAssetBundles.ContainsKey(r.AssetBundleName)))
            {
                coroutineOwner.Run(loadingTask.EnumeratorToGetAsset);
                return;
            }

            foreach (AssetBundleRecord assetBundleRecord in loadingTask.NecessaryAssetBundleRecords)
            {
                string assetBundleName = assetBundleRecord.AssetBundleName;

                if (loadedAssetBundles.ContainsKey(assetBundleName))
                    continue;

                if (loadingAssetBundleRecords.ContainsKey(assetBundleName))
                    continue;

                loadingAssetBundleRecords.Add(assetBundleName, assetBundleRecord);
                storageChecker.Load(assetBundleRecord, onAssetBundleLoaded);
            }
        }

        void OnAssetBundleLoaded(LoaderResult result)
        {
            coroutineOwner.Run(OnAssetBundleLoadedAsync(result));
        }

        IEnumerator OnAssetBundleLoadedAsync(LoaderResult result)
        {
            string assetBundleName = result.AssetBundleRecord.AssetBundleName;
            AssetBundleRecord assetBundleRecord;
            if (!loadingAssetBundleRecords.TryGetValue(assetBundleName, out assetBundleRecord))
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingLoadingAssetBundleRecord, $"cannot find loading asset bundle record: {assetBundleName}");
                yield break;
            }

            byte[] bytes = Decrypt(result.Bytes);
            if (bytes == null)
            {
                errorReceiver.OnError(AssetBundleErrorCode.FailureToDecrypt, $"cannot decrypt asset bundle: {assetBundleName}");
                yield break;
            }

            uint crc = assetBundleRecord.Crc;
            var assetBundleRequest = AssetBundle.LoadFromMemoryAsync(bytes, crc);
            while (!assetBundleRequest.isDone)
                yield return null;

            AssetBundle assetBundle = assetBundleRequest.assetBundle;
            if (assetBundle == null)
            {
                errorReceiver.OnError(AssetBundleErrorCode.FailureToGetAssetBundle, $"cannot get asset bundle: {assetBundleName}");
                yield break;
            }

            loadedAssetBundles.Add(assetBundleName, assetBundle);

            if (!loadingAssetBundleRecords.TryRemove(assetBundleName))
            {
                errorReceiver.OnError(AssetBundleErrorCode.FailureToRemoveLoadingAssetBundleRecord, $"cannot remove loading asset bundle record: {assetBundleName}");
                yield break;
            }

            HashSet<int> taskIds;
            if (!loadingTaskIdSets.TryGetValue(assetBundleName, out taskIds))
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingTaskIdSetInCheckingCompletion, $"cannot get task id set: {assetBundleName}");
                yield break;
            }

            List<IEnumerator> enumeratorsToGetAsset = ListPool<IEnumerator>.Pool.Get();
            foreach (int taskId in taskIds)
            {
                LoadingTask? nullableLoadingTask = loadingTasks.GetNullableValue(taskId);
                if (!nullableLoadingTask.HasValue)
                {
                    errorReceiver.OnError(AssetBundleErrorCode.MissingLoadingTask, $"cannot get loading task: {assetBundleName}");
                    yield break;
                }

                LoadingTask loadingTask = nullableLoadingTask.Value;
                if (loadingTask.NecessaryAssetBundleRecords.All(r => loadedAssetBundles.ContainsKey(r.AssetBundleName)))
                    enumeratorsToGetAsset.Add(loadingTask.EnumeratorToGetAsset);
            }

            foreach (IEnumerator enumerator in enumeratorsToGetAsset)
                coroutineOwner.Run(enumerator);

            ListPool<IEnumerator>.Pool.Put(ref enumeratorsToGetAsset);
        }

        byte[] Decrypt(byte[] bytes)
        {
            try
            {
                return decryptor.Decrypt(bytes);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        IEnumerator GetAsset<T>(
            int loadingTaskId,
            AssetEntryRecord assetEntryRecord,
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords,
            Action<T> onLoaded,
            Func<AssetBundle, AssetEntryRecord, AsyncOperationSet> getAssetbundleRequestFunc,
            Func<AssetBundleRequest, T> getAssetFunc)
        {
            if (assetEntryRecord == null)
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingAssetEntryRecord, $"null asset entry record");
                yield break;
            }

            AssetBundle assetBundle = loadedAssetBundles.GetValue(assetEntryRecord.AssetBundleName);
            if (assetBundle == null)
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingAssetBundleName, $"cannot get asset bundle: AssetEntryKey={assetEntryRecord.AssetEntryKey}, AssetBundleName={assetEntryRecord.AssetBundleName}");
                yield break;
            }

            AsyncOperationSet asyncOperationSet = getAssetbundleRequestFunc.Invoke(assetBundle, assetEntryRecord);
            AsyncOperation asyncOperation = asyncOperationSet.AsyncOperation;
            while (!asyncOperation.isDone)
                yield return null;

            AssetBundleRequest assetBundleRequest = asyncOperationSet.AssetBundleRequest;
            T asset = getAssetFunc.Invoke(assetBundleRequest);
            if (assetBundleRequest != null && asset == null)
            {
                errorReceiver.OnError(AssetBundleErrorCode.FailureToRemoveTaskId, $"cannot get asset: AssetEntryKey={assetEntryRecord.AssetEntryKey}, AssetBundleName={assetEntryRecord.AssetBundleName}, AssetName={assetEntryRecord.AssetName}");
                yield break;
            }

            while (this.IsPaused || TimeUtility.DropFrameExists())
                yield return null;

            onLoaded?.Invoke(asset);

            FinishLoadingTask(loadingTaskId, necessaryAssetBundleRecords);
        }

        void FinishLoadingTask(int loadingTaskId, IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords)
        {
            foreach (AssetBundleRecord assetBundleRecord in necessaryAssetBundleRecords)
            {
                string assetBundleName = assetBundleRecord.AssetBundleName;

                HashSet<int> taskIds = loadingTaskIdSets.GetValue(assetBundleName);
                if (taskIds == null)
                {
                    errorReceiver.OnError(AssetBundleErrorCode.MissingTaskIdSetInTermination, $"cannot get task id set: {assetBundleName}");
                    return;
                }

                if (!taskIds.TryRemove(loadingTaskId))
                {
                    errorReceiver.OnError(AssetBundleErrorCode.FailureToRemoveTaskId, $"cannot remove task id: {assetBundleName}");
                    return;
                }

                if (taskIds.IsEmpty())
                {
                    if (!loadingTaskIdSets.TryRemove(assetBundleName))
                    {
                        errorReceiver.OnError(AssetBundleErrorCode.FailureToRemoveLoadingTaskIdSet, $"cannot remove loading task id set: {assetBundleName}");
                        return;
                    }
                    HashSetPool<int>.Pool.Put(ref taskIds);

                    AssetBundle unusingAssetBundle = loadedAssetBundles.GetValue(assetBundleName);
                    if (unusingAssetBundle == null)
                    {
                        errorReceiver.OnError(AssetBundleErrorCode.MissingLoadedAssetBundle, $"cannot get loaded asset bundle: {assetBundleName}");
                        return;
                    }

                    unusingAssetBundle.Unload(false);

                    if (!loadedAssetBundles.TryRemove(assetBundleName))
                    {
                        errorReceiver.OnError(AssetBundleErrorCode.FailureToRemoveLoadedAssetBundle, $"cannot remove loaded asset bundle: {assetBundleName}");
                        return;
                    }
                }
            }

            if (!loadingTasks.TryRemove(loadingTaskId))
            {
                errorReceiver.OnError(AssetBundleErrorCode.FailureToRemoveLoadingTask, "cannot remove loading task");
                return;
            }
        }

        #region pause

        public bool IsPaused { get; private set; } = false;

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        #endregion

        #region destroy

        bool isDestructed = false;

        public void Destruct()
        {
            isDestructed = true;
            Pause();

            foreach (var assetBundle in loadedAssetBundles.Values)
                assetBundle.Unload(false);
        }

        #endregion
    }
}
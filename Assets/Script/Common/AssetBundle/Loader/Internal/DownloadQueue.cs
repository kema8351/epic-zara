using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Internal
{
    public class DownloadQueue
    {
        const int MaxDownloadingCount = AssetBundleConst.MaxDownloadingCount;
        const int RetryCount = AssetBundleConst.RetryCount;

        Queue<AssetBundleRecord> assetBundleRecordQueue = new Queue<AssetBundleRecord>();
        Dictionary<string, DownloadTask> taskDictionary = new Dictionary<string, DownloadTask>();
        bool isCheckingQueue = false;
        Action<LoaderResult> downloadFinishedAction;
        Action<LoaderResult> retryActionDueToNetwork;
        Action<LoaderResult> retryActionDueToIo;
        int downloadingCount = 0;

        Downloader downloader;
        NullOnlyCoroutineOwner coroutineOwner;
        IErrorReceiverRetriable errorReceiver;

        public DownloadQueue(
            Downloader downloader,
            NullOnlyCoroutineOwner coroutineOwner,
            IErrorReceiverRetriable errorReceiver)
        {
            this.downloader = downloader;
            this.coroutineOwner = coroutineOwner;
            this.errorReceiver = errorReceiver;
            downloadFinishedAction = OnDownloadFinished;
            retryActionDueToNetwork = RetryDueToNetwork;
            retryActionDueToIo = RetryDueToIo;
        }

        public void EnqueueDownloadTask(AssetBundleRecord assetBundleRecord, Action<LoaderResult> onFinished)
        {
            if (isDestroyed)
                return;

            string assetBundleName = assetBundleRecord.AssetBundleName;
            if (taskDictionary.ContainsKey(assetBundleName))
            {
                errorReceiver.OnError(AssetBundleErrorCode.DuplicatePath, $"duplicate path: {assetBundleName}");
                return;
            }

            assetBundleRecordQueue.Enqueue(assetBundleRecord);
            var task = new DownloadTask(assetBundleRecord, onFinished, 0);
            taskDictionary.Add(assetBundleName, task);

            if (!isCheckingQueue)
            {
                coroutineOwner.Run(CheckQueueAsync());
                isCheckingQueue = true;
            }
        }

        IEnumerator CheckQueueAsync()
        {
            while (taskDictionary.Count > 0)
            {
                while (assetBundleRecordQueue.Count > 0 && downloadingCount < MaxDownloadingCount)
                {
                    AssetBundleRecord record = assetBundleRecordQueue.Dequeue();
                    coroutineOwner.Run(downloader.LoadAsync(record, downloadFinishedAction));
                    downloadingCount++;

                    if (TimeUtility.DropFrameExists())
                        break;
                }

                yield return null;
            }

            isCheckingQueue = false;
        }

        void OnDownloadFinished(LoaderResult result)
        {
            if (isDestroyed)
                return;

            downloadingCount--;

            if (result.ErrorType.HasValue)
            {
                OnErrorReceived(result);
                return;
            }

            string assetBundleName = result.AssetBundleRecord.AssetBundleName;
            DownloadTask task;
            if (!taskDictionary.TryGetValue(assetBundleName, out task))
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingTaskDownloadFinished, $"cannot find task: {assetBundleName}");
                return;
            }

            taskDictionary.Remove(assetBundleName);
            task.OnFinished.Invoke(result);
        }

        void OnErrorReceived(LoaderResult result)
        {
            var errorType = result.ErrorType.Value;
            switch (errorType)
            {
                case AssetBundleDownloadErrorType.Network:
                    Retry(result);
                    break;

                case AssetBundleDownloadErrorType.IO:
                    errorReceiver.OnRetriableError(result, retryActionDueToIo);
                    break;

                default:
                    errorReceiver.OnError(AssetBundleErrorCode.UnknownErrorType, $"unknown download error type: {errorType}");
                    break;
            }
        }

        void Retry(LoaderResult result)
        {
            var assetBundleName = result.AssetBundleRecord.AssetBundleName;
            DownloadTask task;
            if (!taskDictionary.TryGetValue(assetBundleName, out task))
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingTaskRetry, $"cannot find task: {assetBundleName}");
                return;
            }

            if (task.TriedCount + 1 >= RetryCount)
            {
                Debug.LogError($"cannot download asset bundle: {assetBundleName}");
                errorReceiver.OnRetriableError(result, retryActionDueToNetwork);
                return;
            }

            EnqueueTaskToRetry(result, task, task.TriedCount + 1);
        }

        void RetryDueToNetwork(LoaderResult result)
        {
            var assetBundleName = result.AssetBundleRecord.AssetBundleName;
            DownloadTask task;
            if (!taskDictionary.TryGetValue(assetBundleName, out task))
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingTaskRetryNetwork, $"cannot find task: {assetBundleName}");
                return;
            }

            EnqueueTaskToRetry(result, task, 0);
        }

        void RetryDueToIo(LoaderResult result)
        {
            var assetBundleName = result.AssetBundleRecord.AssetBundleName;
            DownloadTask task;
            if (!taskDictionary.TryGetValue(assetBundleName, out task))
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingTaskRetryIo, $"cannot find task: {assetBundleName}");
                return;
            }

            EnqueueTaskToRetry(result, task, task.TriedCount);
        }

        void EnqueueTaskToRetry(LoaderResult result, DownloadTask task, int newTriedCount)
        {
            var assetBundleName = result.AssetBundleRecord.AssetBundleName;

            Debug.LogWarning($"asset bundle download retry: AssetBundleName={assetBundleName} TriedCount={newTriedCount}");

            var newTask = new DownloadTask(result.AssetBundleRecord, task.OnFinished, newTriedCount);
            taskDictionary[assetBundleName] = newTask;
            assetBundleRecordQueue.Enqueue(result.AssetBundleRecord);
        }

        #region destroy

        bool isDestroyed = false;

        public void Destroy()
        {
            isDestroyed = true;
        }

        #endregion
    }
}
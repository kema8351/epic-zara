using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Zara.Common.ExAssetBundle.Internal
{
    public class StorageChecker
    {
        DownloadQueue downloadQueue;
        IErrorReceiver errorReceiver;

        Action<LoaderResult> downloadFinishedAction;
        Dictionary<string, Action<LoaderResult>> actionDictionary = new Dictionary<string, Action<LoaderResult>>();

        public StorageChecker(DownloadQueue downloadQueue, IErrorReceiver errorReceiver)
        {
            this.downloadQueue = downloadQueue;
            this.errorReceiver = errorReceiver;
            this.downloadFinishedAction = OnDownloadFinished;
        }

        public void Load(AssetBundleRecord assetBundleRecord, Action<LoaderResult> onFinished, bool forciblyDownload = false)
        {
            string assetBundleName = assetBundleRecord.AssetBundleName;
            if (!forciblyDownload && TryLoadFromLocalStorage(assetBundleRecord, onFinished))
                return;
            actionDictionary.Add(assetBundleName, onFinished);
            downloadQueue.EnqueueDownloadTask(assetBundleRecord, downloadFinishedAction);
        }

        bool TryLoadFromLocalStorage(AssetBundleRecord assetBundleRecord, Action<LoaderResult> onFinished)
        {
            var localPath = AssetBundleUtility.GetLocalStoragePath(assetBundleRecord.AssetBundleName);
            if (!File.Exists(localPath))
                return false;

            Debug.Log($"load from local storage: {localPath}");

            byte[] bytes;
            try
            {
                using (FileStream fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
                {
                    bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            var result = new LoaderResult(assetBundleRecord, bytes, null);
            onFinished.Invoke(result);
            return true;
        }

        void OnDownloadFinished(LoaderResult result)
        {
            var assetBundleName = result.AssetBundleRecord.AssetBundleName;
            Action<LoaderResult> onFinished;
            if (!actionDictionary.TryGetValue(assetBundleName, out onFinished))
            {
                errorReceiver.OnError(AssetBundleErrorCode.MissingFinishedAction, $"cannot find finished action: {assetBundleName}");
                return;
            }

            // release load async coroutine
            actionDictionary.Remove(assetBundleName);
            onFinished.Invoke(result);
        }
    }
}
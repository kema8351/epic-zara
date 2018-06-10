using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zara.Common.ExAssetBundle.Internal;

namespace Zara.Common.ExAssetBundle
{
    public class AssetBundlePreloader
    {
        DownloadQueue downloadQueue;
        AssetBundleRecord[] assetBundleRecordsToDownload;
        bool isPreloadingStarted = false;
        Action<LoaderResult> onDownloaded;

        public string LatestDownloadedAssetBundleName { get; private set; } = "";
        public int MaxAssetBundleCount { get; private set; } = 0;
        public int DownloadedAssetBundleCount { get; private set; } = 0;
        public long MaxBytes { get; private set; } = 0;
        public long DownloadedBytes { get; private set; } = 0;
        public bool IsFinished => (MaxAssetBundleCount <= DownloadedAssetBundleCount);

        public AssetBundlePreloader(
            DownloadQueue downloadQueue,
            IEnumerable<AssetBundleRecord> assetBundleRecords)
        {
            this.onDownloaded = OnDownloaded;

            this.downloadQueue = downloadQueue;
            this.assetBundleRecordsToDownload = assetBundleRecords
                .Where((AssetBundleRecord assetBundleRecord) =>
                {
                    string localStoragePath = AssetBundleUtility.GetLocalStoragePath(assetBundleRecord.AssetBundleName);
                    return !File.Exists(localStoragePath);
                })
                .ToArray();

            foreach (AssetBundleRecord assetBundleRecord in this.assetBundleRecordsToDownload)
            {
                string localStoragePath = AssetBundleUtility.GetLocalStoragePath(assetBundleRecord.AssetBundleName);
                if (File.Exists(localStoragePath))
                    continue;

                this.MaxAssetBundleCount++;
                this.MaxBytes += assetBundleRecord.FileSizeBytes;
            }
        }

        public void StartPreloading()
        {
            if (isPreloadingStarted)
            {
                Debug.LogError("has started preloading");
                return;
            }
            isPreloadingStarted = true;

            foreach (AssetBundleRecord assetBundleRecord in this.assetBundleRecordsToDownload)
            {
                downloadQueue.EnqueueDownloadTask(assetBundleRecord, onDownloaded);
            }
        }

        void OnDownloaded(LoaderResult result)
        {
            this.DownloadedAssetBundleCount++;
            this.DownloadedBytes += result.AssetBundleRecord.FileSizeBytes;
            this.LatestDownloadedAssetBundleName = result.AssetBundleRecord.AssetBundleName;
        }
    }
}
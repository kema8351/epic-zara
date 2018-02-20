using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.ExAssetBundle.Internal;
using Zara.Common.ExBase;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle
{
    public class AssetBundleDirector : IErrorReceiverRetriable
    {
        DownloadQueue downloadQueue;
        Linker linker;
        NullOnlyCoroutineOwner coroutineOwner;
        IAssetBundleErrorHandler errorHandler;
        bool isDestroyed = false;

#if UNITY_EDITOR
        public bool LocalMode { get; private set; } = false;
        public void SetLocalMode(bool localMode)
        {
            this.LocalMode = localMode;
        }
#endif

        public AssetBundleDirector(string url, MonoBehaviour coroutineStarter, IAssetBundleErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;

            coroutineOwner = new NullOnlyCoroutineOwner(coroutineStarter);
            var downloader = new Downloader(url);
            downloadQueue = new DownloadQueue(downloader, coroutineOwner, this);
            var storageChecker = new StorageChecker(downloadQueue, this);
            var encryptionKey = new EncryptionKey();
            var decryptor = new Decryptor(encryptionKey);
            linker = new Linker(storageChecker, decryptor, coroutineOwner, this);
        }

        public IAssetEntry LoadTable(string tablePath, bool forciblyDownload)
        {
            if (isDestroyed)
            {
                string message = "asset bundle director has been destroyed";
                (this as IErrorReceiver).OnError(AssetBundleErrorCode.DestroyedInLoadingTable, message);
                return null;
            }

            var entry = new TableHolder(tablePath, linker, coroutineOwner, this);
#if UNITY_EDITOR
            entry.SetLocalMode(this.LocalMode);
#endif
            entry.LoadTable(forciblyDownload);

            return entry;
        }

        public AssetBundlePreloader Preload(IAssetEntry assetEntry)
        {
            if (assetEntry == null)
            {
                string message = "asset entry is null";
                (this as IErrorReceiver).OnError(AssetBundleErrorCode.NullAssetEntry, message);
                return null;
            }

            if (!assetEntry.IsTableLoaded)
            {
                string message = "wait for loading table";
                (this as IErrorReceiver).OnError(AssetBundleErrorCode.NullAssetBundleTable, message);
                return null;
            }

            return Preload(assetEntry.Table.AssetBundleRecordMap.Values);
        }

        public AssetBundlePreloader Preload(IEnumerable<AssetBundleRecord> assetBundleRecords)
        {
            if (isDestroyed)
            {
                string message = "asset bundle director has been destroyed";
                (this as IErrorReceiver).OnError(AssetBundleErrorCode.DestroyedInPreloading, message);
                return null;
            }

            return new AssetBundlePreloader(downloadQueue, assetBundleRecords);
        }

        public AssetBundleSweeper ClearStorge()
        {
            return ClearStorage(Enumerable.Empty<string>());
        }

        public AssetBundleSweeper ClearStorage(params IAssetEntry[] protectingAssetEntries)
        {
            return ClearStorage(protectingAssetEntries
                .SelectMany((IAssetEntry entry) => entry.Table.AssetBundleRecordMap
                    .Values
                    .Select((AssetBundleRecord record) => record.AssetBundleName)));
        }

        public AssetBundleSweeper ClearStorage(IEnumerable<string> protectingAssetBundleNames)
        {
            if (isDestroyed)
            {
                string message = "asset bundle director has been destroyed";
                (this as IErrorReceiver).OnError(AssetBundleErrorCode.DestroyedInClearance, message);
                return null;
            }

            return new AssetBundleSweeper(protectingAssetBundleNames, coroutineOwner);
        }

        public void Destroy()
        {
            if (isDestroyed)
                return;
            isDestroyed = true;

            downloadQueue?.Destroy();
            linker?.Destroy();
            coroutineOwner?.Destroy();

            downloadQueue = null;
            linker = null;
            coroutineOwner = null;
            errorHandler = null;
        }

        #region interface

        void IErrorReceiver.OnError(AssetBundleErrorCode errorCode, string message)
        {
            OnError(errorCode, message);
        }

        void IErrorReceiver.OnNotFoundError<T>(AssetBundleErrorCode errorCode, string message, Action<T> onLoaded)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
            onLoaded.Invoke(null);
#else
            OnError(errorCode, message);
#endif
        }

        void OnError(AssetBundleErrorCode errorCode, string message)
        {
            Debug.LogError(message);

            if (errorHandler == null)
            {
                Debug.LogError("has not set asset bundle error handler");
                return;
            }

            errorHandler.OnError(errorCode, message);
        }

        void IErrorReceiverRetriable.OnRetriableError(LoaderResult result, Action<LoaderResult> retryAction)
        {
            linker.Pause();

            if (errorHandler == null)
            {
                Debug.LogError("has not set asset bundle error handler");
                return;
            }

            errorHandler.OnRetriableError(
                result.ErrorType ?? AssetBundleDownloadErrorType.Unknown,
                result.AssetBundleRecord?.AssetBundleName,
                () =>
                {
                    linker.Resume();
                    retryAction.Invoke(result);
                });
        }

        #endregion
    }
}
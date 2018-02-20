using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.ExAssetBundle.Internal
{
    public struct LoaderResult
    {
        public AssetBundleRecord AssetBundleRecord { get; }
        public byte[] Bytes { get; }
        public AssetBundleDownloadErrorType? ErrorType { get; }

        public LoaderResult(AssetBundleRecord assetBundleRecord, byte[] bytes, AssetBundleDownloadErrorType? errorType = null)
        {
            this.AssetBundleRecord = assetBundleRecord;
            this.Bytes = bytes;
            this.ErrorType = errorType;
        }
    }
}
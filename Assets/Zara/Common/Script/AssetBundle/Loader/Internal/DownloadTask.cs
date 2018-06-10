using System;

namespace Zara.Common.ExAssetBundle.Internal
{
    public struct DownloadTask
    {
        public AssetBundleRecord AssetBundleRecord { get; }
        public Action<LoaderResult> OnFinished { get; }
        public int TriedCount { get; }

        public DownloadTask(AssetBundleRecord assetBundleRecord, Action<LoaderResult> onFinished, int triedCount)
        {
            this.AssetBundleRecord = assetBundleRecord;
            this.OnFinished = onFinished;
            this.TriedCount = triedCount;
        }
    }
}
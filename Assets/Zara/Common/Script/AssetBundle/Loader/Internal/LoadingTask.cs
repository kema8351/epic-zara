using System.Collections;
using System.Collections.Generic;

namespace Zara.Common.ExAssetBundle.Internal
{
    public struct LoadingTask
    {
        public IReadOnlyList<AssetBundleRecord> NecessaryAssetBundleRecords { get; }
        public IEnumerator EnumeratorToGetAsset { get; }

        public LoadingTask(
            IReadOnlyList<AssetBundleRecord> necessaryAssetBundleRecords,
            IEnumerator enumeratorToGetAsset)
        {
            this.NecessaryAssetBundleRecords = necessaryAssetBundleRecords;
            this.EnumeratorToGetAsset = enumeratorToGetAsset;
        }
    }
}
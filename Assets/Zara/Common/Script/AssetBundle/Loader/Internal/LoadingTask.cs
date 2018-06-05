using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Internal
{
    public struct LoadingTask
    {
        public FixedList<AssetBundleRecord> NecessaryAssetBundleRecords { get; }
        public IEnumerator EnumeratorToGetAsset { get; }

        public LoadingTask(
            FixedList<AssetBundleRecord> necessaryAssetBundleRecords,
            IEnumerator enumeratorToGetAsset)
        {
            this.NecessaryAssetBundleRecords = necessaryAssetBundleRecords;
            this.EnumeratorToGetAsset = enumeratorToGetAsset;
        }
    }
}
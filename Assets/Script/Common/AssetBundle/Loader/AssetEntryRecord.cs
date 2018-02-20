using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zara.Common.ExAssetBundle
{
    [Serializable]
    public class AssetEntryRecord
    {
        public AssetEntryRecord(string assetEntryKey, string assetBundleName, string assetName)
        {
            this.assetEntryKey = assetEntryKey;
            this.assetBundleName = assetBundleName;
            this.assetName = assetName;
        }

        [SerializeField]
        string assetEntryKey;
        public string AssetEntryKey => assetEntryKey;

        [SerializeField]
        string assetBundleName;
        public string AssetBundleName => assetBundleName;

        [SerializeField]
        string assetName;
        public string AssetName => assetName;
    }
}
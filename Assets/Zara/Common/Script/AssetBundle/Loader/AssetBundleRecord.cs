using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zara.Common.ExAssetBundle
{
    [Serializable]
    public class AssetBundleRecord
    {
        public AssetBundleRecord(string assetBundleName)
        {
            // to download asset bundle table
            this.assetBundleName = assetBundleName;
            this.dependencies = Enumerable.Empty<string>().ToArray();
            this.crc = 0;
            this.fileSizeBytes = 0L;
        }

        public AssetBundleRecord(string assetBundleName, IEnumerable<string> dependencies, uint crc, long fileSizeBytes)
        {
            this.assetBundleName = assetBundleName;
            this.dependencies = dependencies.ToArray();
            this.crc = crc;
            this.fileSizeBytes = fileSizeBytes;
        }

        [SerializeField]
        string assetBundleName;
        public string AssetBundleName => assetBundleName;

        [SerializeField]
        string[] dependencies;
        public IEnumerable<string> DependencyNames => dependencies;

        [SerializeField]
        uint crc;
        public uint Crc => crc;

        [SerializeField]
        long fileSizeBytes;
        public long FileSizeBytes => fileSizeBytes;
    }
}
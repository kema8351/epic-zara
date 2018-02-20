using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle
{
    public class AssetBundleTable : ScriptableObject, IAssetBundleTableInjector
    {
        [SerializeField]
        long version;
        public long Version => version;

        [SerializeField]
        string platform;
        public string Platform => platform;

        [SerializeField]
        AssetBundleRecord[] assetBundleRecords;

        [SerializeField]
        AssetEntryRecord[] assetEntryRecords;

        FixedMap<string, AssetBundleRecord> _assetBundleRecordMap;
        public FixedMap<string, AssetBundleRecord> AssetBundleRecordMap
        {
            get
            {
                if (_assetBundleRecordMap == null)
                    _assetBundleRecordMap = new FixedMap<string, AssetBundleRecord>(assetBundleRecords, r => r.AssetBundleName);
                return _assetBundleRecordMap;
            }
        }

        FixedMap<string, AssetEntryRecord> _assetEntryRecordMap;
        public FixedMap<string, AssetEntryRecord> AssetEntryRecordMap
        {
            get
            {
                if (_assetEntryRecordMap == null)
                    _assetEntryRecordMap = new FixedMap<string, AssetEntryRecord>(assetEntryRecords, r => r.AssetEntryKey);
                return _assetEntryRecordMap;
            }
        }

        Dictionary<string, FixedList<AssetBundleRecord>> necessaryAssetBundleRecordsDictionary = new Dictionary<string, FixedList<AssetBundleRecord>>();
        HashSet<string> temporaryHashSet = new HashSet<string>();

        public FixedList<AssetBundleRecord> GetNecessaryAssetBundleRecords(string assetEntryKey)
        {
            FixedList<AssetBundleRecord> cache;
            if (necessaryAssetBundleRecordsDictionary.TryGetValue(assetEntryKey, out cache))
                return cache;

            AssetEntryRecord assetEntryRecord = this.AssetEntryRecordMap.Get(assetEntryKey);
            var result = GetNecessaryAssetBundleRecords(assetEntryRecord);
            necessaryAssetBundleRecordsDictionary.Add(assetEntryKey, result);

            return result;
        }

        FixedList<AssetBundleRecord> GetNecessaryAssetBundleRecords(AssetEntryRecord assetEntryRecord)
        {
            var necessaryAssetBundleRecords = new List<AssetBundleRecord>();
            if (assetEntryRecord == null)
                return new FixedList<AssetBundleRecord>(necessaryAssetBundleRecords);

            string assetBundleName = assetEntryRecord.AssetBundleName;
            AssetBundleRecord assetBundleRecord = this.AssetBundleRecordMap.Get(assetBundleName);
            if (assetBundleRecord == null)
                return new FixedList<AssetBundleRecord>(necessaryAssetBundleRecords);

            temporaryHashSet.Add(assetBundleName);
            GetNecessaryAssetBundleRecords(
                assetBundleName,
                ref temporaryHashSet,
                ref necessaryAssetBundleRecords);

            temporaryHashSet.Clear();
            return new FixedList<AssetBundleRecord>(necessaryAssetBundleRecords);
        }

        void GetNecessaryAssetBundleRecords(
            string assetBundleName,
            ref HashSet<string> checkedAssetBundleNames,
            ref List<AssetBundleRecord> checkedAssetBundleRecords)
        {
            checkedAssetBundleNames.Add(assetBundleName);
            AssetBundleRecord assetBundleRecord = this.AssetBundleRecordMap.Get(assetBundleName);
            if (assetBundleRecord == null)
                return;
            checkedAssetBundleRecords.Add(assetBundleRecord);


            foreach (var dependency in assetBundleRecord.DependencyNames)
            {
                if (checkedAssetBundleNames.Contains(dependency))
                    continue;

                GetNecessaryAssetBundleRecords(
                    dependency,
                    ref checkedAssetBundleNames,
                    ref checkedAssetBundleRecords);
            }
        }

        #region create

        public static AssetBundleTable Create(
            long version,
            string platform,
            IEnumerable<AssetBundleRecord> assetBundleRecords,
            IEnumerable<AssetEntryRecord> assetRecords)
        {
            var table = CreateInstance<AssetBundleTable>();
            var injector = table as IAssetBundleTableInjector;
            injector.Inject(version, platform, assetBundleRecords, assetRecords);
            return table;
        }

        void IAssetBundleTableInjector.Inject(
            long version,
            string platform,
            IEnumerable<AssetBundleRecord> assetBundleRecords,
            IEnumerable<AssetEntryRecord> assetRecords)
        {
            this.version = version;
            this.platform = platform;
            this.assetBundleRecords = assetBundleRecords.ToArray();
            this.assetEntryRecords = assetRecords.ToArray();
        }

        #endregion
    }

    // to hide function for injection
    public interface IAssetBundleTableInjector
    {
        void Inject(
            long version,
            string platform,
            IEnumerable<AssetBundleRecord> assetBundleRecords,
            IEnumerable<AssetEntryRecord> assetRecords);
    }
}
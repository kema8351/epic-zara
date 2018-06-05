using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class AssetEntryRecordFilter
    {
        public string TableFilePath { get; }
        Func<List<AssetEntryRecordCandidate>, AssetEntryRecord> filterFunc;

        public AssetEntryRecordFilter(string tableFilePath, Func<List<AssetEntryRecordCandidate>, AssetEntryRecord> filterFunc)
        {
            this.TableFilePath = tableFilePath;
            this.filterFunc = filterFunc;
        }

        public AssetEntryRecord Filterate(List<AssetEntryRecordCandidate> candidates)
        {
            return filterFunc.Invoke(candidates);
        }
    }
}
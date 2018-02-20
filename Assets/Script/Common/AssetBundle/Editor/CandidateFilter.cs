using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class CandidateFilter
    {
        public string FilePathPostfix { get; }
        public Func<List<AssetEntryRecordCandidate>, List<AssetEntryRecordCandidate>> FilterFunc { get; }

        public CandidateFilter(
            string filePathPostfix,
            Func<List<AssetEntryRecordCandidate>, List<AssetEntryRecordCandidate>> filterFunc)
        {
            this.FilePathPostfix = filePathPostfix;
            this.FilterFunc = filterFunc;
        }

    }
}
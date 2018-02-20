using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public abstract class TagConditionSet
    {
        public int Id { get; }
        public abstract IEnumerable<string> UsingTags { get; }
        public abstract IEnumerable<CandidateFilter> CandidateFilters { get; }

        public TagConditionSet(int id)
        {
            this.Id = id;
        }
    }
}
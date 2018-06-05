using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class BranchTagConditionSet : TagConditionSet
    {
        BranchTagCondition[] conditionArray;
        string[] usingTags;
        CandidateFilter[] candidateFilters;
        public override IEnumerable<string> UsingTags => usingTags;
        public override IEnumerable<CandidateFilter> CandidateFilters => candidateFilters;

        public BranchTagConditionSet(int id, IEnumerable<BranchTagCondition> conditions) : base(id)
        {
            this.conditionArray = conditions.ToArray();
            this.usingTags = conditions.Select(c => c.Tag).Where(tag => !tag.IsNullOrEmpty()).ToArray();
            this.candidateFilters = this.conditionArray.Select(condition => new CandidateFilter(
                condition.PartFileName,
                (List<AssetEntryRecordCandidate> candidates) =>
                {
                    if (candidates.All(c => !HasAnyTags(c)))
                        return candidates.ToList();

                    return candidates.Where(candidate => HasTag(candidate, condition)).ToList();
                })).ToArray();
        }

        bool HasAnyTags(AssetEntryRecordCandidate candidate)
        {
            FixedHashSet<string> candidateTags = candidate.TagsDictionary.GetValue(this.Id);
            if (candidateTags.IsNull)
                return false;

            return candidateTags.Count > 0;
        }

        bool HasTag(AssetEntryRecordCandidate candidate, BranchTagCondition condition)
        {
            FixedHashSet<string> candidateTags = candidate.TagsDictionary.GetValue(this.Id);
            if (candidateTags.IsNull)
                return false;

            if (condition.Tag.IsNullOrEmpty())
                return candidateTags.Count <= 0;

            return candidateTags.Contains(condition.Tag);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class StepTagConditionSet : TagConditionSet
    {
        Dictionary<string, StepTagCondition> conditionDictionary;
        string[] usingTags;
        CandidateFilter[] candidateFilters;
        public override IEnumerable<string> UsingTags => usingTags;
        public override IEnumerable<CandidateFilter> CandidateFilters => candidateFilters;

        public StepTagConditionSet(int id, IEnumerable<StepTagCondition> conditions) : base(id)
        {
            this.conditionDictionary = conditions.ToDictionary(c => c.Tag, c => c);
            this.usingTags = conditions.Select(c => c.Tag).Where(tag => !tag.IsNullOrEmpty()).ToArray();
            this.candidateFilters = this.conditionDictionary.Values.Select(
                (StepTagCondition condition) => new CandidateFilter(
                    condition.PartFileName,
                    (List<AssetEntryRecordCandidate> candidates) =>
                    {
                        if (condition.Tag.IsNullOrEmpty())
                            return candidates.ToList();

                        int thresholdStep = condition.Step;
                        return candidates.Where(candidate => thresholdStep >= GetStep(candidate)).ToList();
                    })).ToArray();
        }

        int GetStep(AssetEntryRecordCandidate candidate)
        {
            int step = int.MaxValue / 2;

            FixedHashSet<string> candidateTags = candidate.TagsDictionary.GetValue(this.Id);
            if (candidateTags.IsNull)
                return step;

            foreach (var tag in candidateTags)
            {
                StepTagCondition condition = conditionDictionary.GetValue(tag);
                if (condition != null)
                    step = Mathf.Min(step, condition.Step);
            }

            return step;
        }
    }
}
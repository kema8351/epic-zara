using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class AssetBundleTableCondition
    {
        public string TableFilePathBase { get; }
        public Func<string, bool> GetNecessityFunc { get; }
        public IEnumerable<TagConditionSet> TagConditionSets => tagConditionSets;
        TagConditionSet[] tagConditionSets;

        public AssetBundleTableCondition(
            string tableFilePathBase,
            Func<string, bool> getNecessityFunc,
            IEnumerable<TagConditionSet> tagConditionSets)
        {
            this.TableFilePathBase = tableFilePathBase;
            this.GetNecessityFunc = getNecessityFunc;
            this.tagConditionSets = tagConditionSets.ToArray();
        }

        public IEnumerable<AssetEntryRecordFilter> EnumerateFilters()
        {
            return EnumerateFiltersRecursively(
                string.Empty,
                new Stack<Func<List<AssetEntryRecordCandidate>, List<AssetEntryRecordCandidate>>>(),
                0);
        }

        IEnumerable<AssetEntryRecordFilter> EnumerateFiltersRecursively(
            string tableFilePathPostfix,
            Stack<Func<List<AssetEntryRecordCandidate>, List<AssetEntryRecordCandidate>>> filterFuncs,
            int tabConditionSetIndex)
        {
            if (tabConditionSetIndex >= tagConditionSets.Length)
            {
                yield return new AssetEntryRecordFilter(
                    $"{this.TableFilePathBase}{tableFilePathPostfix}",
                    GetFilterFunc(filterFuncs));
            }
            else
            {
                TagConditionSet tabConditionSet = tagConditionSets[tabConditionSetIndex];
                foreach (CandidateFilter candidateFilter in tabConditionSet.CandidateFilters)
                {
                    filterFuncs.Push(candidateFilter.FilterFunc);
                    IEnumerable<AssetEntryRecordFilter> filters = EnumerateFiltersRecursively(
                        $"{tableFilePathPostfix}{candidateFilter.FilePathPostfix}",
                        filterFuncs,
                        tabConditionSetIndex + 1);
                    foreach (AssetEntryRecordFilter filter in filters)
                        yield return filter;
                    filterFuncs.Pop();
                }

            }
        }

        Func<List<AssetEntryRecordCandidate>, AssetEntryRecord> GetFilterFunc(Stack<Func<List<AssetEntryRecordCandidate>, List<AssetEntryRecordCandidate>>> filterFuncs)
        {
            Func<List<AssetEntryRecordCandidate>, List<AssetEntryRecordCandidate>>[] filterFuncArray = filterFuncs.ToArray();

            return (List<AssetEntryRecordCandidate> candidates) =>
            {
                List<AssetEntryRecordCandidate> result = candidates;
                foreach (var filterFunc in filterFuncArray)
                    result = filterFunc.Invoke(result);

                if (result.Count >= 2)
                    Debug.LogError($"cannot narrow down to one record: {result.FirstOrDefault().AssetEntryRecord.AssetEntryKey}");

                return result.FirstOrDefault()?.AssetEntryRecord;
            };
        }
    }
}
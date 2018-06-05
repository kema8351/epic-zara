using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class AssetEntryRecordCandidate
    {
        public AssetEntryRecord AssetEntryRecord { get; }
        FixedHashSet<string> tags;
        public Dictionary<int, FixedHashSet<string>> TagsDictionary { get; private set; } = new Dictionary<int, FixedHashSet<string>>();

        static readonly string ResourcesKeyWord = AssetBundleEditor.ResourcesKeyWord.ToLower();

        public static AssetEntryRecordCandidate Create(string assetPath, string assetBundleName)
        {
            string loweredAssetPath = assetPath.ToLower();
            // enumerate files in "resources" folder only
            int keyWordPosition = loweredAssetPath.IndexOf(ResourcesKeyWord);
            if (keyWordPosition < 0)
                return null;

            string assetEntryKey = RemoveTags(loweredAssetPath.Substring(keyWordPosition + ResourcesKeyWord.Length));

            var assetEntryRecord = new AssetEntryRecord(assetEntryKey, assetBundleName, assetPath);
            return new AssetEntryRecordCandidate(assetEntryRecord, GetTags(assetPath));
        }

        static string RemoveTags(string path)
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool isInTag = false;

            foreach (char c in path)
            {
                if (isInTag)
                {
                    if (c == '[')
                        Debug.LogError($"unexpected opening blacket: {path}");
                    else if (c == ']')
                        isInTag = false;
                }
                else
                {
                    if (c == '[')
                        isInTag = true;
                    else if (c == ']')
                        Debug.LogError($"unexpected closing blacket: {path}");
                    else
                        stringBuilder.Append(c);

                }
            }

            return stringBuilder.ToString();
        }

        static IEnumerable<string> GetTags(string path)
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool isInTag = false;

            foreach (char c in path)
            {
                if (isInTag)
                {
                    if (c == '[')
                    {
                        Debug.LogError($"unexpected opening blacket: {path}");
                    }
                    else if (c == ']')
                    {
                        yield return stringBuilder.ToString();
                        stringBuilder.Clear();
                        isInTag = false;
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
                else
                {
                    if (c == '[')
                        isInTag = true;
                    else if (c == ']')
                        Debug.LogError($"unexpected closing blacket: {path}");

                }
            }
        }

        public AssetEntryRecordCandidate(AssetEntryRecord assetEntryRecord, IEnumerable<string> tags)
        {
            this.AssetEntryRecord = assetEntryRecord;

            this.tags = new FixedHashSet<string>(tags.Distinct());
        }

        public void PrepareTagSets(AssetBundleTableCondition tableCondition)
        {
            this.TagsDictionary.Clear();

            foreach (TagConditionSet tabConditionSet in tableCondition.TagConditionSets)
            {
                IEnumerable<string> containedTags = tabConditionSet.UsingTags.Where(t => tags.Contains(t));

                this.TagsDictionary.Add(
                    tabConditionSet.Id,
                    new FixedHashSet<string>(containedTags));
            }
        }
    }
}
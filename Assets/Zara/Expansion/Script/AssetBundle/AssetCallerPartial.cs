using UnityEngine;
using Zara.Common.ExBase;

namespace Zara.Expansion.ExAssetBundle
{
    public partial class AssetCaller : ExMonoBehaviour
    {
        string GetStepPart(AssetStep step)
        {
            switch (step)
            {
                case AssetStep.First: return "[first]";
                case AssetStep.Title: return "[title]";
                case AssetStep.All: return "";
                default:
                    Debug.LogError($"unknown asset step: {step}");
                    return "";
            }
        }

        string GetLanguagePart(AssetLanguage language)
        {
            switch (language)
            {
                case AssetLanguage.Japanese: return "[jp]";
                case AssetLanguage.English: return "[en]";
                default:
                    Debug.LogError($"unknown asset language: {language}");
                    return "";
            }
        }
    }

    public enum AssetStep
    {
        First,
        Title,
        All,
    }

    public enum AssetLanguage
    {
        Japanese,
        English,
    }
}

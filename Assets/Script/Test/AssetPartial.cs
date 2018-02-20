







using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExBase;

namespace Zara.Test
{
    public partial class Asset : ExMonoBehaviour
    {
        string GetStepPart(AssetStep step)
        {
            switch (step)
            {
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
                case AssetLanguage.English: return "[en]";
                
                case AssetLanguage.Japanese: return "[jp]";
                
                default:
                    Debug.LogError($"unknown asset language: {language}");
                    return "";
            }
        }
    }

    public enum AssetStep
    {
        Title,
        
        All,
        
    }

    public enum AssetLanguage
    {
        English,
        
        Japanese,
        
    }
}

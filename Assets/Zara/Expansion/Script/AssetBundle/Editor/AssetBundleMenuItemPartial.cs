







using UnityEditor;
using Zara.Common.ExAssetBundle.Editor;

namespace Zara.Expansion.ExAssetBundle.Editor
{
    public partial class AssetBundleMenuItem
    {
        
        [MenuItem(MenuDirPath + "Build/Android")]
        static void BuildForAndroid()
        {
            Build(BuildTarget.Android);
        }
        
        
        [MenuItem(MenuDirPath + "Build/iOS")]
        static void BuildForiOS()
        {
            Build(BuildTarget.iOS);
        }
        
        
        [MenuItem(MenuDirPath + "Build/WebGL")]
        static void BuildForWebGL()
        {
            Build(BuildTarget.WebGL);
        }
        

        static TagConditionSet CreateLanguageTagConditionSet()
        {
            return new BranchTagConditionSet(
                10,
                new BranchTagCondition[]
                {
                    new BranchTagCondition("", "[jp]"),
                    new BranchTagCondition("en", "[en]"),

                });
        }

        static TagConditionSet CreateStepTagConditionSet()
        {
            return new StepTagConditionSet(
                20,
                new StepTagCondition[]
                {
                    new StepTagCondition("first", "[first]", 1),
                    new StepTagCondition("title", "[title]", 2),
                    new StepTagCondition("", "", 9),

                });
        }
    }
}

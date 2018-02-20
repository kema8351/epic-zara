







using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Zara.Common.ExAssetBundle;
using System;
using Zara.Common.ExAssetBundle.Editor;

namespace Zara.Test.Editor
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
					new BranchTagCondition("", "[en]"),
					
					new BranchTagCondition("jp", "[jp]"),
					
				});
		}

		static TagConditionSet CreateStepTagConditionSet()
		{
			return new StepTagConditionSet(
				20,
				new StepTagCondition[]
				{
					new StepTagCondition("title", "[title]", 5),
					
					new StepTagCondition("", "", 10),
					
				});
		}
	}
}

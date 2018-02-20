using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Zara.Common.Utility;
using Zara.Common.ExAssetBundle.Internal;

namespace Zara.Common.ExAssetBundle
{
    public class AssetBundleSweeper
    {
        NullOnlyCoroutineOwner coroutineOwner;
        HashSet<string> protectingFilePaths;
        public bool IsFinished { get; private set; } = false;

        public AssetBundleSweeper(
            IEnumerable<string> protectingAssetBundleNames,
            NullOnlyCoroutineOwner coroutineOwner)
        {
            this.coroutineOwner = coroutineOwner;
            this.protectingFilePaths = protectingAssetBundleNames
                .Select(assetBundleName => AssetBundleUtility.GetLocalStoragePath(assetBundleName))
                .ToHashSet();
        }

        public void StartClearance()
        {
            coroutineOwner.Run(ClearAsync());
        }

        IEnumerator ClearAsync()
        {
            foreach (string filePath in Directory.GetFiles(AssetBundleUtility.GetLocalStoragePath(string.Empty), "*", SearchOption.AllDirectories))
            {
                if (protectingFilePaths.Contains(filePath))
                    continue;

                try
                {
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                if (TimeUtility.DropFrameExists())
                    yield return null;
            }

            this.IsFinished = true;
        }
    }
}
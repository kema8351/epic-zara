using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class AssetBundleNamer
    {
        public delegate string HookDelegate(string rootDirtPath, string defaultAssetBundleName, bool isInResources, AssetImporter importer);

        HookDelegate hookAction;

        static readonly HashSet<string> ExtensionsToIgnore = new HashSet<string>()
        {
            ".meta",
            ".cs",
        };

        const string ResourcesKeyWord = AssetBundleEditor.ResourcesKeyWord;

        public AssetBundleNamer(HookDelegate hookAction)
        {
            this.hookAction = hookAction;
        }

        public void SetAllAssetsName(string rootDirPath)
        {
            string[] filePaths = Directory.GetFiles(rootDirPath, "*", SearchOption.AllDirectories);
            SetAssetNameToAssets(rootDirPath, filePaths);
        }

        public void SetAssetNameToSelectingObjects(string rootDirPath)
        {
            Object[] selectingObjs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            if (selectingObjs == null || selectingObjs.Length == 0)
            {
                Debug.LogWarning("no selecting");
                return;
            }

            foreach (var obj in selectingObjs)
                SetAssetNameToObject(rootDirPath, obj);

            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        void SetAssetNameToObject(string rootDirPath, Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            SetAssetNameToAsset(rootDirPath, path);
        }

        public void SetAssetNameToAssets(string rootDirPath, params string[] assetPathArray)
        {
            for (int i = 0; i < assetPathArray.Length; i++)
            {
                string assetPath = assetPathArray[i];
                float progress = (float)i / (float)assetPathArray.Length;
                EditorUtility.DisplayProgressBar(
                    string.Format("{0} ({1}/{2})", this.GetType().Name, i, assetPathArray.Length),
                    assetPath,
                    progress);

                SetAssetNameToAsset(rootDirPath, assetPath);
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            EditorUtility.ClearProgressBar();
        }

        void SetAssetNameToAsset(string rootDirPath, string argAssetPath)
        {
            string assetPath = argAssetPath.Replace('\\', '/');

            string extension = Path.GetExtension(assetPath);
            if (ExtensionsToIgnore.Contains(extension))
                return;

            bool isInResources = false;
            string assetBundleName = GetAssetName(rootDirPath, assetPath, out isInResources);

            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                Debug.LogError("cannot get asset importer: " + assetPath);
                return;
            }

            importer.assetBundleName =
                hookAction != null ?
                hookAction.Invoke(rootDirPath, assetBundleName, isInResources, importer) :
                assetBundleName;
            importer.assetBundleVariant = "";
        }

        string GetAssetName(string rootDirPath, string assetPath, out bool isInResources)
        {
            int keyWordPosition = assetPath.IndexOf(ResourcesKeyWord);
            isInResources = (keyWordPosition >= 0);

            string assetBundleName =
                isInResources ?
                assetPath.Substring(keyWordPosition + ResourcesKeyWord.Length) :
                assetPath.Substring(rootDirPath.Length);

            return assetBundleName.Replace(" ", "");
        }
    }
}

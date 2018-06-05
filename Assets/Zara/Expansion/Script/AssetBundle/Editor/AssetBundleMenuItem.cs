using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Zara.Common.ExAssetBundle;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Zara.Common.ExAssetBundle.Editor;
using Zara.Common.ExAssetBundle.Internal;

namespace Zara.Expansion.ExAssetBundle.Editor
{
    public partial class AssetBundleMenuItem
    {
        const string MenuDirPath = "ExMenu/AssetBundle/";
        const string RootAssetDirPath = "Assets/Cabinet/AssetBundle/";
        const string AssetBundleTableDirPath = "Assets/Cabinet/AssetBundleTable/";
        const string OutputPlaneAssetBundleDirPath = "AssetBundle/Plane/";
        const string OutputCryptedAssetBundleDirPath = "AssetBundle/Crypted/";
        const string AssetBundleTableFileNameBase = "table/resource";

        [MenuItem(MenuDirPath + "SetNames/All")]
        static void SetNamesAll()
        {
            var namer = AssetBundleEditor.GetAssetBundleNamerWithPacking();
            namer.SetAllAssetsName(RootAssetDirPath);
        }

        [MenuItem(MenuDirPath + "SetNames/Selecting")]
        static void SetNamesSelecting()
        {
            var namer = AssetBundleEditor.GetAssetBundleNamerWithPacking();
            namer.SetAssetNameToSelectingObjects(RootAssetDirPath);
        }

        static void Build(BuildTarget buildTarget)
        {
            AssetBundleEditor.BuildAssetBundles(OutputPlaneAssetBundleDirPath, buildTarget);

            EncryptionKey encryptionKey = new EncryptionKey();
            AssetBundleEncryptor encryptor = new AssetBundleEncryptor(
                AssetBundleEditor.GetAssetBundleDir(OutputPlaneAssetBundleDirPath, buildTarget),
                AssetBundleEditor.GetAssetBundleDir(OutputCryptedAssetBundleDirPath, buildTarget),
                encryptionKey);

            AssetBundleTableCondition tableCondition = CreateResourceTableCondition();

            AssetBundleEditor.CreateAndBuildAssetBundleTable(
                encryptor,
                AssetBundleTableDirPath,
                buildTarget,
                tableCondition);
        }

        static AssetBundleTableCondition CreateResourceTableCondition()
        {
            return new AssetBundleTableCondition(
                AssetBundleTableFileNameBase,
                (string assetEntryKey) => !IsMasterAsset(assetEntryKey),
                new TagConditionSet[]
                {
                    CreateLanguageTagConditionSet(),
                    CreateStepTagConditionSet(),
                });
        }

        static bool IsMasterAsset(string assetEntryKey)
        {
            return false;
        }
    }
}
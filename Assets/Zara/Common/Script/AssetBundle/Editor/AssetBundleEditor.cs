using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

namespace Zara.Common.ExAssetBundle.Editor
{
    public static class AssetBundleEditor
    {
        public const string ResourcesKeyWord = "/Resources/";

        #region namer

        public static AssetBundleNamer GetAssetBundleNamerWithPacking()
        {
            return new AssetBundleNamer(new AssetBundleNamer.HookDelegate(Hook));
        }

        const char PackingMarkChar = '_';

        static string Hook(string rootPath, string defaultAssetBundleName, bool isInResources, AssetImporter argImporter)
        {
            if (isInResources)
                return defaultAssetBundleName;

            TextureImporter importer = argImporter as TextureImporter;
            if (importer == null)
                return defaultAssetBundleName;

            string fullDir = Path.GetDirectoryName(defaultAssetBundleName);
            string dir = Path.GetFileName(fullDir);
            bool shouldPack = (dir.Length > 0 && dir[0] == PackingMarkChar);

            if (!shouldPack)
                return defaultAssetBundleName;

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePackingTag = fullDir;

            return fullDir;
        }

        #endregion

        #region builder

        public static void BuildAssetBundles(string argOutputPath, BuildTarget buildTarget, bool mustBuildAll = false)
        {
            string outputPath = GetAssetBundleDir(argOutputPath, buildTarget);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            BuildAssetBundleOptions baseOptions =
                BuildAssetBundleOptions.ChunkBasedCompression |
                BuildAssetBundleOptions.AppendHashToAssetBundleName;

            BuildAssetBundleOptions options =
                mustBuildAll ?
                baseOptions | BuildAssetBundleOptions.ForceRebuildAssetBundle :
                baseOptions;

            BuildPipeline.BuildAssetBundles(
                outputPath,
                options,
                buildTarget);

            Debug.Log("Finish building asset bundles");
        }

        public static string GetAssetBundleDir(string outputPath, BuildTarget buildTarget)
        {
            return $"{outputPath}{buildTarget.ToString()}/";
        }

        #endregion

        #region table creator

        public static void CreateAndBuildAssetBundleTable(AssetBundleEncryptor encryptor, string assetBundleTableDirPath, BuildTarget buildTarget, AssetBundleTableCondition tableCondition)
        {
            string tableDirPath = GetAssetBundleDir(assetBundleTableDirPath, buildTarget);

            string assetBundleDirName = encryptor.PlaneAssetBundleDirPath;
            string manifestPath = BackUpManifest(assetBundleDirName, buildTarget);

            List<string> tableFilePaths = AssetBundleTableCreator.CreateTables(
                encryptor,
                tableDirPath,
                manifestPath,
                buildTarget,
                tableCondition);

            AssetBundleEditor.BuildAssetBundleTable(encryptor, tableDirPath, tableFilePaths, buildTarget);

            Debug.Log($"Finish building asset bundle table");
        }

        static string BackUpManifest(string assetBundleDirName, BuildTarget buildTarget)
        {
            var manifestPath = $"{assetBundleDirName}/{buildTarget.ToString()}";
            var manifestPathWithExtension = $"{manifestPath}.manifest";

            BackUp(manifestPath);
            BackUp(manifestPathWithExtension);

            return manifestPath;
        }

        static void BackUp(string sourcePath)
        {
            var destinationPath = $"{Path.GetDirectoryName(sourcePath)}/_{Path.GetFileName(sourcePath)}";

            if (File.Exists(sourcePath))
                File.Copy(sourcePath, destinationPath, true);
            else
                Debug.LogError($"file does not exist: {sourcePath}");
        }

        static void BuildAssetBundleTable(AssetBundleEncryptor encryptor, string tableDirPath, List<string> tableFilePaths, BuildTarget buildTarget)
        {
            AssetBundleBuild[] buildArray = tableFilePaths
                .Select(tableFilePath =>
                {
                    string assetBundleName = GetTableAssetBundleName(tableDirPath, tableFilePath);
                    var tableBuild = new AssetBundleBuild()
                    {
                        assetBundleName = assetBundleName,
                        assetNames = new string[] { tableFilePath },
                    };
                    return tableBuild;

                })
                .ToArray();

            BuildPipeline.BuildAssetBundles(
                encryptor.PlaneAssetBundleDirPath,
                buildArray,
                BuildAssetBundleOptions.ChunkBasedCompression,
                buildTarget);

            foreach (string tableFilePath in tableFilePaths)
            {
                string assetBundleName = GetTableAssetBundleName(tableDirPath, tableFilePath);
                encryptor.Encrypt(assetBundleName, true);
            }
        }

        static string GetTableAssetBundleName(string rootDirPath, string tableFilePath)
        {
            int index = tableFilePath.IndexOf(rootDirPath);
            bool canGetRelativePath = (index == 0);
            if (!canGetRelativePath)
                Debug.LogError($"cannot get relative path: {rootDirPath}, {tableFilePath}");

            return canGetRelativePath ? tableFilePath.Substring(rootDirPath.Length) : tableFilePath;
        }

        #endregion
    }
}
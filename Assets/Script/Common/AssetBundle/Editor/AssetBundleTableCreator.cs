using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class AssetBundleTableCreator
    {
        // return vale = table file paths
        public static List<string> CreateTables(
            AssetBundleEncryptor encryptor,
            string tableDirPath,
            string manifestPath,
            BuildTarget buildTarget,
            AssetBundleTableCondition tableCondition)
        {
            List<string> tableFilePaths = new List<string>();

            AssetBundle assetBundle = AssetBundle.LoadFromFile(manifestPath);
            try
            {
                AssetBundleManifest manifest = (AssetBundleManifest)assetBundle.LoadAsset("AssetBundleManifest", typeof(AssetBundleManifest));

                string planeAssetBundleDirPath = encryptor.PlaneAssetBundleDirPath;
                var assetEntryRecordCandidateLists = manifest.GetAllAssetBundles()
                    .SelectMany(assetBundleName => EnumerateAssetEntryRecords(planeAssetBundleDirPath, assetBundleName))
                    .GroupBy(r => r.AssetEntryRecord.AssetEntryKey)
                    .ToDictionary(g => g.Key, g => g.ToList());

                IEnumerable<string> createdTableFilePaths = CreateTableByTableCondition(
                    encryptor,
                    tableDirPath,
                    manifest,
                    buildTarget,
                    tableCondition,
                    assetEntryRecordCandidateLists);

                tableFilePaths.AddRange(createdTableFilePaths);

                AssetDatabase.SaveAssets();
            }
            finally
            {
                assetBundle.Unload(true);
            }

            return tableFilePaths;
        }

        static IEnumerable<AssetEntryRecordCandidate> EnumerateAssetEntryRecords(string rootDirPath, string assetBundleName)
        {
            string path = $"{rootDirPath}{assetBundleName}";

            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            try
            {
                foreach (var assetPath in assetBundle.GetAllAssetNames())
                {
                    AssetEntryRecordCandidate record = AssetEntryRecordCandidate.Create(assetPath, assetBundleName);
                    if (record != null)
                        yield return record;
                }
            }
            finally
            {
                assetBundle.Unload(true);
            }
        }

        static IEnumerable<string> CreateTableByTableCondition(
            AssetBundleEncryptor encryptor,
            string tableDirPath,
            AssetBundleManifest manifest,
            BuildTarget buildTarget,
            AssetBundleTableCondition tableCondition,
            Dictionary<string, List<AssetEntryRecordCandidate>> assetEntryRecordCandidateLists)
        {
            foreach (AssetEntryRecordCandidate candidate in assetEntryRecordCandidateLists.Values.SelectMany(c => c))
            {
                candidate.PrepareTagSets(tableCondition);
            }

            foreach (AssetEntryRecordFilter filter in tableCondition.EnumerateFilters())
            {
                string tableFilePath = $"{tableDirPath}{filter.TableFilePath}.asset";
                IEnumerable<AssetEntryRecord> assetEntryRecords = assetEntryRecordCandidateLists
                    .Where(pair => tableCondition.GetNecessityFunc.Invoke(pair.Key))
                    .Select(pair => pair.Value)
                    .Select(candidates => filter.Filterate(candidates))
                    .Where(record => record != null);

                CreateTableByAssetEntryRecords(
                    encryptor,
                    manifest,
                    tableFilePath,
                    buildTarget,
                    assetEntryRecords);

                yield return tableFilePath;
            }
        }

        static void CreateTableByAssetEntryRecords(
            AssetBundleEncryptor encryptor,
            AssetBundleManifest manifest,
            string tableFilePath,
            BuildTarget buildTarget,
            IEnumerable<AssetEntryRecord> assetEntryRecords)
        {
            string planeAssetBundleDirPath = encryptor.PlaneAssetBundleDirPath;

            HashSet<string> checkedAssetBundleNames = new HashSet<string>(
                assetEntryRecords
                    .Select(assetEntryRecord => assetEntryRecord.AssetBundleName)
                    .Distinct());
            Queue<string> checkingAssetBundleNameQueue = new Queue<string>(checkedAssetBundleNames);

            while (!checkingAssetBundleNameQueue.IsEmpty())
            {
                string assetBundleName = checkingAssetBundleNameQueue.Dequeue();

                foreach (string dependency in manifest.GetDirectDependencies(assetBundleName))
                {
                    if (checkedAssetBundleNames.Contains(dependency))
                        continue;

                    checkedAssetBundleNames.Add(dependency);
                    checkingAssetBundleNameQueue.Enqueue(dependency);
                }
            }

            IEnumerable<AssetBundleRecord> assetBundleRecords = checkedAssetBundleNames
                .Select(assetBundleName =>
                {
                    string crcFilePath = GetCrcFilePath(planeAssetBundleDirPath, assetBundleName);
                    uint crc = 0;
                    var isSucceeded = BuildPipeline.GetCRCForAssetBundle(crcFilePath, out crc);
                    if (!isSucceeded)
                        Debug.LogError($"cannot get crc: AssetBundelName = {assetBundleName}");

                    string encryptedAssetBundleFilePath = encryptor.Encrypt(assetBundleName, true);
                    //string encryptedAssetBundleFilePath = encryptor.Encrypt(assetBundleName, false);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(encryptedAssetBundleFilePath);
                    long fileSizeBytes = fileInfo.Length;

                    return new AssetBundleRecord(
                        assetBundleName,
                        manifest.GetDirectDependencies(assetBundleName),
                        crc,
                        fileSizeBytes);
                });

            DateTime current = DateTime.Now;
            long version = long.Parse(current.ToString("yyyyMMddHHmmss"));
            string platform = buildTarget.ToString();

            var table = AssetBundleTable.Create(version, platform, assetBundleRecords, assetEntryRecords);

            string tableDirName = Path.GetDirectoryName(tableFilePath);
            if (!Directory.Exists(tableDirName))
                Directory.CreateDirectory(tableDirName);

            //var json = JsonUtility.ToJson(table);
            //using (StreamWriter writer = new StreamWriter(outputPath + ".json"))
            //{
            //    writer.Write(json);
            //}

            AssetDatabase.CreateAsset(table, tableFilePath);
        }

        static string GetCrcFilePath(string rootDirPath, string assetBundleName)
        {
            // crc file = manifest file

            string path = $"{rootDirPath}{assetBundleName}";
            string result = path;

            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            try
            {
                result = $"{rootDirPath}{assetBundle.name}";
            }
            finally
            {
                assetBundle.Unload(true);
            }

            return result;
        }
    }
}
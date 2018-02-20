using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Zara.Common.ExAssetBundle.Internal
{
    public class Downloader
    {
        string url;

        public Downloader(string url)
        {
            this.url = AssetBundleUtility.CheckEndSlash(url);
        }

        public IEnumerator LoadAsync(AssetBundleRecord assetBundleRecord, Action<LoaderResult> onFinished)
        {
            string path = $"{url}{assetBundleRecord.AssetBundleName}";
            Debug.Log($"download from web: {path}");

            var request = UnityWebRequest.Get(path);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SendWebRequest();
            yield return null;
            while (!request.isDone)
                yield return null;

            if (request.isHttpError || request.isNetworkError)
            {
                var errorResult = new LoaderResult(assetBundleRecord, null, AssetBundleDownloadErrorType.Network);
                onFinished.Invoke(errorResult);
                yield break;
            }

            byte[] bytes = request.downloadHandler.data;
            var localPath = AssetBundleUtility.GetLocalStoragePath(assetBundleRecord.AssetBundleName);
            try
            {
                var dirPath = Path.GetDirectoryName(localPath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                using (FileStream fileStream = new FileStream(localPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                var errorResult = new LoaderResult(assetBundleRecord, bytes, AssetBundleDownloadErrorType.IO);
                onFinished.Invoke(errorResult);
                yield break;
            }

            var successResult = new LoaderResult(assetBundleRecord, bytes, null);
            onFinished.Invoke(successResult);
        }
    }
}
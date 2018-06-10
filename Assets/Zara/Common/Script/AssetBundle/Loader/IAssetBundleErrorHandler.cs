using System;

namespace Zara.Common.ExAssetBundle
{
    public interface IAssetBundleErrorHandler
    {
        void OnError(AssetBundleErrorCode errorCode, string message);
        void OnRetriableError(AssetBundleDownloadErrorType type, string assetBundleName, Action retryAction);
    }
}
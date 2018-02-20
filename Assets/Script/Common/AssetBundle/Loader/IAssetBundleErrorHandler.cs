using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExAssetBundle.Internal;

namespace Zara.Common.ExAssetBundle
{
    public interface IAssetBundleErrorHandler
    {
        void OnError(AssetBundleErrorCode errorCode, string message);
        void OnRetriableError(AssetBundleDownloadErrorType type, string assetBundleName, Action retryAction);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.ExAssetBundle.Internal
{
    public interface IErrorReceiver
    {
        void OnError(AssetBundleErrorCode errorCode, string message);
        void OnNotFoundError<T>(AssetBundleErrorCode errorCode, string message, Action<T> onLoaded) where T : class;
    }

    public interface IErrorReceiverRetriable : IErrorReceiver
    {
        void OnRetriableError(LoaderResult result, Action<LoaderResult> retryFunc);
    }
}
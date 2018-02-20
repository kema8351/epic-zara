using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExBase;
using Zara.Common.Utility;

namespace Zara.Test
{
    public partial class Asset : ExMonoBehaviour, IAssetBundleErrorHandler
    {
        AssetBundleDirector director;

        public AssetStep Step { get; private set; } = AssetStep.All;
        public AssetLanguage Languate { get; private set; } = AssetLanguage.English;
        public IAssetEntry Resource { get; private set; }

#if UNITY_EDITOR
        [SerializeField]
        bool localMode = false;
#endif

        public void Init()
        {
            director = new AssetBundleDirector(@"http://localhost/AssetBundle/Android/", this, this);
#if UNITY_EDITOR
            director.SetLocalMode(localMode);
#endif
        }

        public void SetResrouceTable(AssetStep? step = null, AssetLanguage? language = null)
        {
            if (this.Resource?.IsTableLoaded == false)
            {
                Debug.LogError("wait for loading table");
                return;
            }

            if (step.HasValue)
                this.Step = step.Value;

            if (language.HasValue)
                this.Languate = language.Value;

            string resourceTablePath = $"{Const.Asset.ResrouceTablePathHead}{GetLanguagePart(this.Languate)}{GetStepPart(this.Step)}{Const.Asset.ResrouceTablePathTail}";

            this.Resource = director.LoadTable(resourceTablePath, true);
        }

        public AssetBundlePreloader GetResourcePreloader()
        {
            return director.Preload(this.Resource);
        }

        public AssetBundleSweeper GetSweeper()
        {
            return director.ClearStorage();
        }

        #region IAssetBundleErrorHandler

        const string MessagePrefabPath = "Message";

        Message messageForCriticalError = null;

        void IAssetBundleErrorHandler.OnError(AssetBundleErrorCode errorCode, string message)
        {
            if (messageForCriticalError != null)
                return;

            var prefab = Resources.Load<Message>(MessagePrefabPath);
            messageForCriticalError = Instantiate<Message>(prefab);
            messageForCriticalError.Init(
                $"Code: {errorCode}\nmessage",
                () =>
                {
                    Destroy(messageForCriticalError.gameObject);
                    messageForCriticalError = null;
                    Debug.Log("finish");

                    Application.Quit();
                },
                null);
        }

        Message messageForRetriableError = null;
        Queue<Action> retryActionQueue = new Queue<Action>();

        void IAssetBundleErrorHandler.OnRetriableError(AssetBundleDownloadErrorType type, string assetBundleName, Action retryAction)
        {
            retryActionQueue.Enqueue(retryAction);

            if (messageForRetriableError != null)
                return;

            var prefab = Resources.Load<Message>(MessagePrefabPath);
            messageForRetriableError = Instantiate<Message>(prefab);
            messageForRetriableError.Init(
                $"Asset Bundle Download Error: {type}\n{assetBundleName}",
                () =>
                {
                    Debug.Log("start to retry");

                    while (!retryActionQueue.IsEmpty())
                        retryActionQueue.Dequeue()?.Invoke();

                    Destroy(messageForRetriableError.gameObject);
                    messageForRetriableError = null;
                },
                () =>
                {
                    Destroy(messageForRetriableError.gameObject);
                    messageForRetriableError = null;
                    Debug.Log("finish");

                    Application.Quit();
                });
        }

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExBase;
using Zara.Common.Utility;
using Zara.Main;
using Zara.Main.Ui.Error;

namespace Zara.Expansion.ExAssetBundle
{
    public partial class AssetCaller : ExMonoBehaviour, IAssetBundleErrorHandler
    {
        AssetBundleDirector director;
        Coroutine loadTableCoroutine;
        ErrorSceneCaller errorSceneCaller;

        public AssetStep Step { get; private set; } = AssetStep.All;
        public AssetLanguage Languate { get; private set; } = AssetLanguage.English;
        public IAssetEntry Resource { get; private set; }
        bool IsTableLoading => loadTableCoroutine != null;

#if UNITY_EDITOR
        [SerializeField]
        bool localMode = false;
#endif

        public void Init(string assetRootUri, ErrorSceneCaller errorSceneCaller)
        {
            if (director != null)
                return;

            this.errorSceneCaller = errorSceneCaller;

            director = new AssetBundleDirector(assetRootUri, this, this);
#if UNITY_EDITOR
            director.SetLocalMode(localMode);
#endif
        }

        public void Destruct()
        {
            if (director == null)
                return;

            director.Destruct();
            director = null;
        }

        public Coroutine SetResrouceTable(AssetStep? step = null, AssetLanguage? language = null)
        {
            if (this.IsTableLoading)
            {
                Debug.LogError("loading other asset bundle table");
                return loadTableCoroutine;
            }

            loadTableCoroutine = StartCoroutine(SetResrouceTableAsync(step, language));
            return loadTableCoroutine;
        }

        IEnumerator SetResrouceTableAsync(AssetStep? step = null, AssetLanguage? language = null)
        {
            if (step.HasValue)
                this.Step = step.Value;

            if (language.HasValue)
                this.Languate = language.Value;

            string resourceTablePath = $"{ConstAssetBundle.ResrouceTablePathHead}{GetLanguagePart(this.Languate)}{GetStepPart(this.Step)}{ConstAssetBundle.ResrouceTablePathTail}";

            bool forciblyDownload =
#if UNITY_EDITOR
                !localMode;
#else
                false;
#endif
            IAssetEntry newAssetEntry = director.LoadTable(resourceTablePath, forciblyDownload);

            while (!newAssetEntry.IsTableLoaded)
                yield return null;

            this.Resource = newAssetEntry;
            loadTableCoroutine = null;
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

        void IAssetBundleErrorHandler.OnError(AssetBundleErrorCode errorCode, string message)
        {
            string errorMessage = $"Code: {errorCode}\nmessage";
            errorSceneCaller.NotifyError(errorMessage);
        }

        void IAssetBundleErrorHandler.OnRetriableError(AssetBundleDownloadErrorType type, string assetBundleName, Action retryAction)
        {
            string message = $"Asset Bundle Download Error: {type}\n{assetBundleName}";
            errorSceneCaller.AskRetry(message, retryAction);
        }

        #endregion
    }
}
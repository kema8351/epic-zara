using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExScene;
using Zara.Expansion.Ui;
using Zara.Main.Ui.Title;

namespace Zara.Main.Ui.Download
{
    public class DownloadUi : CoreUi
    {
        [SerializeField] Text downloadCountText;

        AssetBundlePreloader assetBundlePreloader;
        Action<OverlayUiOperator> onDownloaded;

        public void Init(AssetBundlePreloader assetBundlePreloader, Action<OverlayUiOperator> onDownloaded)
        {
            this.assetBundlePreloader = assetBundlePreloader;
            this.onDownloaded = onDownloaded;
        }

        protected override IEnumerator OnUiStarted() { yield break; }

        protected override IEnumerator OnUiEntrying()
        {
            StartCoroutine(UpdateDisplayAsync());
            yield break;
        }

        IEnumerator UpdateDisplayAsync()
        {
            while (!assetBundlePreloader.IsFinished)
            {
                UpdateDownloadCount();
                yield return null;
            }

            UpdateDownloadCount();

            while (!IsShown)
                yield return null;

            OverlayUiOperator loadingUiOperator = Game.Scene.ShowLoading();
            onDownloaded.Invoke(loadingUiOperator);
        }

        void UpdateDownloadCount()
        {
            downloadCountText.text = $"{assetBundlePreloader.DownloadedAssetBundleCount}/{assetBundlePreloader.MaxAssetBundleCount}";
        }

        protected override IEnumerator OnUiEntried()
        {
            yield break;
        }

        protected override IEnumerator OnUiExiting() { yield break; }
        protected override IEnumerator OnUiExited() { yield break; }
    }
}
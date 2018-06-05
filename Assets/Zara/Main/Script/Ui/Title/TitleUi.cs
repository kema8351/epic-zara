using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExScene;
using Zara.Expansion.ExAssetBundle;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Title
{
    public class TitleUi : CoreUi
    {
        [SerializeField] Button startButton;
        [SerializeField] Button clearCacheButton;
        [SerializeField] Image image;

        protected override IEnumerator OnUiStarted()
        {
            image.transform.localPosition = new Vector2(UnityEngine.Random.Range(-800f, 800f), UnityEngine.Random.Range(-600f, 600f));
            image.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            startButton.onClick.AddListener(OnStartButtonClicked);
            clearCacheButton.onClick.AddListener(OnClearCacheButtonClicked);
            yield break;
        }

        protected override IEnumerator OnUiEntrying() { yield break; }
        protected override IEnumerator OnUiEntried() { yield break; }
        protected override IEnumerator OnUiExiting() { yield break; }
        protected override IEnumerator OnUiExited() { yield break; }

        void OnStartButtonClicked()
        {
            StartCoroutine(OnStartButtonClickedAsync());
        }

        IEnumerator OnStartButtonClickedAsync()
        {
            OverlayUiOperator loadingUiOperator = Game.Scene.ShowLoading();
            yield return Game.AssetCaller.SetResrouceTable(AssetStep.All, AssetLanguage.Japanese);
            AssetBundlePreloader preloader = Game.AssetCaller.GetResourcePreloader();
            preloader.StartPreloading();

            if (preloader.MaxAssetBundleCount > 0)
            {
                Game.Scene.LoadDownload(
                    ss => ss.Ui.Init(preloader, LoadFirstScene),
                    loadingUiOperator);
            }
            else
            {
                LoadFirstScene(loadingUiOperator);
            }
        }

        void LoadFirstScene(OverlayUiOperator loadingUiOperator)
        {
            Game.Scene.LoadFirst(overlayUiOperator: loadingUiOperator);
        }

        void OnClearCacheButtonClicked()
        {
            AssetBundleSweeper sweeper = Game.AssetCaller.GetSweeper();
            sweeper.StartClearance();
        }
    }

    public interface IDownloadAssetBundleActionCaller
    {
        void OnAssetBundleDownloaded();
    }
}
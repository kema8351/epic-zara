using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExBase;

namespace Zara.Test
{
    public class Root : ExMonoBehaviour
    {
        [SerializeField]
        Asset asset = null;

        IEnumerator Start()
        {
            asset.Init();
            yield return PreloadAsync(AssetStep.Title, AssetLanguage.English);

            bool isTitleFinished = false;
            AssetLanguage language = default(AssetLanguage);
            asset.Resource.LoadPrefab("Title.prefab", (Title prefab) =>
            {
                var title = Instantiate<Title>(prefab);
                title.Init(
                    (AssetLanguage l) =>
                    {
                        language = l;
                        isTitleFinished = true;
                        Destroy(title.gameObject);
                    },
                    () => StartCoroutine(ClearAsync()));
            });
            while (!isTitleFinished)
                yield return null;

            yield return PreloadAsync(AssetStep.All, language);

            asset.Resource.LoadPrefab("End.prefab", (End prefab) =>
            {
                var end = Instantiate<End>(prefab);
                end.Init(asset);
            });
        }

        IEnumerator PreloadAsync(AssetStep step, AssetLanguage language)
        {
            asset.SetResrouceTable(step, language);
            while (!asset.Resource.IsTableLoaded)
                yield return null;

            bool isPreloaded = false;
            asset.Resource.LoadPrefab("Loading.prefab", (Loading prefab) =>
            {
                var loading = Instantiate<Loading>(prefab);
                AssetBundlePreloader preloader = asset.GetResourcePreloader();
                preloader.StartPreloading();
                loading.Init(preloader, () =>
                {
                    Destroy(loading.gameObject);
                    isPreloaded = true;
                });
            });
            while (!isPreloaded)
                yield return null;
        }

        IEnumerator ClearAsync()
        {
            AssetBundleSweeper sweeper = asset.GetSweeper();
            sweeper.StartClearance();
            while (!sweeper.IsFinished)
                yield return null;
            Debug.Log("clear storage");
        }
    }
}
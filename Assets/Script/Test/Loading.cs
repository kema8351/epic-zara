using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExAssetBundle;
using Zara.Common.ExBase;

namespace Zara.Test
{
    public class Loading : ExMonoBehaviour
    {
        [SerializeField]
        Text assetBundleCountText = null;

        [SerializeField]
        Text byteCountText = null;

        [SerializeField]
        Text progressText = null;

        [SerializeField]
        Text latestDownloadedAssetBundleNameText = null;

        AssetBundlePreloader preloader;

        public void Init(AssetBundlePreloader preloader, Action onFinished)
        {
            this.preloader = preloader;
            StartCoroutine(CountUp(onFinished));
        }

        IEnumerator CountUp(Action onFinished)
        {
            while (true)
            {
                assetBundleCountText.text = $"{preloader.DownloadedAssetBundleCount}/{preloader.MaxAssetBundleCount}";
                byteCountText.text = $"{preloader.DownloadedBytes}bytes/{preloader.MaxBytes}bytes";
                float progressPercents =
                    preloader.MaxBytes <= 0 ?
                    100f :
                    (float)preloader.DownloadedBytes / (float)preloader.MaxBytes * 100f;
                progressText.text = $"{progressPercents:n0}%";
                latestDownloadedAssetBundleNameText.text = preloader.LatestDownloadedAssetBundleName;

                if (preloader.IsFinished)
                    break;

                yield return null;
            }

            yield return null;

            onFinished.Invoke();
        }
    }
}
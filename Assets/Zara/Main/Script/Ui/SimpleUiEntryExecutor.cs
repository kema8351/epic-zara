using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Common.Ui;
using Zara.Common.Utility;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui
{
    public class SimpleUiEntryExecutor : UiEntryExecutor
    {
        [AutoAdd]
        [SerializeField]
        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        public override IEnumerator Entry(bool shouldShowImmediately)
        {
            canvasGroup.blocksRaycasts = false;

            if (shouldShowImmediately)
                SkipToEntried();

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += 2f * Time.deltaTime;
                yield return null;
            }

            canvasGroup.blocksRaycasts = true;
        }

        public override void SkipToEntried()
        {
            canvasGroup.alpha = 1f;
        }
    }
}
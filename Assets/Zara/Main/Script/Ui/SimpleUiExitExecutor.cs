using System.Collections;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui
{
    public class SimpleUiExitExecutor : UiExitExecutor
    {
        [AutoAdd]
        [SerializeField]
        CanvasGroup canvasGroup;

        public override IEnumerator Exit(bool shouldHideImmediately)
        {
            canvasGroup.blocksRaycasts = false;

            if (shouldHideImmediately)
                SkipToHidden();

            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= 4f * Time.deltaTime;
                yield return null;
            }
        }

        public override void SkipToHidden()
        {
            canvasGroup.alpha = 0f;
        }
    }
}
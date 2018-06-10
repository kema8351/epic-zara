using System.Collections;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui
{
    public class ZeroUiEntryExecutor : UiEntryExecutor
    {
        [AutoAdd]
        [SerializeField]
        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup.alpha = 0f;
        }

        public override IEnumerator Entry(bool shouldHideImmediately)
        {
            canvasGroup.alpha = 1f;
            yield break;
        }

        public override void SkipToEntried()
        {
        }
    }
}
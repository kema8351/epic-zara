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
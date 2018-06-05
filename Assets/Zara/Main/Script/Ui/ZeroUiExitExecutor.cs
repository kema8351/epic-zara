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
    public class ZeroUiExitExecutor : UiExitExecutor
    {
        public override IEnumerator Exit(bool shouldHideImmediately)
        {
            yield break;
        }

        public override void SkipToHidden()
        {
        }
    }
}
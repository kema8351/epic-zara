using System.Collections;
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
using System.Collections;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Loading
{
    public class LoadingUi : OverlayUi
    {
        protected override IEnumerator OnUiStarted() { yield break; }
        protected override IEnumerator OnUiEntrying() { yield break; }
        protected override IEnumerator OnUiEntried() { yield break; }
        protected override IEnumerator OnUiExiting() { yield break; }
        protected override IEnumerator OnUiExited() { yield break; }
    }
}
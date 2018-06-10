using System.Collections;
using Zara.Common.ExBase;

namespace Zara.Expansion.Ui
{
    public abstract class UiExitExecutor : ExMonoBehaviour
    {
        public abstract IEnumerator Exit(bool shouldHideImmediately);
        public abstract void SkipToHidden();
    }
}
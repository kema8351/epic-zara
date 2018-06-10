using System.Collections;
using Zara.Common.ExBase;

namespace Zara.Expansion.Ui
{
    public abstract class UiEntryExecutor : ExMonoBehaviour
    {
        public abstract IEnumerator Entry(bool shouldShowImmediately);
        public abstract void SkipToEntried();
    }
}
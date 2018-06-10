using System.Collections;

namespace Zara.Expansion.Ui
{
    public abstract class BackgroundUi : BaseUi, IBackgroundUi
    {
        IEnumerator IBackgroundUi.PrepareAndEntry()
        {
            yield return Prepare();
            yield return Entry();
        }

        void IBackgroundUi.Exit()
        {
            Exit(true);
        }
    }

    public interface IBackgroundUi
    {
        IEnumerator PrepareAndEntry();
        void Exit();
    }
}
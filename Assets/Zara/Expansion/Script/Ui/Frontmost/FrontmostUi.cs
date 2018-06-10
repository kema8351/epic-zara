using System.Collections;

namespace Zara.Expansion.Ui
{
    public abstract class FrontmostUi : BaseUi, IFrontmostUi
    {
        IEnumerator IFrontmostUi.Prepare()
        {
            yield return Prepare();
        }
    }

    public interface IFrontmostUi
    {
        IEnumerator Prepare();
    }
}
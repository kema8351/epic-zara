using System.Collections;

namespace Zara.Expansion.Ui
{
    public abstract class ResidentUi : BaseUi, IResidentUi
    {
        IEnumerator IResidentUi.Prepare()
        {
            yield return Prepare();
        }
    }

    public interface IResidentUi
    {
        IEnumerator Prepare();
    }
}
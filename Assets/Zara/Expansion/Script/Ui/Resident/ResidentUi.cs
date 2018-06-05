using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Ui;

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
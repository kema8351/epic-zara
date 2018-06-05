using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Ui;

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
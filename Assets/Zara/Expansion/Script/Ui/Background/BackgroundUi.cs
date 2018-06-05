using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Ui;

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
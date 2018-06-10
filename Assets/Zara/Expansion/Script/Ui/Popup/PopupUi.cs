using System;
using System.Collections;
using UnityEngine;

namespace Zara.Expansion.Ui
{
    public abstract class PopupUi : BaseUi, IPopupUi
    {
        IEnumerator IPopupUi.PrepareAndEntry()
        {
            yield return Prepare();
            yield return Entry();
        }

        void IPopupUi.AddUnloadedAction(Action<AsyncOperation> unloadedAction)
        {
            AddUnloadedAction(unloadedAction);
        }
    }

    public interface IPopupUi
    {
        IEnumerator PrepareAndEntry();
        void AddUnloadedAction(Action<AsyncOperation> unloadedAction);
    }
}
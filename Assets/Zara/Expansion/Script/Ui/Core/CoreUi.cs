using System;
using System.Collections;
using UnityEngine;

namespace Zara.Expansion.Ui
{
    public abstract class CoreUi : BaseUi, ICoreUi
    {
        IEnumerator ICoreUi.Prepare()
        {
            yield return Prepare();
        }

        void ICoreUi.Entry()
        {
            Entry();
        }

        void ICoreUi.EntryImmediately()
        {
            EntryImmediately();
        }

        void ICoreUi.Exit(Action<AsyncOperation> onUnloaded)
        {
            AddUnloadedAction(onUnloaded);
            Exit(true);
        }

        void ICoreUi.ExitImmediately(Action<AsyncOperation> onUnloaded)
        {
            AddUnloadedAction(onUnloaded);
            ExitImmediately(true);
        }
    }

    public interface ICoreUi
    {
        IEnumerator Prepare();
        void Entry();
        void EntryImmediately();
        void Exit(Action<AsyncOperation> onUnloaded);
        void ExitImmediately(Action<AsyncOperation> onUnloaded);
    }
}
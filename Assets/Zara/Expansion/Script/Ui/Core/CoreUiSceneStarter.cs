using System;
using System.Collections;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Common.ExScene;
using Zara.Common.Ui;

namespace Zara.Expansion.Ui
{
    public abstract class CoreUiSceneStarter<T> : SceneStarter<T>, ICoreUiCanvas
        where T : CoreUiSceneStarter<T>
    {
        [AutoAdd]
        [SerializeField]
        CanvasOrderUpdater canvasOrderUpdater;

        CanvasOrderUpdater ICoreUiCanvas.CanvasOrderUpdater => canvasOrderUpdater;

        ICoreUiBank UiBank => CoreUiDirector.Instance;

        protected abstract ICoreUi CoreUi { get; }

        protected sealed override IEnumerator OnSceneStarted()
        {
            yield return this.CoreUi.Prepare();
            this.UiBank.NotifyPrepared(this);
        }

        void ICoreUiCanvas.Entry()
        {
            this.CoreUi.Entry();
        }

        void ICoreUiCanvas.EntryImmediately()
        {
            this.CoreUi.EntryImmediately();
        }

        void ICoreUiCanvas.Exit(Action<AsyncOperation> onUnloaded)
        {
            this.CoreUi.Exit(onUnloaded);
        }

        void ICoreUiCanvas.ExitImmediately(Action<AsyncOperation> onUnloaded)
        {
            this.CoreUi.ExitImmediately(onUnloaded);
        }
    }

    public interface ICoreUiCanvas
    {
        CanvasOrderUpdater CanvasOrderUpdater { get; }
        void Entry();
        void EntryImmediately();
        void Exit(Action<AsyncOperation> onUnloaded);
        void ExitImmediately(Action<AsyncOperation> onUnloaded);
    }
}
using System.Collections;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Common.ExScene;
using Zara.Common.Ui;

namespace Zara.Expansion.Ui
{
    public abstract class BackgroundUiSceneStarter<T> : SceneStarter<T>, IBackgroundUiCanvas
        where T : BackgroundUiSceneStarter<T>
    {
        [AutoAdd]
        [SerializeField]
        CanvasOrderUpdater canvasOrderUpdater;

        CanvasOrderUpdater IBackgroundUiCanvas.CanvasOrderUpdater => canvasOrderUpdater;

        IBackgroundUiBank UiBank => BackgroundUiDirector.Instance;

        protected abstract IBackgroundUi BackgroundUi { get; }

        protected sealed override IEnumerator OnSceneStarted()
        {
            this.UiBank.EnqueueCanvas(this);
            yield return this.BackgroundUi.PrepareAndEntry();
            this.UiBank.NotifyEntried(this);
        }

        void IBackgroundUiCanvas.Exit()
        {
            this.BackgroundUi.Exit();
        }

        bool IBackgroundUiCanvas.IsDestroyed => this == null;
    }

    public interface IBackgroundUiCanvas
    {
        CanvasOrderUpdater CanvasOrderUpdater { get; }
        void Exit();
        bool IsDestroyed { get; }
    }
}
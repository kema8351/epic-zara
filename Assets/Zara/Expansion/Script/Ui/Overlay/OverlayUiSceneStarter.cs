using System;
using System.Collections;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Common.ExScene;
using Zara.Common.Ui;

namespace Zara.Expansion.Ui
{
    public abstract class OverlayUiSceneStarter<T> : SceneStarter<T>, IOverlayUiCanvas
        where T : OverlayUiSceneStarter<T>
    {
        [AutoAdd]
        [SerializeField]
        CanvasOrderUpdater canvasOrderUpdater;

        CanvasOrderUpdater IOverlayUiCanvas.CanvasOrderUpdater => canvasOrderUpdater;
        protected abstract int OrderInStratum { get; }
        int IOverlayUiCanvas.OrderInStratum => OrderInStratum;

        IOverlayUiBank UiBank => OverlayUiDirector.Instance;

        protected abstract IOverlayUi OverlayUi { get; }

        protected sealed override IEnumerator OnSceneStarted()
        {
            this.UiBank.RegisterCanvas<T>(this as T);

            yield return this.OverlayUi.Prepare();
        }

        void IOverlayUiCanvas.Show()
        {
            this.OverlayUi.Show();
        }

        void IOverlayUiCanvas.Hide()
        {
            this.OverlayUi.Hide();
        }

        void IOverlayUiCanvas.AddShownAction(Action onShown)
        {
            this.OverlayUi.AddShownAction(onShown);
        }
    }

    public interface IOverlayUiCanvas
    {
        CanvasOrderUpdater CanvasOrderUpdater { get; }
        int OrderInStratum { get; }
        void Show();
        void Hide();
        void AddShownAction(Action onShown);
    }
}
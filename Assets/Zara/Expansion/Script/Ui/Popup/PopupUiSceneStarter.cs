using System.Collections;
using UnityEngine;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Common.Ui;
using Zara.Common.Utility;

namespace Zara.Expansion.Ui
{
    public abstract class PopupUiSceneStarter<T> : SceneStarter<T>, IPopupUiCanvas
        where T : PopupUiSceneStarter<T>
    {
        [AutoAdd]
        [SerializeField]
        CanvasOrderUpdater canvasOrderUpdater;

        CanvasOrderUpdater IPopupUiCanvas.CanvasOrderUpdater => canvasOrderUpdater;

        protected virtual bool Has3dObject => false;
        bool IPopupUiCanvas.Has3dObject => this.Has3dObject;

        IPopupUiBank UiBank => PopupUiDirector.Instance;

        protected abstract IPopupUi PopupUi { get; }

        protected sealed override IEnumerator OnSceneStarted()
        {
            this.UiBank.RegisterCanvas(this);
            this.PopupUi.AddUnloadedAction(this.UiBank.RefleshAction);

            yield return this.PopupUi.PrepareAndEntry();
        }

        bool IPopupUiCanvas.IsDestroyed => this == null;
    }

    public interface IPopupUiCanvas
    {
        CanvasOrderUpdater CanvasOrderUpdater { get; }
        bool IsDestroyed { get; }

        // If a popup ui has 3d object, a next popup ui must use new camera.
        bool Has3dObject { get; }
    }
}
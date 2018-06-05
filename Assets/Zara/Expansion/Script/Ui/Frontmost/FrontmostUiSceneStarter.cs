using System.Collections;
using UnityEngine;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Common.Ui;
using Zara.Common.Utility;

namespace Zara.Expansion.Ui
{
    public abstract class FrontmostUiSceneStarter<T> : SceneStarter<T>, IFrontmostUiCanvas
        where T : FrontmostUiSceneStarter<T>
    {
        [AutoAdd]
        [SerializeField]
        CanvasOrderUpdater canvasOrderUpdater;

        CanvasOrderUpdater IFrontmostUiCanvas.CanvasOrderUpdater => canvasOrderUpdater;
        protected abstract int OrderInStratum { get; }
        int IFrontmostUiCanvas.OrderInStratum => OrderInStratum;

        IFrontmostUiBank UiBank => FrontmostUiDirector.Instance;

        protected abstract IFrontmostUi FrontmostUi { get; }

        protected sealed override IEnumerator OnSceneStarted()
        {
            this.UiBank.RegisterCanvas(this);

            yield return this.FrontmostUi.Prepare();
        }
    }

    public interface IFrontmostUiCanvas
    {
        CanvasOrderUpdater CanvasOrderUpdater { get; }
        int OrderInStratum { get; }
    }
}
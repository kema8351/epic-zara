using System.Collections;
using UnityEngine;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Common.Ui;
using Zara.Common.Utility;

namespace Zara.Expansion.Ui
{
    public abstract class ResidentUiSceneStarter<T> : SceneStarter<T>, IResidentUiCanvas
        where T : ResidentUiSceneStarter<T>
    {
        [AutoAdd]
        [SerializeField]
        CanvasOrderUpdater canvasOrderUpdater;

        CanvasOrderUpdater IResidentUiCanvas.CanvasOrderUpdater => canvasOrderUpdater;
        protected abstract int OrderInStratum { get; }
        int IResidentUiCanvas.OrderInStratum => OrderInStratum;

        IResidentUiBank UiBank => ResidentUiDirector.Instance;

        protected abstract IResidentUi ResidentUi { get; }

        protected sealed override IEnumerator OnSceneStarted()
        {
            this.UiBank.RegisterCanvas(this);

            yield return this.ResidentUi.Prepare();
        }
    }

    public interface IResidentUiCanvas
    {
        CanvasOrderUpdater CanvasOrderUpdater { get; }
        int OrderInStratum { get; }
    }
}
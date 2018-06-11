using UnityEngine;
using Zara.Common.Ui;
using Zara.Common.Utility;
using Zara.Expansion.ExScene;

namespace Zara.Expansion.Ui
{
    public class OverlayUiDirector : Singleton<OverlayUiDirector>, IOverlayUiBank
    {
        public OverlayUiDirector() : base()
        {
        }

        ISceneLoader sceneLoader;
        ICanvasBank canvasBank;
        IdGenerator idGenerator = new IdGenerator();

        public void Init(ISceneLoader sceneLoader, ICanvasBank canvasBank)
        {
            this.sceneLoader = sceneLoader;
            this.canvasBank = canvasBank;
        }

        public OverlayUiOperator Show<T>() where T : IOverlayUiCanvas
        {
            if (StatusCache<T>.Cache == null)
            {
                StatusCache<T>.SetCache(new OverlayUiStatus<T>(idGenerator));
                sceneLoader.Load<T>();
            }

            OverlayUiStatus<T> status = StatusCache<T>.Cache;
            return status.GetChecker();
        }

        void IOverlayUiBank.RegisterCanvas<T>(T overlayUiCanvas)
        {
            if (canvasBank == null)
            {
                Debug.LogError($"has not inited {this.GetType().Name}");
                return;
            }

            OverlayUiStatus<T> status = StatusCache<T>.Cache;
            if (status == null)
            {
                Debug.LogError($"has not inited status by using Show function: {typeof(T).Name}");
                return;
            }

            if (!status.TrySetUiCanvas(overlayUiCanvas))
                return;

            CanvasInStratum canvasInStratum = new CanvasInStratum(
                ConstStratum.Overlay,
                overlayUiCanvas.OrderInStratum,
                overlayUiCanvas.CanvasOrderUpdater);
            canvasBank.AddCanvas(canvasInStratum);
        }

        class StatusCache<T> : StaticLazyCache<StatusCache<T>, OverlayUiStatus<T>> where T : IOverlayUiCanvas
        {
        }
    }

    public interface IOverlayUiBank
    {
        void RegisterCanvas<T>(T OverlayUiCanvas) where T : IOverlayUiCanvas;
    }
}
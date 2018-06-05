using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Ui;
using Zara.Common.Utility;
using Zara.Expansion.ExScene;

namespace Zara.Expansion.Ui
{
    public class OverlayUiDirector : IOverlayUiBank
    {
        public static OverlayUiDirector Instance { get; } = new OverlayUiDirector();

        public OverlayUiDirector()
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
            if (Cache<T>.Status == null)
            {
                Cache<T>.Status = new OverlayUiStatus<T>(idGenerator);
                sceneLoader.Load<T>();
            }

            OverlayUiStatus<T> status = Cache<T>.Status;
            return status.GetChecker();
        }

        void IOverlayUiBank.RegisterCanvas<T>(T overlayUiCanvas)
        {
            if (canvasBank == null)
            {
                Debug.LogError($"has not inited {this.GetType().Name}");
                return;
            }

            OverlayUiStatus<T> status = Cache<T>.Status;
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

        class Cache<T> where T : IOverlayUiCanvas
        {
            public static OverlayUiStatus<T> Status;
        }
    }

    public interface IOverlayUiBank
    {
        void RegisterCanvas<T>(T OverlayUiCanvas) where T : IOverlayUiCanvas;
    }
}
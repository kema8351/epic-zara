using System;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Ui;
using Zara.Common.Utility;

namespace Zara.Expansion.Ui
{
    public class PopupUiDirector : Singleton<PopupUiDirector>, IPopupUiBank
    {
        public PopupUiDirector() : base()
        {
            refleshAction = Reflesh;
        }

        ICanvasBank canvasBank;
        List<IPopupUiCanvas> uiCanvasList = new List<IPopupUiCanvas>();

        public void Init(ICanvasBank canvasBank)
        {
            this.canvasBank = canvasBank;
        }

        void IPopupUiBank.RegisterCanvas<T>(T popupUiCanvas)
        {
            if (canvasBank == null)
            {
                Debug.LogError($"has not inited {this.GetType().Name}");
                return;
            }

            uiCanvasList.RemoveAll(ui => ui == null);
            uiCanvasList.Add(popupUiCanvas);

            CanvasInStratum canvas = new CanvasInStratum(
                ConstStratum.Popup,
                // 呼び出し順に登録するのですべて同じOrderInStratumで登録する
                0,
                popupUiCanvas.CanvasOrderUpdater);
            IEnumerable<CameraMat> cameraMats = EnumerateCameraMats(uiCanvasList);
            canvasBank.AddCanvas(canvas, cameraMats);
        }

        IEnumerable<CameraMat> EnumerateCameraMats(IReadOnlyList<IPopupUiCanvas> uiCanvases)
        {
            for (int i = 1; i < uiCanvases.Count - 1; i++)
            {
                IPopupUiCanvas previousUi = uiCanvases[i - 1];
                if (previousUi.Has3dObject)
                {
                    IPopupUiCanvas currentUi = uiCanvases[i];
                    yield return new CameraMat(currentUi.CanvasOrderUpdater, true);
                }
            }

            if (uiCanvases.Count > 0)
                yield return new CameraMat(uiCanvases[uiCanvases.Count - 1].CanvasOrderUpdater, true);
        }

        Action<AsyncOperation> refleshAction;
        Action<AsyncOperation> IPopupUiBank.RefleshAction => refleshAction;

        void Reflesh(AsyncOperation _)
        {
            // This function is run when a scene is unloaded.
            // The argument AsyncOperation is necessary to register to an unloading AsyncOperation as a completed event. 

            uiCanvasList.RemoveAll(ui => ui.IsDestroyed);
            IEnumerable<CameraMat> cameraMats = EnumerateCameraMats(uiCanvasList);
            canvasBank.ReplaceCameraMats(ConstStratum.Popup, cameraMats);
        }
    }

    public interface IPopupUiBank
    {
        void RegisterCanvas<T>(T popupUi) where T : IPopupUiCanvas;
        Action<AsyncOperation> RefleshAction { get; }
    }
}
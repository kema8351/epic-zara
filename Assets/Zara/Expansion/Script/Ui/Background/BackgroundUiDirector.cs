using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Ui;
using Zara.Common.Utility;

namespace Zara.Expansion.Ui
{
    public class BackgroundUiDirector : IBackgroundUiBank
    {
        public static BackgroundUiDirector Instance { get; } = new BackgroundUiDirector();

        public BackgroundUiDirector()
        {
        }

        ICanvasBank canvasBank;
        List<IBackgroundUiCanvas> backgroundUiCanvasList = new List<IBackgroundUiCanvas>();
        HashSet<IBackgroundUiCanvas> entriedCanvases = new HashSet<IBackgroundUiCanvas>();

        public void Init(ICanvasBank canvasBank)
        {
            this.canvasBank = canvasBank;
        }

        void IBackgroundUiBank.EnqueueCanvas<T>(T backgroundUiCanvas)
        {
            if (canvasBank == null)
            {
                Debug.LogError($"has not inited {this.GetType().Name}");
                return;
            }

            backgroundUiCanvasList.RemoveAll(c => c.IsDestroyed);
            backgroundUiCanvasList.Add(backgroundUiCanvas);

            CanvasInStratum canvasInStratum = new CanvasInStratum(
                ConstStratum.Background,
                // 呼び出し順に登録するのですべて同じOrderInStratumで登録する
                0,
                backgroundUiCanvas.CanvasOrderUpdater);
            canvasBank.AddCanvas(canvasInStratum);
        }

        void IBackgroundUiBank.NotifyEntried(IBackgroundUiCanvas backgroundUiCanvas)
        {
            entriedCanvases.RemoveWhere(c => c.IsDestroyed);
            entriedCanvases.TryAdd(backgroundUiCanvas);

            backgroundUiCanvasList.RemoveAll(c => c.IsDestroyed);
            int forefrontCanvasIndex = GetForefrontCanvasIndex(backgroundUiCanvasList, new FixedHashSet<IBackgroundUiCanvas>(entriedCanvases));
            for (int i = 0; i < forefrontCanvasIndex; i++)
                backgroundUiCanvasList[i].Exit();
        }

        int GetForefrontCanvasIndex(IReadOnlyList<IBackgroundUiCanvas> uiCanvasList, FixedHashSet<IBackgroundUiCanvas> entried)
        {
            for (int i = uiCanvasList.Count - 1; i >= 0; i--)
            {
                if (entried.Contains(uiCanvasList[i]))
                    return i;
            }

            return -1;
        }
    }

    public interface IBackgroundUiBank
    {
        void EnqueueCanvas<T>(T BackgroundUi) where T : IBackgroundUiCanvas;
        void NotifyEntried(IBackgroundUiCanvas backgroundUi);
    }
}
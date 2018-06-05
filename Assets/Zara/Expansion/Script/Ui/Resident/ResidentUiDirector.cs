using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Ui;

namespace Zara.Expansion.Ui
{
    public class ResidentUiDirector : IResidentUiBank
    {
        public static ResidentUiDirector Instance { get; } = new ResidentUiDirector();

        public ResidentUiDirector()
        {
        }

        ICanvasBank canvasBank;

        public void Init(ICanvasBank canvasBank)
        {
            this.canvasBank = canvasBank;
        }

        void IResidentUiBank.RegisterCanvas<T>(T residentUiCanvas)
        {
            if (canvasBank == null)
            {
                Debug.LogError($"has not inited {this.GetType().Name}");
                return;
            }

            CanvasInStratum canvas = new CanvasInStratum(
                ConstStratum.Resident,
                residentUiCanvas.OrderInStratum,
                residentUiCanvas.CanvasOrderUpdater);
            canvasBank.AddCanvas(canvas);
        }
    }

    public interface IResidentUiBank
    {
        void RegisterCanvas<T>(T residentUiCanvas) where T : IResidentUiCanvas;
    }
}
using UnityEngine;
using Zara.Common.Ui;

namespace Zara.Expansion.Ui
{
    public class FrontmostUiDirector : IFrontmostUiBank
    {
        public static FrontmostUiDirector Instance { get; } = new FrontmostUiDirector();

        public FrontmostUiDirector()
        {
        }

        ICanvasBank canvasBank;

        public void Init(ICanvasBank canvasBank)
        {
            this.canvasBank = canvasBank;
        }

        void IFrontmostUiBank.RegisterCanvas<T>(T residentUiCanvas)
        {
            if (canvasBank == null)
            {
                Debug.LogError($"has not inited {this.GetType().Name}");
                return;
            }

            CanvasInStratum canvas = new CanvasInStratum(
                ConstStratum.Frontmost,
                residentUiCanvas.OrderInStratum,
                residentUiCanvas.CanvasOrderUpdater);
            canvasBank.AddCanvas(canvas);
        }
    }

    public interface IFrontmostUiBank
    {
        void RegisterCanvas<T>(T residentUiCanvas) where T : IFrontmostUiCanvas;
    }
}
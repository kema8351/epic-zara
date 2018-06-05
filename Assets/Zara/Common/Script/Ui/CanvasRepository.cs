using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.Ui
{
    public class CanvasRepository : ICanvasListHolder
    {
        IReadOnlyList<CanvasInStratum> emptyCanvasList = new List<CanvasInStratum>(Enumerable.Empty<CanvasInStratum>());
        SortedList<int, List<CanvasInStratum>> canvasListDictionary = new SortedList<int, List<CanvasInStratum>>();

        IReadOnlyList<CanvasInStratum> ICanvasListHolder.GetCanvasList(int stratumId)
        {
            List<CanvasInStratum> result = canvasListDictionary.GetValue(stratumId, false);
            return result ?? emptyCanvasList;
        }

        public void AddCanvas(CanvasInStratum canvas)
        {
            List<CanvasInStratum> canvasList = this.canvasListDictionary.GetValue(canvas.StratumId, false);
            if (canvasList == null)
            {
                canvasList = new List<CanvasInStratum>();
                this.canvasListDictionary.Add(canvas.StratumId, canvasList);
            }

            // DestroyされたCanvasを消去する
            canvasList.RemoveAll(cs => cs.CanvasOrderUpdater == null);

            int index = GetIndexToInsert(canvasList, canvas);
            canvasList.Insert(index, canvas);
        }

        int GetIndexToInsert(List<CanvasInStratum> canvasList, CanvasInStratum insertingCanvas)
        {
            for (int i = canvasList.Count - 1; i >= 0; i--)
            {
                if (canvasList[i].OrderInStratum > insertingCanvas.OrderInStratum)
                    continue;

                return i + 1;
            }
            return 0;
        }

        public IEnumerable<CanvasInStratum> EnumerateCanvases()
        {
            // CanvasOrderModifierを通すことでorderの降順に並ぶことが期待される
            for (int i = canvasListDictionary.Count - 1; i >= 0; i--)
            {
                List<CanvasInStratum> canvasList = canvasListDictionary.Values[i];
                for (int j = canvasList.Count - 1; j >= 0; j--)
                {
                    CanvasInStratum canvasInStratum = canvasList[j];
                    if (canvasInStratum.CanvasOrderUpdater != null)
                        yield return canvasInStratum;
                }
            }
        }
    }

    public struct CanvasInStratum
    {
        public int StratumId { get; private set; }
        public int OrderInStratum { get; private set; }
        public CanvasOrderUpdater CanvasOrderUpdater { get; private set; }

        public CanvasInStratum(int stratumId, int orderInStratum, CanvasOrderUpdater canvasOrderUpdater)
        {
            this.StratumId = stratumId;
            this.OrderInStratum = orderInStratum;
            this.CanvasOrderUpdater = canvasOrderUpdater;
        }
    }
}

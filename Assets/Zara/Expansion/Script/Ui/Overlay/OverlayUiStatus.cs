using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Expansion.Ui
{
    public class OverlayUiStatus<T> where T : IOverlayUiCanvas
    {
        T uiCanvas;
        Action<Action> actionToAddShownAction;
        List<Action> shownActionList;
        Action<int> notifyTerminationAction;
        IdGenerator idGenerator;
        HashSet<int> ids = new HashSet<int>();

        public OverlayUiStatus(IdGenerator idGenerator)
        {
            this.idGenerator = idGenerator;
            this.actionToAddShownAction = AddShownAction;
            this.notifyTerminationAction = NotifyTermination;
        }

        void AddShownAction(Action onShown)
        {
            if (uiCanvas != null)
            {
                uiCanvas.AddShownAction(onShown);
                return;
            }

            if (shownActionList == null)
                shownActionList = ListPool<Action>.Pool.Get();
            shownActionList.Add(onShown);
        }

        void NotifyTermination(int id)
        {
            ids.TryRemove(id);
            if (ids.IsEmpty())
            {
                if (uiCanvas != null)
                    uiCanvas.Hide();
                else
                    InvokeAndClearShownActionList();
            }
        }

        public bool TrySetUiCanvas(T uiCanvas)
        {
            if (this.uiCanvas != null)
            {
                Debug.LogError($"invalid registeration: {typeof(T).Name}");
                return false;
            }

            this.uiCanvas = uiCanvas;

            if (!ids.IsEmpty())
            {
                if (shownActionList != null)
                {
                    foreach (Action onShown in shownActionList)
                        uiCanvas.AddShownAction(onShown);

                    ListPool<Action>.Pool.Put(ref shownActionList);
                    shownActionList = null;
                }

                uiCanvas.Show();
            }

            return true;
        }

        void InvokeAndClearShownActionList()
        {
            if (shownActionList == null)
                return;

            foreach (Action onShown in shownActionList)
                onShown.Invoke();

            ListPool<Action>.Pool.Put(ref shownActionList);
            shownActionList = null;
        }

        public OverlayUiOperator GetChecker()
        {
            if (ids.IsEmpty())
                uiCanvas?.Show();

            int id = idGenerator.Get();
            ids.TryAdd(id);
            return new OverlayUiOperator(actionToAddShownAction, notifyTerminationAction, id);
        }
    }
}
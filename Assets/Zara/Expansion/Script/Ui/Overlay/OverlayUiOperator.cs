using System;
using UnityEngine;

namespace Zara.Expansion.Ui
{
    public struct OverlayUiOperator
    {
        Action<Action> actionToAddShownAction;
        Action<int> notifyTerminationAction;
        int id;
        bool isNotified;

        public OverlayUiOperator(Action<Action> actionToAddShownAction, Action<int> notifyTerminationAction, int id)
        {
            this.actionToAddShownAction = actionToAddShownAction;
            this.notifyTerminationAction = notifyTerminationAction;
            this.id = id;
            this.isNotified = false;
        }

        public void AddShownAction(Action onShown)
        {
            actionToAddShownAction.Invoke(onShown);
        }

        public void NotifyTermination()
        {
            if (isNotified)
            {
                Debug.LogWarning("already notified");
                return;
            }
            isNotified = true;
            notifyTerminationAction.Invoke(id);
        }
    }
}
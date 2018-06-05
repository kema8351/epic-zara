using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Ui;

namespace Zara.Expansion.Ui
{
    public abstract class OverlayUi : BaseUi, IOverlayUi
    {
        IEnumerator IOverlayUi.Prepare()
        {
            yield return Prepare();
        }

        void IOverlayUi.Show()
        {
            Entry();
        }

        void IOverlayUi.Hide()
        {
            Exit(shouldUnload: false);
        }

        void IOverlayUi.AddShownAction(Action onShown)
        {
            AddEntriedAction(onShown);
        }
    }

    public interface IOverlayUi
    {
        IEnumerator Prepare();
        void Show();
        void Hide();
        void AddShownAction(Action onShown);
    }
}
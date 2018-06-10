using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Header
{
    public class HeaderUi : ResidentUi, IHeaderUi
    {
        [SerializeField] Button add2dButton;
        [SerializeField] Button add3dButton;

        protected override IEnumerator OnUiStarted()
        {
            add2dButton.onClick.AddListener(() => Game.Scene.LoadPopup2d());
            add3dButton.onClick.AddListener(() => Game.Scene.LoadPopup3d());
            yield break;
        }

        protected override IEnumerator OnUiEntrying() { yield break; }
        protected override IEnumerator OnUiEntried() { yield break; }
        protected override IEnumerator OnUiExiting() { yield break; }
        protected override IEnumerator OnUiExited() { yield break; }

        public void Show()
        {
            Entry();
        }

        public void Hide()
        {
            Exit(false);
        }
    }
}
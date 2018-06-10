using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Popup2d
{
    public class Popup2dSceneStarter : PopupUiSceneStarter<Popup2dSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        Popup2dUi ui;

        protected override IPopupUi PopupUi => ui;
    }
}
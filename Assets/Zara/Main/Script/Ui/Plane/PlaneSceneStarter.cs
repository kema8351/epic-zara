using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Plane
{
    public class PlaneSceneStarter : BackgroundUiSceneStarter<PlaneSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        PlaneUi ui;

        protected override IBackgroundUi BackgroundUi => ui;
    }
}
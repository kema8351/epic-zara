using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Loading
{
    public class LoadingSceneStarter : OverlayUiSceneStarter<LoadingSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        LoadingUi ui;

        protected override IOverlayUi OverlayUi => ui;

        protected override int OrderInStratum => 1;
    }
}
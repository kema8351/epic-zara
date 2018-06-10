using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Error
{
    public class ErrorSceneStarter : FrontmostUiSceneStarter<ErrorSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        ErrorUi ui;
        public ErrorUi Ui => ui;

        protected override IFrontmostUi FrontmostUi => ui;

        protected override int OrderInStratum => 1;
    }
}
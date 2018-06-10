using UnityEngine;
using Zara.Common.Menu;
using Zara.Common.Utility;

namespace Zara.Common.ExBase
{
    public class ExMonoBehaviour : MonoBehaviour
#if UNITY_EDITOR
    , IModifiedComponent
#endif
    {
#if UNITY_EDITOR
        [System.Diagnostics.Conditional(ConstConditional.Editor)]
        void Reset()
        {
            Modify();
        }

        void IModifiedComponent.Modify()
        {
            Modify();
        }

        [ContextMenu(ConstMenu.Modify)]
        [System.Diagnostics.Conditional(ConstConditional.Editor)]
        void Modify()
        {
            AutoAttributeUtility.SetComponentAutomatilcally(this);
            ModifyMonoBehaviour();
        }
#endif

        [System.Diagnostics.Conditional(ConstConditional.Editor)]
        protected virtual void ModifyMonoBehaviour() { }
    }
}
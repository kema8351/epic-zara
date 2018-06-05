using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExBase;
using Zara.Common.Utility;
using Zara.Expansion.ExBase;
using Zara.Expansion.Utility;

namespace Zara.Common.Ui
{
    public class CanvasOrderUpdater : SortingOrderUpdater
    {
        [AutoAddCanvas]
        [SerializeField]
        Canvas canvas;

        public int CanvasOrder => canvas.sortingOrder;

        protected override void SetOrderInDerivedClass(int order)
        {
            canvas.sortingOrder = order;
        }

        protected override void SetCameraInDerivedClass(Camera camera)
        {
            canvas.worldCamera = camera;
        }

#if UNITY_EDITOR
        [SerializeField]
        bool defaultCanvasScaler = true;

        [SerializeField]
        bool defaultGraphicRaycaster = true;

        protected override void ModifyComponentInDerivedClass()
        {
            if (defaultCanvasScaler)
                ModifyCanvasScaler();

            if (defaultGraphicRaycaster)
                ModifyGraphicRaycaster();
        }

        void ModifyCanvasScaler()
        {
            if (canvas.rootCanvas == canvas)
            {
                var canvasScaler = ModificationUtility.GetAndAddComponent<CanvasScaler>(this);
                DefaultSettingUtility.SetCanvasScaler(canvasScaler);
            }
            else
            {
                ModificationUtility.DeleteComponent<CanvasScaler>(this);
            }
        }

        void ModifyGraphicRaycaster()
        {
            var graphicRaycaster = ModificationUtility.GetAndAddComponent<GraphicRaycaster>(this);
            DefaultSettingUtility.SetGraphicRaycaster(graphicRaycaster);
        }
#endif
    }
}
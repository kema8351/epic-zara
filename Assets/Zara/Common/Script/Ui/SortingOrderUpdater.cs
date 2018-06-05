using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Common.Utility;

namespace Zara.Common.Ui
{
    public abstract class SortingOrderUpdater : ExMonoBehaviour
    {
        [SerializeField]
        SortingOrderUpdater[] childSortingOrderUpdaters;

        [SerializeField]
        int relativeOrder = 0;

        List<SortingOrderUpdater> additionalSortingOrderUpdaters = null;

        public void SetOrder(int order)
        {
            int newOrder = order + relativeOrder;

            SetOrderInDerivedClass(newOrder);

            foreach (var updater in EnumerateSortingOrderUpdaters())
                updater.SetOrder(newOrder);
        }

        public void SetCamera(Camera camera)
        {
            // Cameraを設定するのはRootのみなのでchildには設定を行わない
            SetCameraInDerivedClass(camera);
        }

        IEnumerable<SortingOrderUpdater> EnumerateSortingOrderUpdaters()
        {
            if (additionalSortingOrderUpdaters == null)
                return childSortingOrderUpdaters;
            else
                return childSortingOrderUpdaters.Concat(
                    additionalSortingOrderUpdaters.Where(u => u != null));
        }

        public void AddChildSortingOrderUpdater(SortingOrderUpdater updater)
        {
            if (additionalSortingOrderUpdaters == null)
                additionalSortingOrderUpdaters = new List<SortingOrderUpdater>();

            additionalSortingOrderUpdaters.Add(updater);
        }

        protected abstract void SetOrderInDerivedClass(int order);
        protected abstract void SetCameraInDerivedClass(Camera camera);

#if UNITY_EDITOR
        protected override void ModifyMonoBehaviour()
        {
            base.ModifyMonoBehaviour();

            this.childSortingOrderUpdaters = ModificationUtility.GetDirectChildComponents<SortingOrderUpdater>(this);
            gameObject.layer = ConstUi.UserLayerMask;
            ModifyComponentInDerivedClass();
        }

        protected abstract void ModifyComponentInDerivedClass();
#endif
    }

    public struct UiOrderSetting
    {
        public UiOrderSetting(int sortingOrder, int sortingLayerId, Camera camera)
        {
            this.SortingOrder = sortingOrder;
            this.SortingLayerId = sortingLayerId;
            this.Camera = camera;
        }

        public int SortingOrder { get; private set; }
        public int SortingLayerId { get; private set; }
        public Camera Camera { get; private set; }
    }
}
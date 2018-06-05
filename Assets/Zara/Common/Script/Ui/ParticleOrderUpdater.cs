using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Utility;
using Zara.Expansion.ExBase;

namespace Zara.Common.Ui
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleOrderUpdater : SortingOrderUpdater
    {
        [AutoGetParticle]
        [SerializeField]
        ParticleSystemRenderer particleSystemRenderer;

        protected override void SetOrderInDerivedClass(int order)
        {
            particleSystemRenderer.sortingOrder = order;
        }

        protected override void SetCameraInDerivedClass(Camera camera)
        {
        }
#if UNITY_EDITOR
        protected override void ModifyComponentInDerivedClass()
        {
        }
#endif
    }
}
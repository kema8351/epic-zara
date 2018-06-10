using UnityEngine;

namespace Zara.Common.Ui
{
    public static class ConstUi
    {
        public static readonly Vector3 CameraPositionBase = Vector3.down * 1024f;
        public static readonly Vector3 CameraPositionInterval = Vector3.down * 512f;
#if UNITY_EDITOR
        public static readonly int SortingLayerId = SortingLayer.NameToID("Default");
        public static readonly int UserLayerMask = LayerMask.NameToLayer("UI");
#endif
    }
}
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.Ui;

namespace Zara.Expansion.Utility
{
#if UNITY_EDITOR
    public class DefaultSettingUtility
    {
        public static void SetCanvas(Canvas canvas)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.overrideSorting = true;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 128f;
            canvas.sortingLayerID = ConstUi.SortingLayerId;
        }

        public static void SetCanvasGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        public static void SetCanvasScaler(CanvasScaler canvasScaler)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1440f);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0f;
            canvasScaler.referencePixelsPerUnit = 256f;
        }

        public static void SetGraphicRaycaster(GraphicRaycaster graphicRaycaster)
        {
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        }

        public static void SetParticleSystemRenderer(ParticleSystemRenderer particleSystemRenderer)
        {
            particleSystemRenderer.sortingLayerID = ConstUi.SortingLayerId;
        }
    }
#endif
}
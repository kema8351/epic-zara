using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExCamera;
using Zara.Common.Utility;
using Zara.Expansion.ExCamera;

namespace Zara.Common.Ui
{
    public interface ICanvasBank
    {
        void AddCanvas(CanvasInStratum canvas, IEnumerable<CameraMat> cameraMats = null);
        void ReplaceCameraMats(int stratumId, IEnumerable<CameraMat> cameraMats);
    }

    public class CanvasDirector : ICanvasBank
    {
        CanvasRepository canvasRepository;
        CanvasOrderModifier canvasOrderModifier;
        CameraMatRepository cameraMatRepository;
        List<CameraInStudio> cameraList;
        ICameraGenerator uiCameraGenerator;
        ICameraBank cameraBank;

        public CanvasDirector(
            IEnumerable<StratumSetting> stratumSettings,
            ICameraGenerator uiCameraGenerator,
            ICameraBank cameraBank)
        {
            canvasRepository = new CanvasRepository();
            canvasOrderModifier = new CanvasOrderModifier(stratumSettings, canvasRepository);
            cameraMatRepository = new CameraMatRepository();
            cameraList = new List<CameraInStudio>();
            this.uiCameraGenerator = uiCameraGenerator;
            this.cameraBank = cameraBank;
        }

        void ICanvasBank.AddCanvas(CanvasInStratum canvas, IEnumerable<CameraMat> cameraMats)
        {
            canvasRepository.AddCanvas(canvas);
            canvasOrderModifier.SetOrder(canvas.StratumId);

            if (cameraMats != null)
                cameraMatRepository.ReplaceCameraMatList(canvas.StratumId, cameraMats);

            SetCameraToCanvases();
        }

        void ICanvasBank.ReplaceCameraMats(int stratumId, IEnumerable<CameraMat> cameraMats)
        {
            cameraMatRepository.ReplaceCameraMatList(stratumId, cameraMats);
            SetCameraToCanvases();
        }

        void SetCameraToCanvases()
        {
            var dictinoaryPool = DictionaryPool<CanvasOrderUpdater, CameraMat>.Pool;
            var cameraMats = dictinoaryPool.Get();
            foreach (CameraMat cameraMat in cameraMatRepository.EnumerateCameraMats())
                cameraMats.Add(cameraMat.CanvasOrderUpdater, cameraMat);

            // camera数の調整
            int cameraCount = cameraMats.Count + 1;
            AdjustCameraCount(cameraCount, ref cameraList);

            IEnumerable<CanvasInStratum> sortedCanvases = canvasRepository.EnumerateCanvases();
            // CanvasRepositoryの内部実装によりorder in layerの降順で列挙されるので
            // 以下のOrderByDescendingは省略することができる
            //.OrderByDescending((CanvasInStratum canvas) => canvas.CanvasOrderUpdater.CanvasOrder);

            int cameraIndex = cameraCount - 1;
            CameraInStudio cameraInStudio = cameraList[cameraIndex];
            Camera camera = (cameraInStudio.CameraOperator as ICameraHolder).Camera;
            float maxBlurryCameraDepth = -1f;
            foreach (CanvasInStratum canvas in sortedCanvases)
            {
                CanvasOrderUpdater canvasOrderUpdater = canvas.CanvasOrderUpdater;
                CameraMat cameraMat;
                if (cameraMats.TryGetValue(canvasOrderUpdater, out cameraMat))
                {
                    canvasOrderUpdater.SetCamera(camera);

                    cameraIndex--;
                    if (cameraIndex < 0)
                    {
                        Debug.LogError("Unexpected camera index: " + cameraIndex);
                        cameraIndex = 0;
                    }
                    cameraInStudio = cameraList[cameraIndex];
                    camera = (cameraInStudio.CameraOperator as ICameraHolder).Camera;

                    // Blurをかける最大Depthを取得しておく
                    if (cameraMat.IsNecessaryForBlurryBack &&
                        maxBlurryCameraDepth < camera.depth)
                        maxBlurryCameraDepth = camera.depth;
                }
                else
                {
                    canvasOrderUpdater.SetCamera(camera);
                }
            }

            // CameraListを取得しBlurをかける／外す
            foreach (CameraInStudio c in cameraBank.EnumerateCameras())
            {
                c.CameraOperator.SetBlur(c.CameraOperator.CameraDepth <= maxBlurryCameraDepth);
            }

            dictinoaryPool.Put(cameraMats);
        }

        void AdjustCameraCount(int necessaryCameraCount, ref List<CameraInStudio> refCameraList)
        {
            for (int i = refCameraList.Count; i < necessaryCameraCount; i++)
            {
                CameraOperator cameraOperator = uiCameraGenerator.Generate();
                cameraOperator.transform.position = ConstUi.CameraPositionBase + ConstUi.CameraPositionInterval * i;
                CameraInStudio cameraInStudio = new CameraInStudio(ConstCamera.Ui, i, cameraOperator);

                refCameraList.Add(cameraInStudio);
                cameraBank.AddCamera(cameraInStudio);
            }
            for (int i = 0; i < necessaryCameraCount; i++)
                refCameraList[i].CameraOperator.EnableCamera(true);
            for (int i = necessaryCameraCount; i < refCameraList.Count; i++)
                refCameraList[i].CameraOperator.EnableCamera(false);
        }
    }
}
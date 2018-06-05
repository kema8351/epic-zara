using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExCamera
{
    public class CameraRepository : ICameraListHolder
    {
        IReadOnlyList<CameraInStudio> emptyCameraList = new List<CameraInStudio>(Enumerable.Empty<CameraInStudio>());
        SortedList<int, List<CameraInStudio>> cameraListDictionary = new SortedList<int, List<CameraInStudio>>();

        IReadOnlyList<CameraInStudio> ICameraListHolder.GetCameraList(int studioId)
        {
            List<CameraInStudio> result = cameraListDictionary.GetValue(studioId, false);
            return result ?? emptyCameraList;
        }

        public void AddCamera(CameraInStudio camera)
        {
            List<CameraInStudio> cameraList = this.cameraListDictionary.GetValue(camera.StudioId, false);
            if (cameraList == null)
            {
                cameraList = new List<CameraInStudio>();
                this.cameraListDictionary.Add(camera.StudioId, cameraList);
            }

            // DestroyされたCameraを消去する
            cameraList.RemoveAll(cs => cs.CameraOperator == null);

            int index = GetIndexToInsert(cameraList, camera);
            cameraList.Insert(index, camera);
        }

        int GetIndexToInsert(List<CameraInStudio> cameraList, CameraInStudio insertingCamera)
        {
            for (int i = cameraList.Count - 1; i >= 0; i--)
            {
                if (cameraList[i].DepthInStudio > insertingCamera.DepthInStudio)
                    continue;

                return i + 1;
            }
            return 0;
        }

        public IEnumerable<CameraInStudio> EnumerateCameras()
        {
            // CameraDepthModifierを通すことでdepth順に並ぶことが期待される
            for (int i = cameraListDictionary.Count - 1; i >= 0; i--)
            {
                List<CameraInStudio> cameraList = cameraListDictionary.Values[i];
                for (int j = cameraList.Count - 1; j >= 0; j--)
                    yield return cameraList[j];
            }
        }
    }

    public struct CameraInStudio
    {
        public int StudioId { get; private set; }
        public int DepthInStudio { get; private set; }
        public CameraOperator CameraOperator { get; private set; }

        public CameraInStudio(int studioId, int depthInStudio, CameraOperator cameraOperator)
        {
            this.StudioId = studioId;
            this.DepthInStudio = depthInStudio;
            this.CameraOperator = cameraOperator;
        }
    }
}
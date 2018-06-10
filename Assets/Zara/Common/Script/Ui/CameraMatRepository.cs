using System.Collections.Generic;
using System.Linq;
using Zara.Common.Utility;

namespace Zara.Common.Ui
{
    public class CameraMatRepository
    {
        Dictionary<int, List<CameraMat>> cameraMatListDictionary = new Dictionary<int, List<CameraMat>>();

        public void ReplaceCameraMatList(int stratumId, IEnumerable<CameraMat> cameraMats)
        {
            List<CameraMat> cameraMatList = this.cameraMatListDictionary.GetValue(stratumId, false);
            if (cameraMatList == null)
            {
                cameraMatList = new List<CameraMat>();
                this.cameraMatListDictionary.Add(stratumId, cameraMatList);
            }

            cameraMatList.Clear();
            cameraMatList.AddRange(cameraMats);
        }

        public IEnumerable<CameraMat> EnumerateCameraMats()
        {
            return cameraMatListDictionary.Values
                .SelectMany(list => list)
                .Where(cameraMat => cameraMat.CanvasOrderUpdater != null);
        }
    }

    public struct CameraMat
    {
        public CanvasOrderUpdater CanvasOrderUpdater { get; private set; }
        public bool IsNecessaryForBlurryBack { get; private set; }

        public CameraMat(CanvasOrderUpdater canvasOrderUpdater, bool isNecessaryForBlurryBack)
        {
            this.CanvasOrderUpdater = canvasOrderUpdater;
            this.IsNecessaryForBlurryBack = isNecessaryForBlurryBack;
        }
    }
}

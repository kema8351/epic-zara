using System.Collections.Generic;

namespace Zara.Common.ExCamera
{
    public interface ICameraBank
    {
        void AddCamera(CameraInStudio camera);
        IEnumerable<CameraInStudio> EnumerateCameras();
    }

    public class CameraDirector : ICameraBank
    {
        CameraRepository cameraRepository;
        CameraDepthModifier cameraDepthModifier;

        public CameraDirector(IEnumerable<StudioSetting> studioSettings)
        {
            cameraRepository = new CameraRepository();
            cameraDepthModifier = new CameraDepthModifier(studioSettings, cameraRepository);
        }

        void ICameraBank.AddCamera(CameraInStudio camera)
        {
            cameraRepository.AddCamera(camera);
            cameraDepthModifier.SetDepth(camera.StudioId);
        }

        IEnumerable<CameraInStudio> ICameraBank.EnumerateCameras()
        {
            return cameraRepository.EnumerateCameras();
        }
    }
}
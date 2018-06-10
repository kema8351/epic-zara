using UnityEngine;
using UnityEngine.PostProcessing;
using Zara.Common.ExBase;

namespace Zara.Common.ExCamera
{
    public interface ICameraHolder
    {
        Camera Camera { get; }
    }

    public class CameraOperator : ExMonoBehaviour, ICameraHolder
    {
        [AutoAdd]
        [SerializeField]
        Camera cameraComponent;

        [SerializeField]
        PostProcessingBehaviour postProcessing;

        // 参照のみ行うこと
        Camera ICameraHolder.Camera => cameraComponent;

        public float CameraDepth => cameraComponent.depth;

        PostProcessingProfile profile = null;

        void Awake()
        {
            profile = Instantiate<PostProcessingProfile>(postProcessing.profile);
            postProcessing.profile = profile;
        }

        public void SetDepth(float depth)
        {
            cameraComponent.depth = depth;
        }

        public void EnableCamera(bool enabled)
        {
            this.gameObject.SetActive(enabled);
        }

        public void SetBlur(bool enabled)
        {
            postProcessing.profile.depthOfField.enabled = enabled;
        }

        void OnDestroy()
        {
            Destroy(profile);
            profile = null;
        }
    }
}
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Common.ExCamera;

namespace Zara.Common.Ui
{
    public class UiCameraGenerator : ExMonoBehaviour, ICameraGenerator
    {
        [SerializeField]
        CameraOperator uiCameraOperatorPrefab;

        CameraOperator ICameraGenerator.Generate()
        {
            return Instantiate<CameraOperator>(uiCameraOperatorPrefab);
        }
    }
}
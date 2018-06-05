using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExCamera;
using Zara.Common.ExBase;

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
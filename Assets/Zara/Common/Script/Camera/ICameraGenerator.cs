using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.ExCamera
{
    public interface ICameraGenerator
    {
        CameraOperator Generate();
    }
}
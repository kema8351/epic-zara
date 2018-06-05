using UnityEngine;
using System;
using Zara.Common.ExScene;
using Zara.Expansion.Ui;

namespace Zara.Expansion.ExScene
{
    public partial class SceneCaller
    {
        ISceneLoader sceneLoader;

        public SceneCaller(ISceneLoader sceneLoader)
        {
            this.sceneLoader = sceneLoader;
        }
    }
}

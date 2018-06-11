using System;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Expansion.ExScene
{
    public partial class SceneLoader : ISceneLoader
    {
        class LoadingActionCache<T> : StaticLazyCache<LoadingActionCache<T>, Action<Action<T>>>
        {
        }

        void ISceneLoader.Load<T>(Action<T> onLoaded)
        {
            Action<Action<T>> loadingAction = LoadingActionCache<T>.Cache;

            if (loadingAction == null)
            {
                Debug.LogError($"has not registered loading action: {typeof(T).Name}");
                return;
            }

            loadingAction.Invoke(onLoaded);
        }
    }
}

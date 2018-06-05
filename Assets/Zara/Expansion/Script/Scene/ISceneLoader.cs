using System;

namespace Zara.Expansion.ExScene
{
    public interface ISceneLoader
    {
        void Load<T>(Action<T> onLoaded = null);
    }
}

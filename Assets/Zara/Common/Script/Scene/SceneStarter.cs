using System.Collections;
using Zara.Common.ExBase;

namespace Zara.Common.ExScene
{
    public abstract class SceneStarter<T> : ExMonoBehaviour
        where T : SceneStarter<T>
    {
        IEnumerator Start()
        {
            ILoadedActionHolder loadedActionHolder = SceneDirector.Instance;
            loadedActionHolder.RunLoadedAction(this as T);

            return OnSceneStarted();
        }

        protected abstract IEnumerator OnSceneStarted();
    }
}
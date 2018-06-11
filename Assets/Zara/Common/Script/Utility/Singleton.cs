using System;

namespace Zara.Common.Utility
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        public static T Instance { get; private set; } = new T();
        static Action restartAction = null;

        public Singleton()
        {
            if (restartAction == null)
                restartAction = () => Instance = new T();

            StaticFieldRestarter.RegisterRestartAction(typeof(T), restartAction);
        }
    }
}
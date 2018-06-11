namespace Zara.Common.Utility
{
    public class StaticInstanceCache<T, TValue>
        where T : StaticInstanceCache<T, TValue>
        where TValue : new()
    {
        public static TValue Cache { get; protected set; } = new TValue();

        static StaticInstanceCache()
        {
            StaticFieldRestarter.RegisterRestartAction(typeof(T), Restart);
        }

        static void Restart()
        {
            Cache = new TValue();
        }


    }
}
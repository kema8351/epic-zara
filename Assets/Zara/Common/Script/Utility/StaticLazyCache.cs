namespace Zara.Common.Utility
{
    public class StaticLazyCache<T, TValue> where T : StaticLazyCache<T, TValue>
    {
        public static TValue Cache { get; protected set; }

        static StaticLazyCache()
        {
            StaticFieldRestarter.RegisterRestartAction(typeof(T), Restart);
        }

        static void Restart()
        {
            Cache = default(TValue);
        }

        public static void SetCache(TValue cache)
        {
            Cache = cache;
        }

    }
}
using System.Collections.Generic;
using System.Linq;

namespace Zara.Common.Utility
{
    public class CollectionPool<T, TValue> where T : ICollection<TValue>, new()
    {
        public static CollectionPool<T, TValue> Pool { get; } = new CollectionPool<T, TValue>();

        Stack<T> pool = new Stack<T>();

        public CollectionPool()
        {
        }

        public T Get()
        {
            return Get(Enumerable.Empty<TValue>());
        }

        public T Get(IEnumerable<TValue> values)
        {
            T result =
                pool.IsEmpty() ?
                new T() :
                pool.Pop();

            result.Clear();

            foreach (TValue v in values)
                result.Add(v);

            return result;
        }

        public void Put(ref T t)
        {
            t.Clear();
            pool.Push(t);
            t = default(T);
        }
    }

    public class HashSetPool<T> : CollectionPool<HashSet<T>, T>
    {
    }

    public class ListPool<T> : CollectionPool<List<T>, T>
    {
    }
}
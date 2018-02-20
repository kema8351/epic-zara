using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zara.Common.Utility
{
    public class CollectionPool<T, TValue> where T : ICollection<TValue>, new()
    {
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

        public void Put(T t)
        {
            t.Clear();
            pool.Push(t);
        }
    }
}
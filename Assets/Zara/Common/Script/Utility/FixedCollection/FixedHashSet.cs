using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.Utility
{
    public struct FixedHashSet<T> : IEnumerable<T>
    {
        HashSet<T> hashSet;

        public FixedHashSet(IEnumerable<T> values)
        {
            try
            {
                hashSet = new HashSet<T>(values);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                hashSet = new HashSet<T>();
            }
        }

        public FixedHashSet(HashSet<T> hashSet)
        {
            this.hashSet = hashSet;
        }

        public bool Contains(T value)
        {
            return hashSet.Contains(value);
        }

        public bool IsNull => hashSet == null;
        public bool IsEmpty => hashSet.Count <= 0;
        public int Count => hashSet.Count;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return hashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hashSet.GetEnumerator();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Zara.Common.Utility
{
    public struct FixedList<T> : IEnumerable<T>
    {
        List<T> list;

        public FixedList(IEnumerable<T> values)
        {
            try
            {
                list = new List<T>(values);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                list = new List<T>();
            }
        }

        public FixedList(List<T> list)
        {
            this.list = list;
        }

        public bool IsNull => list == null;
        public bool IsEmpty => list.Count <= 0;
        public int Count => list.Count;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
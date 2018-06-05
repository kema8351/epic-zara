using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.Utility
{
    public class NullOnlyCoroutineOwner
    {
        List<IEnumerator> enumerators = new List<IEnumerator>();
        Coroutine coroutine = null;
        Queue<int> vacantIndices = new Queue<int>();
        MonoBehaviour monoBehaviour = null;

        public NullOnlyCoroutineOwner(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
        }

        public void Run(IEnumerator enumerator)
        {
            if (isDestructed)
                return;

            if (enumerator == null)
                return;

            if (!enumerator.MoveNext())
                return;

            if (vacantIndices.IsEmpty())
                enumerators.Add(enumerator);
            else
                enumerators[vacantIndices.Dequeue()] = enumerator;

            if (coroutine == null)
                coroutine = monoBehaviour.StartCoroutine(MoveNext());
        }

        IEnumerator MoveNext()
        {
            while (enumerators.Count > vacantIndices.Count)
            {
                yield return new WaitForEndOfFrame();

                if (isDestructed)
                    yield break;

                for (int i = 0; i < enumerators.Count; i++)
                {
                    IEnumerator enumerator = enumerators[i];

                    if (enumerator == null)
                        continue;

                    if (enumerator.MoveNext())
                        continue;

                    enumerators[i] = null;
                    vacantIndices.Enqueue(i);
                }
            }

            ClearCoroutine();
        }

        void ClearCoroutine()
        {
            enumerators.Clear();
            vacantIndices.Clear();
            coroutine = null;
        }

        #region destroy

        bool isDestructed = false;

        public void Destruct()
        {
            isDestructed = true;
            ClearCoroutine();
        }

        #endregion
    }
}
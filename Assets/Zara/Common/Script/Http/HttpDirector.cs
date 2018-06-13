using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zara.Common.Utility;

namespace Zara.Common.Http
{
    public class HttpDirector : IHttpDirector
    {
        const int MaxRequestCount = 100;
        int currentRequestingCount = 0;

        NullOnlyCoroutineOwner nullOnlyCoroutineOwner;
        Queue<UnityWebRequest> waitingRequestQueue = new Queue<UnityWebRequest>();

        public HttpDirector(MonoBehaviour coroutineStarter)
        {
            nullOnlyCoroutineOwner = new NullOnlyCoroutineOwner(coroutineStarter);
        }

        HttpRequestStatus IHttpDirector.Get(string url, byte[] sendingBytes)
        {
            return Request(url, UnityWebRequest.kHttpVerbGET, sendingBytes);
        }

        HttpRequestStatus IHttpDirector.Post(string url, byte[] sendingBytes)
        {
            return Request(url, UnityWebRequest.kHttpVerbPOST, sendingBytes);
        }

        HttpRequestStatus Request(string url, string method, byte[] sendingBytes)
        {
            var request = new UnityWebRequest(url, method);
            if (sendingBytes != null)
                request.uploadHandler = new UploadHandlerRaw(sendingBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            if (currentRequestingCount < MaxRequestCount)
                nullOnlyCoroutineOwner.Run(RequestAsync(request));
            else
                waitingRequestQueue.Enqueue(request);

            return new HttpRequestStatus(request);
        }

        IEnumerator RequestAsync(UnityWebRequest request)
        {
            currentRequestingCount++;

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone)
                yield return null;

            currentRequestingCount--;

            if (waitingRequestQueue.Count > 0)
            {
                UnityWebRequest waitingRequest = waitingRequestQueue.Dequeue();
                nullOnlyCoroutineOwner.Run(RequestAsync(waitingRequest));
            }
        }
    }

    public interface IHttpDirector
    {
        HttpRequestStatus Get(string url, byte[] sendingBytes = null);
        HttpRequestStatus Post(string url, byte[] sendingBytes = null);
    }
}
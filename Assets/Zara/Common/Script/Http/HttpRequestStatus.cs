using UnityEngine.Networking;

namespace Zara.Common.Http
{
    public struct HttpRequestStatus
    {
        public bool IsDone => webRequest.isDone;
        public float Progress => (webRequest.uploadProgress + webRequest.downloadProgress) / 2f;
        public byte[] ReceivedBytes => webRequest.downloadHandler.data;
        public long ResponseCode => webRequest.responseCode;
        public bool IsHttpError => webRequest.isHttpError;
        public bool IsNetworkError => webRequest.isNetworkError;


        UnityWebRequest webRequest;


        public HttpRequestStatus(UnityWebRequest webRequest)
        {
            this.webRequest = webRequest;
        }
    }
}
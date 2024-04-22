﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Duck.Http.Service.Unity
{
	public class UnityHttpService : IHttpService
	{
		public IHttpRequest Get(string uri)
		{
			return new UnityHttpRequest(UnityWebRequest.Get(uri));
		}

		public IHttpRequest GetTexture(string uri)
		{
			return new UnityHttpRequest(UnityWebRequestTexture.GetTexture(uri));
		}

		public IHttpRequest Post(string uri, string postData)
		{
			return new UnityHttpRequest(UnityWebRequest.PostWwwForm(uri, postData));
		}

		public IHttpRequest Post(string uri, WWWForm formData)
		{
			return new UnityHttpRequest(UnityWebRequest.Post(uri, formData));
		}

		public IHttpRequest Post(string uri, Dictionary<string, string> formData)
		{
			return new UnityHttpRequest(UnityWebRequest.Post(uri, formData));
		}

		public IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
		{
			return new UnityHttpRequest(UnityWebRequest.Post(uri, multipartForm));
		}

		public IHttpRequest Post(string uri, byte[] bytes, string contentType)
		{
			var unityWebRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST)
			{
				uploadHandler = new UploadHandlerRaw(bytes)
				{
					contentType = contentType
				},
				downloadHandler = new DownloadHandlerBuffer()
			};
			return new UnityHttpRequest(unityWebRequest);
		}

		public IHttpRequest PostJson(string uri, string json)
		{
			return Post(uri, Encoding.UTF8.GetBytes(json), "application/json");
		}

		public IHttpRequest PostJson<T>(string uri, T payload) where T : class
		{
			return PostJson(uri, JsonUtility.ToJson(payload));
		}

		public IHttpRequest Put(string uri, byte[] bodyData)
		{
			return new UnityHttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		public IHttpRequest Put(string uri, string bodyData)
		{
			return new UnityHttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		public IHttpRequest Delete(string uri)
		{
			return new UnityHttpRequest(UnityWebRequest.Delete(uri));
		}

		public IHttpRequest Head(string uri)
		{
			return new UnityHttpRequest(UnityWebRequest.Head(uri));
		}

		public IEnumerator Send(IHttpRequest request, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null,string requestMethod = "")
		{
			var unityHttpRequest = (UnityHttpRequest) request;
			var unityWebRequest = unityHttpRequest.UnityWebRequest;
#if !UNITY_WEBGL
			unityWebRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
#else
            unityWebRequest.SetRequestHeader("Content-Type", "text/plain");
#endif
            if (!string.IsNullOrEmpty(requestMethod))
            {
                unityWebRequest.method = "POST";
            }
			yield return unityWebRequest.SendWebRequest();
			
			var response = CreateResponse(unityWebRequest);
			//Debug.LogError("response " + response.IsHttpError + " "+ response.IsNetworkError +" s "+ response.IsSuccessful);
			//Debug.LogError("response " + response.Text);
			
			if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
			{
				onNetworkError?.Invoke(response);
			}
			else if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
			{
				onError?.Invoke(response);
			}
			else
			{
				onSuccess?.Invoke(response);
			}
		}

		public void Abort(IHttpRequest request)
		{
			var unityHttpRequest = request as UnityHttpRequest;
			if (unityHttpRequest?.UnityWebRequest != null && !unityHttpRequest.UnityWebRequest.isDone)
			{
				unityHttpRequest.UnityWebRequest.Abort();
			}
		}

		private static HttpResponse CreateResponse(UnityWebRequest unityWebRequest)
		{
			return new HttpResponse
			{
				Url = unityWebRequest.url,
				Bytes = unityWebRequest.downloadHandler?.data,
				Text = unityWebRequest.downloadHandler?.text,
				IsSuccessful = (unityWebRequest.result != UnityWebRequest.Result.ProtocolError) && (unityWebRequest.result != UnityWebRequest.Result.ConnectionError),
				IsHttpError = (unityWebRequest.result == UnityWebRequest.Result.ProtocolError),
				IsNetworkError = (unityWebRequest.result == UnityWebRequest.Result.ConnectionError),
				Error = unityWebRequest.error,
				StatusCode = unityWebRequest.responseCode,
				ResponseHeaders = unityWebRequest.GetResponseHeaders(),
				Texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture)?.texture
			};
		}
	}
}

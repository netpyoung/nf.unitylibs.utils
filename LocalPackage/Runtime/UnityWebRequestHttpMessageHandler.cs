using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NF.UnityLibs.Utils
{
    // ref: https://gist.github.com/neuecc/854192b8d176170caf2c53fa7589dc90
    public sealed class UnityWebRequestHttpMessageHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            using (UnityWebRequest uwr = new UnityWebRequest(requestMessage.RequestUri, requestMessage.Method.ToString()))
            {
                UploadHandlerRaw uploadHandler = new UploadHandlerRaw(await requestMessage.Content.ReadAsByteArrayAsync());
                uploadHandler.contentType = requestMessage.Headers.Accept.ToString();
                uwr.uploadHandler = uploadHandler;

                DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
                uwr.downloadHandler = downloadHandler;

                foreach (KeyValuePair<string, IEnumerable<string>> header in requestMessage.Headers)
                {
                    uwr.SetRequestHeader(header.Key, string.Join(",", header.Value));
                }

                CancellationTokenRegistration cancellationTokenRegistration = default;
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.Register(static state =>
                    {
                        UnityWebRequest req = (UnityWebRequest)state;
                        req.Abort();
                        req.Dispose();
                    }, uwr);
                }

                using (cancellationTokenRegistration)
                {
                    await new AwaitableAsyncOperation(uwr.SendWebRequest());
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (uwr.result == UnityWebRequest.Result.ConnectionError)
                {
                    throw new InvalidOperationException($"UnityWebRequest.SendWebRequest is failed, Result: {uwr.result}");
                }

                HttpResponseMessage response = new HttpResponseMessage((HttpStatusCode)uwr.responseCode)
                {
                    RequestMessage = requestMessage,
                    Content = new ByteArrayContent(downloadHandler.data),
                    Version = HttpVersion.Version10
                };

                HttpResponseHeaders responseHeader = response.Headers;
                foreach (KeyValuePair<string, string> header in uwr.GetResponseHeaders())
                {
                    responseHeader.TryAddWithoutValidation(header.Key, header.Value);
                }

                return response;
            }
        }

        private readonly struct AwaitableAsyncOperation
        {
            readonly AsyncOperation _operation;

            public AwaitableAsyncOperation(AsyncOperation operation)
            {
                _operation = operation;
            }

            public AsyncOperationAwaiter GetAwaiter()
            {
                return new AsyncOperationAwaiter(_operation);
            }
        }

        private struct AsyncOperationAwaiter : ICriticalNotifyCompletion
        {
            private AsyncOperation? _asyncOperationOrNull;
            private Action<AsyncOperation>? _continuationActionOrNull;
            public bool IsCompleted => _asyncOperationOrNull!.isDone;

            public AsyncOperationAwaiter(AsyncOperation? asyncOperationOrNull)
            {
                _asyncOperationOrNull = asyncOperationOrNull;
                _continuationActionOrNull = null;
            }


            public void GetResult()
            {
                if (_continuationActionOrNull != null)
                {
                    _asyncOperationOrNull!.completed -= _continuationActionOrNull;
                    _continuationActionOrNull = null;
                    _asyncOperationOrNull = null;
                }
                else
                {
                    _asyncOperationOrNull = null;
                }
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (_continuationActionOrNull != null)
                {
                    throw new InvalidOperationException("continuation is already registered.");
                }

                _continuationActionOrNull = (_) =>
                {
                    continuation();
                };

                _asyncOperationOrNull!.completed += _continuationActionOrNull;
            }
        }
    }
}
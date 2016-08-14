using System;
using System.Net;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public static class WebRequestExtension
    {
        public static Task<IAsyncResult> GetResponseExtendedAsync(this WebRequest webRequest)
        {
            var taskCompletionSource = new TaskCompletionSource<IAsyncResult>();

            EventHandler eventHandler = (sender, args) =>
            {
                try
                {
                    var webRequestGetResponseAwaiterSender = (WebRequestGetResponseAwaiter)sender;
                    taskCompletionSource.SetResult(webRequestGetResponseAwaiterSender.Result);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            };

            var webRequestGetResponseAwaiter = new WebRequestGetResponseAwaiter(webRequest);
            webRequestGetResponseAwaiter.GetResponseCompleted += eventHandler;
            webRequestGetResponseAwaiter.GetResponseAsync();

            return taskCompletionSource.Task;
        }
    }
}

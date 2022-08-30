using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Nyris.Api.Utils
{
    internal class HttpRetryHandler : DelegatingHandler
    {
        private const int NumberRetry = Constants.DefaultHttpRetryCount;

        public HttpRetryHandler(HttpClientHandler httpClientHandler)
            : base(httpClientHandler ?? new HttpClientHandler())
        {
        }

        [NotNull, ItemNotNull]
        protected override async Task<HttpResponseMessage> SendAsync(
            [NotNull] HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await OnRequestPreparingAsync(request).ConfigureAwait(false);

            var start = DateTime.Now;
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var retried = 0;
            while (!response.IsSuccessStatusCode && retried < NumberRetry)
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                retried++;
            }

            var end = DateTime.Now;

            await OnRequestFinishedAsync(end, start, request, response).ConfigureAwait(false);
            return response;
        }

        [NotNull]
        protected virtual Task OnRequestPreparingAsync([NotNull] HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }

        [NotNull]
        protected virtual Task OnRequestFinishedAsync(DateTime end, DateTime start,
            [NotNull] HttpRequestMessage request,
            [NotNull] HttpResponseMessage response)
        {
            return Task.CompletedTask;
        }
    }
}

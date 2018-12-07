using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Io.Nyris.Sdk.Utils
{
    public class HttpLoggingAndRetryHandler : DelegatingHandler
    {
        readonly string[] _types = new[] {"html", "text", "xml", "json", "txt", "x-www-form-urlencoded"};
        private readonly string _id;
        private readonly int _numberRetry;
        private readonly bool _isDebug;

        public HttpLoggingAndRetryHandler(string id, int numberRetry, bool isDebug) : base(new HttpClientHandler())
        {
            _id = id;
            _numberRetry = numberRetry;
            _isDebug = isDebug;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            PrintHeader(request);

            var start = DateTime.Now;
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var retried = 0;
            while (!response.IsSuccessStatusCode && retried < _numberRetry)
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                retried++;
            }

            var end = DateTime.Now;

            PrinterFooter(end, start, request, response);

            return response;
        }

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out var values))
                return false;
            var header = string.Join(" ", values).ToLowerInvariant();

            return _types.Any(t => header.Contains(t));
        }

        private async void PrintHeader(HttpRequestMessage request)
        {
            if (!_isDebug)
            {
                return;
            }

            var msg = $"[{_id} - Request]";

            Debug.WriteLine($"{msg}========Start==========");
            Debug.WriteLine(
                $"{msg} {request.Method} {request.RequestUri.PathAndQuery} {request.RequestUri.Scheme}/{request.Version}");
            Debug.WriteLine($"{msg} Host: {request.RequestUri.Scheme}://{request.RequestUri.Host}");

            foreach (var header in request.Headers)
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (request.Content == null)
            {
                return;
            }

            foreach (var header in request.Content.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (!(request.Content is StringContent) && !IsTextBasedContentType(request.Headers) &&
                !IsTextBasedContentType(request.Content.Headers))
            {
                return;
            }

            var result = await request.Content.ReadAsStringAsync();
            Debug.WriteLine($"{msg} Content:");
            Debug.WriteLine($"{msg} {string.Join("", result.Take(255))}...");
        }

        private async void PrinterFooter(DateTime end, DateTime start, HttpRequestMessage request,
            HttpResponseMessage response)
        {
            var msg = $"[{_id} - Request]";
            Debug.WriteLine($"{msg} Duration: {end - start}");
            Debug.WriteLine($"{msg}==========End==========");

            Debug.WriteLine("");
            msg = $"[{_id} - Response]";
            Debug.WriteLine($"{msg}=========Start=========");

            Debug.WriteLine(
                $"{msg} {request.RequestUri.Scheme.ToUpper()}/{response.Version} {(int) response.StatusCode} {response.ReasonPhrase}");

            foreach (var header in response.Headers)
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                    Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

                if (response.Content is StringContent || IsTextBasedContentType(response.Headers) ||
                    IsTextBasedContentType(response.Content.Headers))
                {
                    start = DateTime.Now;
                    var result = await response.Content.ReadAsStringAsync();
                    end = DateTime.Now;

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {string.Join("", result.Take(255))}...");
                    Debug.WriteLine($"{msg} Duration: {end - start}");
                }
            }

            Debug.WriteLine($"{msg}==========End==========");
        }
    }
}
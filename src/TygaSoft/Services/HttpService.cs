using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using TygaSoft.IServices;
using TygaSoft.Model;
using System.Net.Http;

namespace TygaSoft.Services
{
    public class HttpService:IHttpService
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient();
            return await client.SendAsync(request, cancellationToken);
        }

        public async Task<NetResponse> GetResponseAsync(HttpRequestMessage request)
        {
            var netResponse = new NetResponse();

            var response = await SendAsync(request, CancellationToken.None);

            await ParseToNetResponse(netResponse, response);

            return netResponse;
        }

        private async Task ParseToNetResponse(NetResponse netResponse, HttpResponseMessage response)
        {
            using (response)
            {
                netResponse.IsSuccessful = response.IsSuccessStatusCode;
                netResponse.ContentEncoding = string.Join(",", response.Content.Headers.ContentEncoding);
                netResponse.ContentType = response.Content.Headers.ContentType?.MediaType;
                netResponse.ContentLength = response.Content.Headers.ContentLength;
                netResponse.ResponseUri = response.RequestMessage.RequestUri;
                var cookies = response.Headers?.Where(m => m.Key.ToLower() == "set-cookie");
                if (cookies != null && cookies.Any())
                {
                    var items = new List<string>();
                    foreach (var item in cookies)
                    {
                        items.AddRange(item.Value.Select(m => m.Split(';')[0]));
                    }
                    netResponse.CookieAppend = string.Join(";", items);
                }

                if (response.Content.Headers.ContentEncoding.Any(m => m == "gzip"))
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var decompressed = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        netResponse.RawBytes = await decompressed.AsBytes();
                    }
                }
                else
                {
                    netResponse.RawBytes = await response.Content.ReadAsByteArrayAsync();
                }
            }
        }
    }
}
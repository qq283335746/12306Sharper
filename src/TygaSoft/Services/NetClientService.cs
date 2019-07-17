using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TygaSoft.IServices;
using TygaSoft.Model;

namespace TygaSoft.Services
{
    public class NetClientService : INetClientService
    {
        private readonly IHttpService _httpService;
        public NetClientService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<NetResponse> ExecuteAsync(NetRequest netRequest)
        {
            _isDefaultContentType = netRequest.Parameters.Any(m => m.ParamsOptions == ParameterOptions.HttpContentHeader && m.Name == HttpR.ContentTypeKey && m.Value == HttpR.ContentType);

            var request = new HttpRequestMessage();

            switch (netRequest.Method)
            {
                case HttpMethodOptions.Get:
                    request.Method = HttpMethod.Get;
                    break;
                case HttpMethodOptions.Post:
                    request.Method = HttpMethod.Post;
                    if (!_isDefaultContentType) _isDefaultContentType = !netRequest.Parameters.Any(m => m.ParamsOptions == ParameterOptions.HttpContentHeader && m.Name == HttpR.ContentTypeKey);
                    break;
                default:
                    request.Method = HttpMethod.Get;
                    break;
            }

            if (!string.IsNullOrEmpty(netRequest.Resource)) request.RequestUri = new Uri(netRequest.Resource);

            PrepareRequest(netRequest, request);

            return await _httpService.GetResponseAsync(request);
        }

        private bool _isDefaultContentType;

        private void PrepareRequest(NetRequest netRequest, HttpRequestMessage request)
        {
            if (!netRequest.Parameters.Any()) return;

            AppendContent(netRequest, request);
            AppendHeaders(netRequest, request);
            AppendCookies(netRequest, request);
        }

        private void AppendHeaders(NetRequest netRequest, HttpRequestMessage request)
        {
            var headers = netRequest.Parameters.Where(m => m.ParamsOptions == ParameterOptions.HttpHeader || m.ParamsOptions == ParameterOptions.HttpContentHeader);
            if (headers == null || !headers.Any()) return;

            foreach (var item in headers)
            {
                switch (item.ParamsOptions)
                {
                    case ParameterOptions.HttpHeader:
                        switch (item.Name)
                        {
                            case HttpR.AcceptKey:
                                request.Headers.Accept.TryParseAdd(item.Value);
                                break;
                            case HttpR.AcceptEncodingKey:
                                request.Headers.AcceptEncoding.TryParseAdd(item.Value);
                                break;
                            case HttpR.AcceptLanguageKey:
                                request.Headers.AcceptLanguage.TryParseAdd(item.Value);
                                break;
                            case HttpR.UserAgentKey:
                                request.Headers.UserAgent.TryParseAdd(item.Value);
                                break;
                            case HttpR.RefererKey:
                                request.Headers.Referrer = new Uri(item.Value);
                                break;
                            default:
                                request.Headers.TryAddWithoutValidation(item.Name, item.Value);
                                break;
                        }
                        break;
                    case ParameterOptions.HttpContentHeader:
                        request.Content.Headers.TryAddWithoutValidation(item.Name, item.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AppendCookies(NetRequest netRequest, HttpRequestMessage request)
        {
            var items = netRequest.Parameters.Where(m => m.ParamsOptions == ParameterOptions.Cookie);
            if (items == null || !items.Any()) return;

            var sCookie = string.Join(";", items.Select(m => m.ToString()));

            if (request.Headers.TryGetValues(HttpR.CookieKey, out var values))
            {
                var cookieValue = values.First().Trim(';');
                request.Headers.Remove(HttpR.CookieKey);
                request.Headers.Add(HttpR.CookieKey, string.Format("{0}{1}{2}", cookieValue, string.IsNullOrEmpty(cookieValue) ? "" : ";", sCookie));

            }
            else
            {
                request.Headers.Add(HttpR.CookieKey, sCookie);
            }
        }

        private void AppendContent(NetRequest netRequest, HttpRequestMessage request)
        {
            if (netRequest.Method == HttpMethodOptions.Post)
            {
                if (_isDefaultContentType)
                {
                    var formItems = netRequest.Parameters.Where(m=>m.ParamsOptions == ParameterOptions.FormUrlEncodedContent);
                    if(formItems != null && formItems.Any()) request.Content = new FormUrlEncodedContent(formItems.ToNvcs());
                }
            }

            var items = netRequest.Parameters.Where(m => m.ParamsOptions == ParameterOptions.GetOrPost);
            if (items == null || !items.Any()) return;

            request.Content = new ByteArrayContent(items.ToParameterString().AsBytes());
        }
    }
}
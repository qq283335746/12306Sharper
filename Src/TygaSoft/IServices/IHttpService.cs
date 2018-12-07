using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TygaSoft.Model;

namespace TygaSoft.IServices
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);

        Task<NetResponse> GetResponseAsync(HttpRequestMessage request);
    }
}
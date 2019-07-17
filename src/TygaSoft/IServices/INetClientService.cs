using System.Threading.Tasks;
using TygaSoft.Model;

namespace TygaSoft.IServices
{
    public interface INetClientService
    {
        Task<NetResponse> ExecuteAsync(NetRequest netRequest);
    }
}
using System.Threading.Tasks;
using TygaSoft.Model;

namespace TygaSoft.IServices
{
    public interface IRailwayService
    {
        string GetHelloAsync();

        Task Execute12306cnAsync(Execute12306cnInfo execute12306cnInfo);
    }
}
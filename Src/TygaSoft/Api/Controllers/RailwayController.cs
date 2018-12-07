using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TygaSoft.Model;
using TygaSoft.IServices;
using TygaSoft.Services;

namespace TygaSoft.Api.Controllers
{
    public class RailwayController : Controller
    {
        private readonly IRailwayService _railwayService;

        public RailwayController(IRailwayService railwayService)
        {
            _railwayService = railwayService;
        }

        public HelloResult GetHelloAsync()
        {
            return new HelloResult { ResCode = ResCodeOptions.Success, Data = _railwayService.GetHelloAsync() };
        }

        public async Task<Result> Execute12306cnAsync([FromBody]Execute12306cnInfo execute12306cnInfo)
        {
            try
            {
                await _railwayService.Execute12306cnAsync(execute12306cnInfo);
                return new Result { ResCode = ResCodeOptions.Success};
            }
            catch (Exception ex)
            {
                return new Result { ResCode = ResCodeOptions.Error, Message = ex.Message };
            }

        }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TygaSoft.IServices;
using TygaSoft.Model;

namespace TygaSoft.Services
{
    public class RailwayService : IRailwayService
    {
        private readonly INetClientService _netClientService;

        public RailwayService(INetClientService netClientService)
        {
            _netClientService = netClientService;
        }

        private const string _submitOrderRequest = "https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest";

        //示例：https://kyfw.12306.cn/otn/leftTicket/init?linktypeid=dc&fs=%E6%B7%B1%E5%9C%B3,SZQ&ts=%E9%95%BF%E6%B2%99,CSQ&date=2018-12-04&flag=N,N,Y
        private const string _otnLeftTicketInit = "https://kyfw.12306.cn/otn/leftTicket/init?linktypeid={0}&fs={1}&ts={2}&date={3}&flag={4}";

        // 示例：https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2018-12-04&leftTicketDTO.from_station=SZQ&leftTicketDTO.to_station=CSQ&purpose_codes=ADULT
        private const string _otnLeftTicketQuery = "https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes={3}";

        private Execute12306cnInfo _execute12306cnInfo;

        public string GetHelloAsync()
        {
            return "RailwayService--Hello word";
        }

        public async Task<string> Get12306cnIndex()
        {
            var request = new NetRequest("https://www.12306.cn/index/");

            var res = await _netClientService.ExecuteAsync(request);

            return res.Content;
        }

        /// 示例：https://kyfw.12306.cn/otn/leftTicket/init?linktypeid=dc&fs=%E6%B7%B1%E5%9C%B3,SZQ&ts=%E9%95%BF%E6%B2%99,CSQ&date=2018-12-27&flag=N,N,Y
        // public async Task<string> Get12306cnOtnLeftTicketInit(string linktypeid, string fromStation, string toStation, string date, string flag)
        // {

        // }

        public async Task Execute12306cnAsync(Execute12306cnInfo execute12306cnInfo)
        {
            _execute12306cnInfo = execute12306cnInfo;

            var userOrderInfo = GetUserOrderInfo(execute12306cnInfo.UserName);
            var otnLeftTicketQueryInfo = new OtnLeftTicketQueryInfo { PurposeCode = userOrderInfo.PurposeCode, Date = userOrderInfo.RideDate, FromStation = userOrderInfo.FromStationCode, ToStation = userOrderInfo.ToStationCode };
            otnLeftTicketQueryInfo.Referer = string.Format(_otnLeftTicketInit, userOrderInfo.TourFlag, string.Format("{0},{1}", userOrderInfo.FromStationName, userOrderInfo.FromStationCode), string.Format("{0},{1}", userOrderInfo.ToStationName, userOrderInfo.ToStationCode), userOrderInfo.RideDate, "N,N,Y");
            var otnLeftTicketQueryResult = await OtnLeftTicketQueryAsync(otnLeftTicketQueryInfo);

            Console.WriteLine("otnLeftTicketQueryResult--{0}", otnLeftTicketQueryResult == null ? null : JsonConvert.SerializeObject(otnLeftTicketQueryResult));

            if (otnLeftTicketQueryResult == null)
            {
                return;
            }
            var tickets = ParseToTickets(otnLeftTicketQueryResult);
            if (tickets == null || !tickets.Any())
            {
                return;
            }

            var firstTicketInfo = tickets.First();
            var submitOrderRequestInfo = new SubmitOrderRequestInfo { SecretStr = firstTicketInfo.SecretStr, TrainDate = userOrderInfo.RideDate, BackTrainDate = userOrderInfo.BackRideDate, FromStation = userOrderInfo.FromStationName, ToStation = userOrderInfo.ToStationName, TourFlag = userOrderInfo.TourFlag, PurposeCode = userOrderInfo.PurposeCode };
            submitOrderRequestInfo.Referer = otnLeftTicketQueryInfo.Referer;
            var submitOrderRequestResult = await SubmitOrderRequestAsync(submitOrderRequestInfo);
        }

        public UserOrderInfo GetUserOrderInfo(string userName)
        {
            return new UserOrderInfo { RideDate = "2018-12-010", BackRideDate = DateTime.Now.ToString("yyyy-MM-dd"), FromStationCode = "SZQ", FromStationName = "深圳", ToStationCode = "CSQ", ToStationName = "长沙", TourFlag = "dc", PurposeCode = PurposeOptions.ADULT.ToString() };
        }

        /// 示例：https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2018-12-04&leftTicketDTO.from_station=SZQ&leftTicketDTO.to_station=CSQ&purpose_codes=ADULT
        public async Task<OtnLeftTicketQueryResult> OtnLeftTicketQueryAsync(OtnLeftTicketQueryInfo reqInfo)
        {
            var url = string.Format(_otnLeftTicketQuery, reqInfo.Date, reqInfo.FromStation, reqInfo.ToStation, reqInfo.PurposeCode);

            var request = CreateRequest(url, reqInfo.Referer,HttpMethodOptions.Get);

            var res = await _netClientService.ExecuteAsync(request);

            OtnLeftTicketQueryResult otnLeftTicketQueryResult = null;
            try
            {
                otnLeftTicketQueryResult = JsonConvert.DeserializeObject<OtnLeftTicketQueryResult>(res.Content);
            }
            catch
            {

            }

            return otnLeftTicketQueryResult;
        }

        public async Task<SubmitOrderRequestResult> SubmitOrderRequestAsync(SubmitOrderRequestInfo reqInfo)
        {
            var request = CreateRequest(_submitOrderRequest, reqInfo.Referer,HttpMethodOptions.Post);
            //request.Method = MethodOptions.Post;
            //request.AddParameter(string.Empty, string.Empty, "application/x-www-form-urlencoded", ParameterType.HttpHeader);
            request.AddParameter("secretStr", reqInfo.SecretStr);
            request.AddParameter("train_date", reqInfo.TrainDate);
            request.AddParameter("back_train_date", reqInfo.BackTrainDate);
            request.AddParameter("tour_flag", reqInfo.TourFlag);
            request.AddParameter("purpose_codes", reqInfo.PurposeCode);
            request.AddParameter("query_from_station_name", reqInfo.FromStation);
            request.AddParameter("query_to_station_name", reqInfo.ToStation);
            request.AddParameter("undefined", string.Empty);

            var res = await _netClientService.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<SubmitOrderRequestResult>(res.Content);
        }

        private NetRequest CreateRequest(string baseUrl, string referer, HttpMethodOptions methodOptions)
        {
            var request = new NetRequest(baseUrl,methodOptions);
            request.AddParameter(HttpR.CookieKey, _execute12306cnInfo.Cookie, ParameterOptions.HttpHeader);
            request.AddParameter(HttpR.RefererKey, referer, ParameterOptions.HttpHeader);
            request.AddParameter(HttpR.UserAgentKey, HttpR.UserAgent, ParameterOptions.HttpHeader);
            request.AddParameter(HttpR.AcceptKey, HttpR.Accept, ParameterOptions.HttpHeader);
            request.AddParameter(HttpR.AcceptEncodingKey, HttpR.AcceptEncoding, ParameterOptions.HttpHeader);
            request.AddParameter(HttpR.AcceptLanguageKey, HttpR.AcceptLanguage, ParameterOptions.HttpHeader);
            request.AddParameter(HttpR.XRequestedWithKey, HttpR.XRequestedWith, ParameterOptions.HttpHeader);

            return request;
        }

        private IEnumerable<RailwayTicketInfo> ParseToTickets(OtnLeftTicketQueryResult model)
        {
            var tickets = new List<RailwayTicketInfo>();

            if (model == null || model.data == null || !model.data.result.Any()) return tickets;

            foreach (var item in model.data.result)
            {
                var itemArr = item.ToArray1();

                tickets.Add(new RailwayTicketInfo { SecretStr = itemArr[0], Coded = itemArr[3] });
            }

            return tickets;
        }
    }
}
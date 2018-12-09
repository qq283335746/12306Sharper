using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TygaSoft.IServices;
using TygaSoft.Model;
using System.Text.RegularExpressions;

namespace TygaSoft.Services
{
    public class RailwayService : IRailwayService
    {
        private readonly INetClientService _netClientService;

        public RailwayService(INetClientService netClientService)
        {
            _netClientService = netClientService;
        }

        //示例：https://kyfw.12306.cn/otn/leftTicket/init?linktypeid=dc&fs=%E6%B7%B1%E5%9C%B3,SZQ&ts=%E9%95%BF%E6%B2%99,CSQ&date=2018-12-04&flag=N,N,Y
        private const string _otnLeftTicketInit = "https://kyfw.12306.cn/otn/leftTicket/init?linktypeid={0}&fs={1}&ts={2}&date={3}&flag={4}";

        // 示例：https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2018-12-04&leftTicketDTO.from_station=SZQ&leftTicketDTO.to_station=CSQ&purpose_codes=ADULT
        private const string _otnLeftTicketQuery = "https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes={3}";

        //POST
        private const string _submitOrderRequest = "https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest";

        //POST
        private const string _otnConfirmPassengerInitDc = "https://kyfw.12306.cn/otn/confirmPassenger/initDc";

        //POST
        private const string _otnConfirmPassengerDTOs = "https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs";

        //POST
        private const string _otnConfirmCheckOrderInfoUrl = "https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo";

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
            if (otnLeftTicketQueryResult == null)
            {
                return;
            }
            var tickets = ParseToTickets(otnLeftTicketQueryResult.data);
            if (tickets == null || !tickets.Any())
            {
                return;
            }

            var firstTicketInfo = tickets.First();
            var submitOrderRequestInfo = new SubmitOrderRequestInfo { SecretStr = firstTicketInfo.SecretStr, TrainDate = userOrderInfo.RideDate, BackTrainDate = userOrderInfo.BackRideDate, FromStation = userOrderInfo.FromStationName, ToStation = userOrderInfo.ToStationName, TourFlag = userOrderInfo.TourFlag, PurposeCode = userOrderInfo.PurposeCode };
            submitOrderRequestInfo.Referer = otnLeftTicketQueryInfo.Referer;
            
            var submitOrderRequestResult = await SubmitOrderRequestAsync(submitOrderRequestInfo);
            if (submitOrderRequestResult == null)
            {
                return;
            }

            var confirmPassengerInitDcResult = await OtnConfirmPassengerInitDcAsync(otnLeftTicketQueryInfo.Referer);
            if (confirmPassengerInitDcResult == null)
            {
                return;
            }

            var confirmPassengerDTOsResult = await ConfirmPassengerDTOsAsync(confirmPassengerInitDcResult.GlobalRepeatSubmitToken);
            if (confirmPassengerDTOsResult == null)
            {
                Console.WriteLine("ConfirmPassengerDTOsAsync,confirmPassengerDTOsResult is null");
                return;
            }
            var passengerInfo = confirmPassengerDTOsResult.data.normal_passengers.FirstOrDefault();
            if (passengerInfo == null)
            {
                Console.WriteLine("ConfirmPassengerDTOsAsync,passengerInfo is null");
                return;
            }
            var confirmPassengerCheckOrderInfo = new ConfirmPassengerCheckOrderInfo { RepeatSubmitToken = confirmPassengerInitDcResult.GlobalRepeatSubmitToken, JsonAtt = string.Empty, BedLevelOrderNum = "000000000000000000000000000000", CancelFlag = 2, OldPassengerStr = Enum12306Datas.OldPassengerStrFormat(passengerInfo), PassengerTicketStr = Enum12306Datas.PassengerTicketStrFormat(passengerInfo), TourFlag = userOrderInfo.TourFlag, WhatsSelect = 1 };
            confirmPassengerCheckOrderInfo.Referer = _otnConfirmPassengerInitDc;
            var isequar = Guid.Empty.ToString("N").Equals(confirmPassengerCheckOrderInfo.BedLevelOrderNum);
            var confirmPassengerCheckOrderResult = await ConfirmPassengerCheckOrderAsync(confirmPassengerCheckOrderInfo);
        }

        public UserOrderInfo GetUserOrderInfo(string userName)
        {
            return new UserOrderInfo { RideDate = "2018-12-10", BackRideDate = DateTime.Now.ToString("yyyy-MM-dd"), FromStationCode = "SZQ", FromStationName = "深圳", ToStationCode = "CSQ", ToStationName = "长沙", TourFlag = "dc", PurposeCode = PurposeOptions.ADULT.ToString() };
        }

        /// 示例：https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2018-12-04&leftTicketDTO.from_station=SZQ&leftTicketDTO.to_station=CSQ&purpose_codes=ADULT
        public async Task<Base12306cnResult<OtnLeftTicketQueryResult>> OtnLeftTicketQueryAsync(OtnLeftTicketQueryInfo reqInfo)
        {
            var url = string.Format(_otnLeftTicketQuery, reqInfo.Date, reqInfo.FromStation, reqInfo.ToStation, reqInfo.PurposeCode);

            var request = CreateRequest(url, reqInfo.Referer, HttpMethodOptions.Get);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("OtnLeftTicketQueryAsync,res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            return res.Content.ToModel<Base12306cnResult<OtnLeftTicketQueryResult>>();
        }

        public async Task<Base12306cnResult<SubmitOrderRequestResult>> SubmitOrderRequestAsync(SubmitOrderRequestInfo reqInfo)
        {
            var request = CreateRequest(_submitOrderRequest, reqInfo.Referer, HttpMethodOptions.Post);
            //request.Method = MethodOptions.Post;
            request.AddParameter(HttpR.ContentTypeKey, HttpR.ContentType, ParameterOptions.HttpContentHeader);
            request.AddParameter("secretStr", reqInfo.SecretStr);
            //request.AddParameter("train_date", reqInfo.TrainDate);
            request.AddParameter("back_train_date", reqInfo.BackTrainDate);
            request.AddParameter("tour_flag", reqInfo.TourFlag);
            request.AddParameter("purpose_codes", reqInfo.PurposeCode);
            request.AddParameter("query_from_station_name", reqInfo.FromStation);
            request.AddParameter("query_to_station_name", reqInfo.ToStation);
            request.AddParameter("undefined", string.Empty);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("SubmitOrderRequestAsync,res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            return res.Content.ToModel<Base12306cnResult<SubmitOrderRequestResult>>();
        }

        public async Task<ConfirmPassengerInitDcResult> OtnConfirmPassengerInitDcAsync(string referer)
        {
            var request = CreateRequest(_otnConfirmPassengerInitDc, referer, HttpMethodOptions.Post);
            request.AddParameter("_json_att", string.Empty);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("OtnConfirmPassengerInitDcAsync--res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            ConfirmPassengerInitDcResult confirmPassengerInitDcResult = null;

            var grstPatternMatch = Regex.Match(res.Content, ConfirmPassengerInitDcResult.GlobalRepeatSubmitTokenPattern);
            if (grstPatternMatch != null && !string.IsNullOrEmpty(grstPatternMatch.Value))
            {
                if (confirmPassengerInitDcResult == null) confirmPassengerInitDcResult = new ConfirmPassengerInitDcResult();
                confirmPassengerInitDcResult.GlobalRepeatSubmitToken = grstPatternMatch.Value.Substring(grstPatternMatch.Value.LastIndexOf("=") + 1).Trim().Trim(new char[] { ';', '\'' });
            }

            return confirmPassengerInitDcResult;
        }

        public async Task<Base12306cnResult<ConfirmPassengerDTOsResult>> ConfirmPassengerDTOsAsync(string repeatSubmitToken)
        {
            var request = CreateRequest(_otnConfirmPassengerDTOs, _otnConfirmPassengerInitDc, HttpMethodOptions.Post);
            request.AddParameter("_json_att", string.Empty);
            request.AddParameter("REPEAT_SUBMIT_TOKEN", repeatSubmitToken);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("ConfirmPassengerDTOsAsync--res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            return res.Content.ToModel<Base12306cnResult<ConfirmPassengerDTOsResult>>();
        }

        public async Task<Base12306cnResult<ConfirmPassengerCheckOrderResult>> ConfirmPassengerCheckOrderAsync(ConfirmPassengerCheckOrderInfo reqInfo)
        {
            var request = CreateRequest(_otnConfirmCheckOrderInfoUrl, reqInfo.Referer, HttpMethodOptions.Post);
            request.AddParameter("REPEAT_SUBMIT_TOKEN", reqInfo.RepeatSubmitToken, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("_json_att", reqInfo.JsonAtt, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("bed_level_order_num", reqInfo.BedLevelOrderNum, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("cancel_flag", reqInfo.CancelFlag.ToString(), ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("oldPassengerStr", reqInfo.OldPassengerStr, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("passengerTicketStr", reqInfo.PassengerTicketStr, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("randCode", reqInfo.RandCode, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("tour_flag", reqInfo.TourFlag, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("whatsSelect", reqInfo.WhatsSelect.ToString(), ParameterOptions.FormUrlEncodedContent);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("ConfirmPassengerCheckOrderAsync,res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            return res.Content.ToModel<Base12306cnResult<ConfirmPassengerCheckOrderResult>>();
        }

        private NetRequest CreateRequest(string baseUrl, string referer, HttpMethodOptions methodOptions)
        {
            var request = new NetRequest(baseUrl, methodOptions);
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

            if (model == null || !model.result.Any()) return tickets;

            foreach (var item in model.result)
            {
                var itemArr = item.ToArray1();

                tickets.Add(new RailwayTicketInfo { SecretStr = itemArr[0], Coded = itemArr[3] });
            }

            return tickets;
        }
    }
}
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

        private Execute12306cnInfo _execute12306cnInfo;

        public string GetHelloAsync()
        {
            return "RailwayService--Hello word";
        }

        public async Task Execute12306cnAsync(Execute12306cnInfo execute12306cnInfo)
        {
            _execute12306cnInfo = execute12306cnInfo;

            var userOrderInfo = GetUserOrderInfo(execute12306cnInfo.UserName);
            var otnLeftTicketQueryInfo = new OtnLeftTicketQueryInfo 
            { 
                PurposeCode = userOrderInfo.PurposeCode, 
                Date = userOrderInfo.RideDate, 
                FromStation = userOrderInfo.FromStationCode, 
                ToStation = userOrderInfo.ToStationCode 
            };
            otnLeftTicketQueryInfo.Referer = string.Format(UrlsIn12306cn._otnLeftTicketInitUrl, userOrderInfo.TourFlag, string.Format("{0},{1}", userOrderInfo.FromStationName, userOrderInfo.FromStationCode), string.Format("{0},{1}", userOrderInfo.ToStationName, userOrderInfo.ToStationCode), userOrderInfo.RideDate, "N,N,Y");

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

            var ticketInfo = tickets.First();
            var submitOrderRequestInfo = new SubmitOrderRequestInfo 
            { 
                SecretStr = ticketInfo.SecretStr, 
                TrainDate = userOrderInfo.RideDate, 
                BackTrainDate = userOrderInfo.BackRideDate, 
                FromStation = userOrderInfo.FromStationName, 
                ToStation = userOrderInfo.ToStationName, 
                TourFlag = userOrderInfo.TourFlag, 
                PurposeCode = userOrderInfo.PurposeCode,
                Referer = otnLeftTicketQueryInfo.Referer
            };

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
                return;
            }
            var passengerInfo = confirmPassengerDTOsResult.data.normal_passengers.FirstOrDefault();
            if (passengerInfo == null)
            {
                Console.WriteLine("ConfirmPassengerDTOsAsync,passengerInfo is null");
                return;
            }
            var confirmPassengerCheckOrderInfo = new ConfirmPassengerCheckOrderInfo 
            { 
                RepeatSubmitToken = confirmPassengerInitDcResult.GlobalRepeatSubmitToken, 
                JsonAtt = string.Empty, 
                BedLevelOrderNum = "000000000000000000000000000000", 
                CancelFlag = 2, 
                OldPassengerStr = Enum12306Datas.OldPassengerStrFormat(passengerInfo), 
                PassengerTicketStr = Enum12306Datas.PassengerTicketStrFormat(passengerInfo), 
                TourFlag = userOrderInfo.TourFlag, WhatsSelect = 1,
                Referer = UrlsIn12306cn._otnConfirmPassengerInitDcUrl
            };
            var confirmPassengerCheckOrderResult = await ConfirmPassengerCheckOrderAsync(confirmPassengerCheckOrderInfo);

            var confirmPassengerQueueCountInfo = new ConfirmPassengerQueueCountInfo 
            {
                //TrainDate = 
                TrainNo = ticketInfo.TrainCode,
                RepeatSubmitToken = confirmPassengerInitDcResult.GlobalRepeatSubmitToken, 
                FromStationTelecode = ticketInfo.FromStationTelecode,
                ToStationTelecode = ticketInfo.ToStationTelecode, 
                LeftTicket = confirmPassengerInitDcResult.TicketInfoForPassengerInfo.leftTicketStr,
                PurposeCode=confirmPassengerInitDcResult.TicketInfoForPassengerInfo.purpose_codes,
                SeatType = userOrderInfo.SeatType,
                TrainLocation = confirmPassengerInitDcResult.TicketInfoForPassengerInfo.train_location,
                StationTrainCode = ticketInfo.TrainNo,
                Referer = UrlsIn12306cn._otnConfirmPassengerInitDcUrl
            };
            await ConfirmPassengerQueueCount(confirmPassengerQueueCountInfo);
        }

        public UserOrderInfo GetUserOrderInfo(string userName)
        {
            return new UserOrderInfo { RideDate = "2018-12-10", BackRideDate = DateTime.Now.ToString("yyyy-MM-dd"), FromStationCode = "SZQ", FromStationName = "深圳", ToStationCode = "CSQ", ToStationName = "长沙", TourFlag = "dc", PurposeCode = PurposeOptions.ADULT.ToString(),TrainType = TrainTypeOptions.G,SeatType = Enum12306Datas.GetSeatType("二等座") };
        }

        /// 示例：https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2018-12-04&leftTicketDTO.from_station=SZQ&leftTicketDTO.to_station=CSQ&purpose_codes=ADULT
        public async Task<Base12306cnResult<OtnLeftTicketQueryResult>> OtnLeftTicketQueryAsync(OtnLeftTicketQueryInfo reqInfo)
        {
            var url = string.Format(UrlsIn12306cn._otnLeftTicketQueryUrl, reqInfo.Date, reqInfo.FromStation, reqInfo.ToStation, reqInfo.PurposeCode);

            var request = CreateRequest(url, reqInfo.Referer, HttpMethodOptions.Get);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("OtnLeftTicketQueryAsync,res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            return res.Content.ToModel<Base12306cnResult<OtnLeftTicketQueryResult>>();
        }

        public async Task<Base12306cnResult<SubmitOrderRequestResult>> SubmitOrderRequestAsync(SubmitOrderRequestInfo reqInfo)
        {
            var request = CreateRequest(UrlsIn12306cn._submitOrderRequestUrl, reqInfo.Referer, HttpMethodOptions.Post);
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
            var request = CreateRequest(UrlsIn12306cn._otnConfirmPassengerInitDcUrl, referer, HttpMethodOptions.Post);
            request.AddParameter("_json_att", string.Empty);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("OtnConfirmPassengerInitDcAsync--res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            var confirmPassengerInitDcResult = new ConfirmPassengerInitDcResult();

            var grstPatternMatch = Regex.Match(res.Content, ConfirmPassengerInitDcResult.GlobalRepeatSubmitTokenPattern);
            if (grstPatternMatch != null && !string.IsNullOrEmpty(grstPatternMatch.Value))
            {
                confirmPassengerInitDcResult.GlobalRepeatSubmitToken = grstPatternMatch.Value.Substring(grstPatternMatch.Value.LastIndexOf("=") + 1).Trim().Trim(new char[] { ';', '\'' });
            }
            var ticketInfoForPassengerFormPatternMatch = Regex.Match(res.Content, ConfirmPassengerInitDcResult.TicketInfoForPassengerFormPattern);
            if (ticketInfoForPassengerFormPatternMatch != null && !string.IsNullOrEmpty(ticketInfoForPassengerFormPatternMatch.Value))
            {
                var ticketInfoForPassengerFormJsonStr = ticketInfoForPassengerFormPatternMatch.Value.Substring(ticketInfoForPassengerFormPatternMatch.Value.LastIndexOf("=") + 1).Trim().Trim(new char[] { ';', '\'' });
                confirmPassengerInitDcResult.TicketInfoForPassengerInfo = JsonConvert.DeserializeObject<TicketInfoForPassengerFormInfo>(ticketInfoForPassengerFormJsonStr);
            }

            return confirmPassengerInitDcResult;
        }

        public async Task<Base12306cnResult<ConfirmPassengerDTOsResult>> ConfirmPassengerDTOsAsync(string repeatSubmitToken)
        {
            var request = CreateRequest(UrlsIn12306cn._otnConfirmPassengerDTOsUrl, UrlsIn12306cn._otnConfirmPassengerInitDcUrl, HttpMethodOptions.Post);
            request.AddParameter("_json_att", string.Empty);
            request.AddParameter("REPEAT_SUBMIT_TOKEN", repeatSubmitToken);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("ConfirmPassengerDTOsAsync--res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);

            return res.Content.ToModel<Base12306cnResult<ConfirmPassengerDTOsResult>>();
        }

        public async Task<Base12306cnResult<ConfirmPassengerCheckOrderResult>> ConfirmPassengerCheckOrderAsync(ConfirmPassengerCheckOrderInfo reqInfo)
        {
            var request = CreateRequest(UrlsIn12306cn._otnConfirmCheckOrderInfoUrl, reqInfo.Referer, HttpMethodOptions.Post);
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

        public async Task ConfirmPassengerQueueCount(ConfirmPassengerQueueCountInfo reqInfo)
        {
            var request = CreateRequest(UrlsIn12306cn._otnConfirmPassengerQueueCountUrl, reqInfo.Referer, HttpMethodOptions.Post);
            request.AddParameter("REPEAT_SUBMIT_TOKEN", reqInfo.RepeatSubmitToken, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("_json_att", reqInfo.JsonAtt, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("fromStationTelecode", reqInfo.FromStationTelecode, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("leftTicket", reqInfo.LeftTicket, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("purpose_codes", reqInfo.PurposeCode, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("seatType", reqInfo.SeatType, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("stationTrainCode", reqInfo.StationTrainCode, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("toStationTelecode", reqInfo.ToStationTelecode, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("train_date", DateTime.Parse(reqInfo.TrainDate).ToCst(), ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("train_location", reqInfo.TrainLocation, ParameterOptions.FormUrlEncodedContent);
            request.AddParameter("train_no", reqInfo.TrainNo, ParameterOptions.FormUrlEncodedContent);

            var res = await _netClientService.ExecuteAsync(request);

            Console.WriteLine("ConfirmPassengerQueueCount,res.ResponseUri--{0},res.Content--{1}", res.ResponseUri, res.Content);
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

                tickets.Add(new RailwayTicketInfo { SecretStr = itemArr[0], BtnText = itemArr[1], TrainCode = itemArr[2], TrainNo = itemArr[3], FromStationTelecode = itemArr[4], ToStationTelecode = itemArr[7] });
            }

            return tickets;
        }
    }
}
using System;
using System.Collections.Generic;

namespace TygaSoft.Model
{
    public class Base12306cnResult<T>
    {
        public string validateMessagesShowId { get; set; }
        public bool status { get; set; }
        public int httpstatus { get; set; }
        public IEnumerable<string> messages { get; set; }
        public T data{get;set;}
    }

    public class OtnLeftTicketQueryResult
    {
        public int flag { get; set; }

        public Dictionary<string, string> map { get; set; }

        public IEnumerable<string> result { get; set; }
    }

    public class SubmitOrderRequestResult
    {
        public string data{get;set;}
    }

    public class ConfirmPassengerDTOsResult
    {
        public bool isExist{get;set;}
        public string exMsg{get;set;}
        public IEnumerable<RailwayPassengerInfo> normal_passengers{get;set;}
    }

    public class ConfirmPassengerInitDcResult
    {
        public string GlobalRepeatSubmitToken{get;set;}
        public const string GlobalRepeatSubmitTokenPattern = @"(.*)globalRepeatSubmitToken(\s*)=(\s*)([""\'])?(.*)([""\'])?(.*)";
        public const string GlobalRepeatSubmitTokenValuePattern = @"([""\'])?(.*)([""\'])?";
    }

    public class ConfirmPassengerCheckOrderResult
    {
        public string ifShowPassCode{get;set;}
        public string canChooseBeds{get;set;}
        public string canChooseSeats{get;set;}
        public string choose_Seats{get;set;}
        public string isCanChooseMid{get;set;}
        public string ifShowPassCodeTime{get;set;}
        public bool submitStatus{get;set;}
        public string smokeStr{get;set;}
    }
}
using System.Collections.Generic;

namespace TygaSoft.Model
{
    public class SubmitOrderRequestResult
    {
        public string validateMessagesShowId{get;set;}

        public bool status{get;set;}

        public int httpstatus{get;set;}

        public string data{get;set;}

        public string messages{get;set;}
    }
}
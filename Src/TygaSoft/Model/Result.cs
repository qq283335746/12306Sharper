using System.Collections.Generic;
using TygaSoft.Model;

namespace TygaSoft.Model
{
    public class Result
    {
        public ResCodeOptions ResCode { get; set; }

        public string Message { get; set; }
    }

    public class HelloResult : Result
    {
        public string Data { get; set; }
    }
}
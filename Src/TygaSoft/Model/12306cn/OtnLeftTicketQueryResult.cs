using System.Collections.Generic;

namespace TygaSoft.Model
{
    public class OtnLeftTicketQueryResult
    {
        public int httpstatus { get; set; }

        public string messages { get; set; }

        public bool status { get; set; }

        public OtnLeftTicketQueryDataInfo data { get; set; }
    }

    public class OtnLeftTicketQueryDataInfo
    {
        public int flag { get; set; }

        public Dictionary<string, string> map { get; set; }

        public IEnumerable<string> result { get; set; }
    }
}
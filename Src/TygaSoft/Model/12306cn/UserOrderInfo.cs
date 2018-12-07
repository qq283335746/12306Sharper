namespace TygaSoft.Model
{
    public class UserOrderInfo
    {
        public string UserName { get; set; }

        public string OrderCode{get;set;}

        public string FromStationCode{get;set;}

        public string FromStationName{get;set;}

        public string ToStationCode{get;set;}

        public string ToStationName{get;set;}

        public string RideDate{get;set;}    //乘车日期

        public string BackRideDate{get;set;}    //返程日期

        public string TourFlag{get;set;}    //旅行类型

        public string PurposeCode{get;set;}    //乘客类型
    }
}
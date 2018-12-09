namespace TygaSoft.Model
{
    public class UrlsIn12306cn
    {
        //示例：https://kyfw.12306.cn/otn/leftTicket/init?linktypeid=dc&fs=%E6%B7%B1%E5%9C%B3,SZQ&ts=%E9%95%BF%E6%B2%99,CSQ&date=2018-12-04&flag=N,N,Y
        public const string _otnLeftTicketInitUrl = "https://kyfw.12306.cn/otn/leftTicket/init?linktypeid={0}&fs={1}&ts={2}&date={3}&flag={4}";

        // 示例：https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2018-12-04&leftTicketDTO.from_station=SZQ&leftTicketDTO.to_station=CSQ&purpose_codes=ADULT
        public const string _otnLeftTicketQueryUrl = "https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes={3}";

        //POST
        public const string _submitOrderRequestUrl = "https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest";

        //POST
        public const string _otnConfirmPassengerInitDcUrl = "https://kyfw.12306.cn/otn/confirmPassenger/initDc";

        //POST
        public const string _otnConfirmPassengerDTOsUrl = "https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs";

        //POST
        public const string _otnConfirmCheckOrderInfoUrl = "https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo";

        //POST
        public const string _otnConfirmPassengerQueueCountUrl = "https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount";

    }
}
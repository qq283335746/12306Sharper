namespace TygaSoft.Model
{
    public class RailwayTicketInfo
    {
        public string Coded{get;set;}

        public string SecretStr{get;set;}

        public string TrainCode{get;set;}    //车辆代号，如：6i0000D9040E

        public string FromStationTelecode{get;set;}    //始发站代码，如：IOQ

        public string ToStationTelecode{get;set;}    //终点站代码，如：CWQ

        public string StartShortTime{get;set;}    //始发时间，如：07:00

        public string EndShortTime{get;set;}    //终点时间，如：10:31

        public string TakeTimes{get;set;}    //历时，如：03:31

        public string BtnText{get;set;}    //按钮显示文本，如：预订

    }
}
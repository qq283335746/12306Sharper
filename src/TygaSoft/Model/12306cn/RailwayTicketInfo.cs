namespace TygaSoft.Model
{
    public class RailwayTicketInfo
    {
        public string SecretStr{get;set;}

        public string TrainNo{get;set;}    //车次，如：G72

        public string TrainCode{get;set;}    //车辆代号，如：6i0000D9040E

        public string FromStationTelecode{get;set;}    //始发站代码，如：IOQ

        public string ToStationTelecode{get;set;}    //终点站代码，如：CWQ

        public string StartShortTime{get;set;}    //始发时间，如：07:00

        public string EndShortTime{get;set;}    //终点时间，如：10:31

        public string TakeTimes{get;set;}    //历时，如：03:31

        public int SpecialSeatNum{get;set;}    //特等座数量

        public int FirstSeatNum{get;set;}    //一等座数量

        public int SecondSeatNum{get;set;}    //二等座数量

        public string TrainLocation{get;set;}    //如：Q6

        public string BtnText{get;set;}    //按钮显示文本，如：预订

    }
}
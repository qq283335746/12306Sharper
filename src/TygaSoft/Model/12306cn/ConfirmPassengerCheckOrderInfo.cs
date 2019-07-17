namespace TygaSoft.Model
{
    public class ConfirmPassengerCheckOrderInfo
    {
        public string Referer{get;set;}
        public string RepeatSubmitToken{get;set;}
        public string JsonAtt{get;set;}
        public string BedLevelOrderNum{get;set;}
        public int CancelFlag{get;set;}
        public string OldPassengerStr{get;set;}
        public string PassengerTicketStr{get;set;}
        public string RandCode{get;set;}
        public string TourFlag{get;set;}
        public int WhatsSelect{get;set;}
    }
}
namespace TygaSoft.Model
{
    public class ConfirmSingleForQueueInfo
    {
        public ConfirmSingleForQueueInfo() { }
        public ConfirmSingleForQueueInfo(string referer, string token, string jsonAtt, string chooseSeats, string dwAll, string keyCheckIsChange, string leftTicketStr, string oldPassengerStr, string passengerTicketStr, string purposeCode, string randCode, string roomType, string seatDetailType, string trainLocation, string whatsSelect)
        {
            Referer = referer ?? UrlsIn12306cn._otnConfirmPassengerInitDcUrl;
            RepeatSubmitToken = token;
            JsonAtt = jsonAtt ?? string.Empty;
            ChooseSeats = chooseSeats ?? string.Empty;
            DwAll = dwAll ?? "N";
            KeyCheckIsChange = KeyCheckIsChange;
            LeftTicketStr = leftTicketStr;
            OldPassengerStr = oldPassengerStr;
            PassengerTicketStr = passengerTicketStr;
            PurposeCode = purposeCode;
            RandCode = randCode;
            RoomType = roomType ?? "00";
            SeatDetailType = seatDetailType ?? "000";
            TrainLocation = trainLocation;
            whatsSelect = whatsSelect ?? "1";
        }

        public string Referer { get; set; }
        public string RepeatSubmitToken { get; set; }
        public string JsonAtt { get; set; }
        public string ChooseSeats { get; set; }
        public string DwAll { get; set; }
        public string KeyCheckIsChange { get; set; }
        public string LeftTicketStr { get; set; }
        public string OldPassengerStr { get; set; }
        public string PassengerTicketStr { get; set; }
        public string PurposeCode { get; set; }
        public string RandCode { get; set; }
        public string RoomType { get; set; }
        public string SeatDetailType { get; set; }
        public string TrainLocation { get; set; }
        public string WhatsSelect { get; set; }
    }
}
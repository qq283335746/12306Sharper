namespace TygaSoft.Model
{
    public class SubmitOrderRequestInfo
    {
        public string SecretStr{get;set;}

        public string TrainDate{get;set;}

        public string BackTrainDate{get;set;}

        public string TourFlag{get;set;}    //dc or wc

        public string PurposeCode{get;set;}    //ADULT

        public string FromStation{get;set;}

        public string ToStation{get;set;}

        public string Referer{get;set;}
    }
}
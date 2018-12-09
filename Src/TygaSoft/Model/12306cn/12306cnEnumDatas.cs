using System;
using System.Collections.Generic;
using System.Linq;

namespace TygaSoft.Model
{
    public class Enum12306Datas
    {
        public static string OldPassengerStrFormat(RailwayPassengerInfo passengerInfo)
        {
            return string.Format("{0},{1},{2},{3}_", passengerInfo.passenger_name, passengerInfo.passenger_id_type_code, passengerInfo.passenger_id_no, passengerInfo.passenger_type);
        }

        public static string PassengerTicketStrFormat(RailwayPassengerInfo passengerInfo,string submitOrderRequestDataAttr="N")
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", Enum12306Datas.GetSeatType("二等座"),passengerInfo.passenger_flag,passengerInfo.passenger_type,passengerInfo.passenger_name,passengerInfo.passenger_id_type_code,passengerInfo.passenger_id_no,passengerInfo.mobile_no,submitOrderRequestDataAttr);
        }

        public static string GetSeatType(string name)
        {
            return GetSeatTypeOptions().Last().Key;
        }

        public static Dictionary<string, string> GetSeatTypeOptions()
        {
            var datas = new Dictionary<string, string>();
            datas.Add("1", "硬座");
            datas.Add("3", "硬卧");
            datas.Add("4", "软卧");
            datas.Add("M", "一等座");
            datas.Add("O", "二等座");

            return datas;
        }
    }
}
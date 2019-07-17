using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TygaSoft.Model
{
    public enum SeatTypeOptions
    {
        [Description("未知")]
        None,
        [Description("硬座")]
        Yz = 1,
        [Description("硬卧")]
        Yw = 3,
        [Description("软卧")]
        Rw = 4,
        [Description("动卧")]
        F = 11,
        [Description("一等座")]
        M = 21,
        [Description("二等座")]
        O = 22
    }

    public enum TicketTypeOptions
    {
        [Description("成人票")]
        Adult = 1,
        [Description("儿童票")]
        Children = 2,
        [Description("学生票")]
        Student = 3,
        [Description("残军票")]
        Soldier = 4,
    }

    public enum TrainTypeOptions
    {
        [Description("普快")]
        K,
        [Description("动车")]
        D,
        [Description("高铁")]
        G
    }

    public class Enum12306Datas
    {
        public static string OldPassengerStrFormat(RailwayPassengerInfo passengerInfo)
        {
            return string.Format("{0},{1},{2},{3}_", passengerInfo.passenger_name, passengerInfo.passenger_id_type_code, passengerInfo.passenger_id_no, passengerInfo.passenger_type);
        }

        public static string PassengerTicketStrFormat(UserOrderInfo userOrderInfo, RailwayPassengerInfo passengerInfo, string submitOrderRequestDataAttr = "N")
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", userOrderInfo.SeatType.ToString(), passengerInfo.passenger_flag, passengerInfo.passenger_type, passengerInfo.passenger_name, passengerInfo.passenger_id_type_code, passengerInfo.passenger_id_no, passengerInfo.mobile_no, submitOrderRequestDataAttr);
        }

        public static IEnumerable<EnumAttrInfo> GetEnumAttrs(Type enumType)
        {
            var list = new List<EnumAttrInfo>();
            return list;
        }

        // public static string GetSeatType(string value)
        // {

        //     typeof(SeatTypeOptions).getatt
        //     return GetSeatTypeOptions().First(m => m.Value == value).Key;
        // }

        // public static Dictionary<string, string> GetSeatTypeOptions()
        // {
        //     var datas = new Dictionary<string, string>();
        //     datas.Add("1", "硬座");
        //     datas.Add("3", "硬卧");
        //     datas.Add("4", "软卧");
        //     datas.Add("M", "一等座");
        //     datas.Add("O", "二等座");
        //     datas.Add("F", "动卧");

        //     return datas;
        // }

        public static Dictionary<string, string> GetCardTypes()
        {
            var datas = new Dictionary<string, string>();
            datas.Add("1", "中国居民身份证");
            datas.Add("C", "港澳居民来往内地通行证");
            datas.Add("G", "台湾居民来往大陆通行证");
            datas.Add("B", "护照");
            datas.Add("H", "外国人永久居留身份证");

            return datas;
        }


    }
}
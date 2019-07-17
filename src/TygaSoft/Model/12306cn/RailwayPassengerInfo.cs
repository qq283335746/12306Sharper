using System;

namespace TygaSoft.Model
{
    //对应12306cn，直接json字符串转为实体类
    public class RailwayPassengerInfo
    {
        public string passenger_name { get; set; }    //姓名
        public string sex_code { get; set; }    //性别代号，如：M
        public string sex_name { get; set; }    //性别，如：男
        public DateTime born_date { get; set; }    //出生日期
        public string country_code { get; set; }    //国籍代号 如：CN
        public string passenger_id_type_code { get; set; }    //证件ID类型代号，如：1
        public string passenger_id_type_name { get; set; }    //证件ID类型名称，如：中国居民身份证
        public string passenger_id_no { get; set; }    //证件ID号码，如：身份证号码
        public string passenger_type { get; set; }    //乘客类型代号，如：1
        public string passenger_type_name { get; set; }    //乘客类型名称，如：成人
        public string passenger_flag { get; set; }    //乘客标识，如：0
        public string mobile_no { get; set; }    //手机号码
        public string phone_no { get; set; }    //电话号码
        public string email { get; set; }    //电子邮箱
        public string address { get; set; }    //现居住地址，如：广东省深圳市
        public string postalcode { get; set; }    //邮政编码
        public string first_letter { get; set; }    //姓名首字母大写
        public string recordCount { get; set; }
        public string total_times { get; set; }
        public string index_id { get; set; }
        public string gat_born_date { get; set; }
        public string gat_valid_date_start { get; set; }
        public string gat_valid_date_end { get; set; }
        public string gat_version { get; set; }
    }
}
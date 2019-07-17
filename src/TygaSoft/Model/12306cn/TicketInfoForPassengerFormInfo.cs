using System;
using System.Collections.Generic;

namespace TygaSoft.Model
{
    // 对应12306cn相关字段，将json字符串转换为该实体类
    public class TicketInfoForPassengerFormInfo
    {
        public string key_check_isChange{get;set;}    //如：0047227C13D4757297571E160101C00B3390019486424E35204D34BE
        public string leftTicketStr{get;set;}    //如：c7DKihNtkHKSHypesbUGfoD2bBgQLwHQ
        public string purpose_codes{get;set;}    //如：00
        public string train_location{get;set;}    //如：Q9
        public IEnumerable<string> leftDetails{get;set;}    //如： ['动卧(320.00元)有票', '二等座(265.00元)无票']
    }
}
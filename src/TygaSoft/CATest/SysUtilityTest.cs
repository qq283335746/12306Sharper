using System;
using TygaSoft.SysUtility;

namespace TygaSoft.CATest
{
    public class SysUtilityTest
    {
        public static void Run()
        {
            var oldTIme = SignHelper.ConvertLongToDateTime(1543955874034,0);
            var oldTime2 = SignHelper.ConvertLongToDateTime(1543665049154,0);
            var oldTs = (oldTime2 - oldTIme).TotalSeconds;

            var currTime = DateTime.Now;
            //var ts = SignHelper.GetTimestamp(DateTime.Now.AddSeconds(30));
            Console.WriteLine("currTime--" + SignHelper.GetTimestamp(currTime));
            var ts = SignHelper.GetTimestamp(DateTime.Now.AddMinutes(1));

            Console.WriteLine(ts);
        }
    }
}
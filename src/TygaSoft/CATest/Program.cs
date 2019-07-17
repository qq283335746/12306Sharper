using System;

namespace TygaSoft.CATest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                SysUtilityTest.Run();
            }
            catch (Exception ex)
            {

            }


            Console.Read();
        }
    }
}

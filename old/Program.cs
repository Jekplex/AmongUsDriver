using System;
using System.Collections.Generic;

namespace AmongUsDriver
{
    class Program
    {
        public static List<int> integerList;

        static void Main(string[] args)
        {
            integerList = new List<int>();


            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();



        }
    }
}

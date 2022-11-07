using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.ASPNETCore.Mini.Common
{
    public class ConsoleHelper
    {
        public static void WriteLine(string Text, ConsoleColor Color = ConsoleColor.Red)
        {
            Console.ForegroundColor = Color;
            Console.Write("Info：");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("ASP.NET Core.Mini.Information");
            Console.Write($"      {Text}{Environment.NewLine}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace CSVLibrary.Benchmark
{
    internal class Program
    {

        static void Main(string[] args)
        {

            var summary = BenchmarkRunner.Run<WriteString_VS_WriteCharArray>();
            //char[] result = new char[300];
            //var sb = new StringBuilder();
            //sb.AppendLine("1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114");
            //sb.AppendLine("1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114");
            //sb.AppendLine("1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114");
            
            //sb.CopyTo(0,result,0,sb.Length);

            //StreamWriter writer = new StreamWriter("data.csv");
            //writer.Write(result,0,sb.Length);   
            //writer.Flush(); 
            //writer.Close(); 
            
            Console.ReadKey();
        }
    }
}

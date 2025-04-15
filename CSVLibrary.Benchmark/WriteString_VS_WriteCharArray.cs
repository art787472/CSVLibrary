using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace CSVLibrary.Benchmark
{
    [MemoryDiagnoser]
    public class WriteString_VS_WriteCharArray
    {
        StringBuilder sb = new StringBuilder("1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114");
        char[] chars = new char[90];
        [Benchmark]
        public void WrtieString()
        {
            var sw = new StreamWriter("writeStream.csv");

            sw.WriteLine("1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114");
            sw.Flush();
            sw.Close();
        }

        [Benchmark]
        public void WriteCharArray()
        {
            sb.CopyTo(0, chars, 0, sb.Length);
            var sw = new StreamWriter("writeStream.csv");

            sw.WriteLine(chars,0,sb.Length);
            sw.Flush();
            sw.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary
{
    internal class Program
    {
        static void Main(string[] args)
        {



            List<Item> list = CSVHelper.Read<Item>(@"D:\c_sharp\記帳程式\記帳程式\bin\Debug\data.csv");

            CSVHelper.Write<Item>(list, "C:\\Users\\art78\\Documents\\notexist\\notexist.csv");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Item> items = CSVHelper.Read<Item>("C:\\Users\\art78\\Documents\\notexist\\notexist.csv");
            

            CSVHelper.Write<Item>(items, "C:\\Users\\art78\\Documents\\notexist\\notexist.csv");
        }
    }
}

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

            // D:\\c_sharp\\記帳程式\\記帳程式\\bin\\Debug\\2024-07-04\\data.csv
            // D:\\c_sharp\\記帳程式\\記帳程式\\bin\\Debug\\2024-07-04\\data.csv
            List<DataModel> list = CSVHelper.Read<DataModel>(@"D:\c_sharp\平行處理練習\平行處理練習\MOCK_1000.csv");
            CSVHelper.Write<DataModel>(list, @"D:\c_sharp\平行處理進階操作\Data\Output\1000.csv");

           
        }
    }
}

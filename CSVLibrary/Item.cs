using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLibrary
{
    internal class Item
    {
        [DisplayName("時間")]
        public string dateTime { get; set; }

        [DisplayName("金額")]
        public int price { get; set; }

        //[DisplayName("類型")]
        //public string category { get; set; }
        //[DisplayName("消費目的")]
        //public string reason { get; set; }
        public string account { get; set; }
    }
}

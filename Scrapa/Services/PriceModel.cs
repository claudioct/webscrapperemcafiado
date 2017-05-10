using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapa.Services
{
    public class LastSold
    {
        public string Date { get; set; }
        public string Price { get; set; }
    }

    public class Stats
    {
        public int Count { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public LastSold LastSold { get; set; }
        public string mean { get; set; }
    }

    public class PriceModel
    {
        public string Book_id { get; set; }
        public Stats Stats { get; set; }
    }
}

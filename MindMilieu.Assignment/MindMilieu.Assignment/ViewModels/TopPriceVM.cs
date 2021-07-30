using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindMilieu.Assignment.ViewModels
{
    public class TopPriceVM
    {
        public int SchoolCode { get; set; }
        public string SchoolDesc { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}

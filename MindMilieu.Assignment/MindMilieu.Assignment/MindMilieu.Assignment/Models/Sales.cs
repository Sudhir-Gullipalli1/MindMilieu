using System;
using System.Collections.Generic;

namespace MindMilieu.Assignment.Models
{
    public class Sales
    {
        public int PropertyZip { get; set; }
        public int SchoolCode { get; set; }
        public string SchoolDesc { get; set; }
        public DateTime? RecordDate { get; set; }
        public DateTime? SaleDate { get; set; }
        public decimal? Price { get; set; }
    }
}

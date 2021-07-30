using MindMilieu.Assignment.Models;
using MindMilieu.Assignment.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MindMilieu.Assignment.Service
{
    public interface ISalesService
    {
        IEnumerable<TopPriceVM> GetTopNDistinctSchools(int Year, int Month, int Top, List<Sales> allSales;

        double? GetAverageDays(string SchoolName, int Year, int Month, List<Sales> allSales;
    }
}

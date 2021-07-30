using CsvHelper;
using MindMilieu.Assignment.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindMilieu.Assignment.ViewModels;

namespace MindMilieu.Assignment.Service
{

    class SalesService : ISalesService
    {
        private readonly ILogger _logger;

        public SalesService(ILogger<SalesService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<TopPriceVM> GetTopNDistinctSchools(int Year, int Month, int Top, List<Sales> allSales)
        {
            try
            {
                var result = allSales
                            .Where(x => x.SaleDate.HasValue && x.SaleDate.Value.Year == Year && x.SaleDate.Value.Month == Month)
                            .GroupBy(x => new { x.SchoolCode, x.SchoolDesc })
                            .Select(x => new TopPriceVM()
                            {
                                SchoolCode = x.Key.SchoolCode,
                                SchoolDesc = x.Key.SchoolDesc,
                                MaxPrice = x.ToList().Max(x => x.Price),
                            }).OrderByDescending(x => x.MaxPrice).Take(Top);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

        }

        public double? GetAverageDays(string SchoolName, int Year, int Month, List<Sales> allSales)
        {
            try
            {
                var result = allSales
                            .Where(x => x.SchoolDesc.ToLower() == SchoolName.ToLower() & x.RecordDate.HasValue && x.SaleDate.HasValue && x.SaleDate.Value.Year == Year && x.SaleDate.Value.Month == Month)
                            .Average(x => x.RecordDate.Value.Subtract(x.SaleDate.Value).Days);

                return Math.Round(result, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        private List<Sales> ReadInCSV(string absolutePath)
        {
            List<Sales> result = new List<Sales>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = "\t"

            };
            using (TextReader fileReader = System.IO.File.OpenText(absolutePath))
            {
                var csv = new CsvReader(fileReader, config);
                csv.Read();
                while (csv.Read())
                {
                    try
                    {
                        Sales sales = new Sales();
                        sales.PropertyZip = Convert.ToInt32(csv.GetField(0));
                        sales.SchoolCode = Convert.ToInt32(csv.GetField(1));
                        sales.SchoolDesc = Convert.ToString(csv.GetField(2));
                        DateTime dtRecordDate;
                        if (DateTime.TryParse(csv.GetField(3), CultureInfo.CurrentUICulture, DateTimeStyles.None, out dtRecordDate))
                        {
                            sales.RecordDate = dtRecordDate;
                        }

                        DateTime dtSaleDate;
                        if (DateTime.TryParse(csv.GetField(4), CultureInfo.CurrentUICulture, DateTimeStyles.None, out dtSaleDate))
                        {
                            sales.SaleDate = dtSaleDate;
                        }
                        sales.Price = Convert.ToDecimal(csv.GetField(5));
                        result.Add(sales);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }

                }
            }
            return result;
        }

    }
}

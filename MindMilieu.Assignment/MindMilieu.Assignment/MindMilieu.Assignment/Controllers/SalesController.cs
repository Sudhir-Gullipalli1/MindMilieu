using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MindMilieu.Assignment.Models;
using MindMilieu.Assignment.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MindMilieu.Assignment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly IOptions<MyConfig> _config;

        public SalesController(ILogger<SalesController> logger, IOptions<MyConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("GetTopNDistinctSchools")]
        public IEnumerable<TopPriceVM> GetTopNDistinctSchools(int Year, int Month, int Top)
        {
            try
            {
                var allSales = ReadInCSV(_config.Value.FilePath);

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

        [HttpGet("GetAverageDays")]
        public double? GetAverageDays(string SchoolName, int Year, int Month)
        {
            try
            {
                var allSales = ReadInCSV(_config.Value.FilePath);

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

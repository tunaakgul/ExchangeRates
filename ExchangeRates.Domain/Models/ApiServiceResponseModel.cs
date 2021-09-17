using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Domain.Models
{
    public class ApiServiceResponseModel
    {
        public string result { get; set; }
        public string base_code { get; set; }
        public ConversionRate conversion_rates { get; set; }
    }
    public class ConversionRate
    {
        public double EUR { get; set; }
        public double GBP { get; set; }
        public double ILS { get; set; }
        public double USD { get; set; }
    }

}

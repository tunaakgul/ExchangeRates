using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Domain.Models
{
    public class ConvertAmountRequestModel
    {
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public int BaseAmount { get; set; }
    }
}

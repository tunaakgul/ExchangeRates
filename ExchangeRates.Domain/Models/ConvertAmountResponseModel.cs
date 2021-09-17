using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Domain.Models
{
    public class ConvertAmountResponseModel
    {
        public decimal ExchangeRate { get; set; }
        public int QuoteAmount { get; set; }
    }
}

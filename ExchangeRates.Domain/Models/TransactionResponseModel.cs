using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Domain.Models
{
    public class TransactionResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Response { get; set; }
        public string ErrorMesssage { get; set; }
    }
}

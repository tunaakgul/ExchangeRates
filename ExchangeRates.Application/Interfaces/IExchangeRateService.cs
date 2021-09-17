using ExchangeRates.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Application.Interfaces
{
    public interface IExchangeRateService
    {
        ConvertAmountResponseModel ConvertAmount(ConvertAmountRequestModel exchangeRateRequestModel);
    }
}

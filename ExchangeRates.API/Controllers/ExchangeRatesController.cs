using ExchangeRates.Application.Interfaces;
using ExchangeRates.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRates.API.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    {
        IExchangeRateService _exchangeRateService;

        public ExchangeRatesController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }
        
        [HttpGet]
        public TransactionResponseModel ConvertAmount([FromQuery] ConvertAmountRequestModel request)
        {
            TransactionResponseModel transactionResponse = new TransactionResponseModel();
            try
            {
                ConvertAmountResponseModel exchangeRateResponse = _exchangeRateService.ConvertAmount(request);
                transactionResponse.IsSuccess = true;
                transactionResponse.Response = JsonConvert.SerializeObject(exchangeRateResponse);
            }
            catch (Exception ex)
            {
                transactionResponse.IsSuccess = false;
                transactionResponse.ErrorMesssage = ex.Message;
            }
            
            return transactionResponse;
        }
    }
}

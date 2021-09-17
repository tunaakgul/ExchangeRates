using ExchangeRates.Application.Cache;
using ExchangeRates.Application.Configuration;
using ExchangeRates.Application.Interfaces;
using ExchangeRates.Domain.Enums;
using ExchangeRates.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly LruCache _lruCache;

        public ExchangeRateService(LruCache lruCache)
        {
            _lruCache = lruCache;
        }

        public ConvertAmountResponseModel ConvertAmount(ConvertAmountRequestModel exchangeRateRequestModel)
        {
            if(exchangeRateRequestModel.BaseAmount <= 0)
            {
                throw new Exception("BaseAmount should be greater than 0.");
            }

            CurrencyTypes baseCurrency;
            CurrencyTypes quoteCurrency;
            try
            {
                baseCurrency = (CurrencyTypes)Enum.Parse(typeof(CurrencyTypes), exchangeRateRequestModel.BaseCurrency);
                quoteCurrency = (CurrencyTypes)Enum.Parse(typeof(CurrencyTypes), exchangeRateRequestModel.QuoteCurrency);
            }
            catch (Exception)
            {
                throw new Exception("BaseCurrency or QuoteCurrency is not supported.");
            }

            double? exchangeRate = null;
            try
            {
                //getting exchangerate from cache
                string cacheResult = _lruCache.Get(exchangeRateRequestModel.BaseCurrency);
                if(cacheResult != "")
                {
                    ApiServiceResponseModel cachedCurrencyRates = JsonConvert.DeserializeObject<ApiServiceResponseModel>(cacheResult);
                    exchangeRate = GetExchangeRateFromResponseModel(cachedCurrencyRates, quoteCurrency);
                    //adding cache again for move first on cache
                    _lruCache.Put(exchangeRateRequestModel.BaseCurrency,cacheResult);
                }
                //if cache does not include exchangerate
                if (exchangeRate == null)
                {
                    string apiKey = Constants.ApiKey;
                    string apiUrl = string.Concat(Constants.ApiUrl.Replace("YOUR-API-KEY", apiKey), exchangeRateRequestModel.BaseCurrency);
                    using (var webClient = new System.Net.WebClient())
                    {
                        var json = webClient.DownloadString(apiUrl);
                        ApiServiceResponseModel apiServiceResponse = JsonConvert.DeserializeObject<ApiServiceResponseModel>(json);
                        if (apiServiceResponse.result != "success")
                        {
                            throw new Exception();
                        }
                        exchangeRate = GetExchangeRateFromResponseModel(apiServiceResponse,quoteCurrency);
                        if(exchangeRate == null)
                        {
                            throw new Exception();
                        }
                        //putting service json to cache
                        _lruCache.Put(exchangeRateRequestModel.BaseCurrency, json);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("An error has occurred, try again later.");
            }
            var response = new ConvertAmountResponseModel();
            response.ExchangeRate = Math.Round(Convert.ToDecimal(exchangeRate), 3);
            response.QuoteAmount = (int)Math.Floor(exchangeRateRequestModel.BaseAmount * response.ExchangeRate);
            return response;
        }

        private double? GetExchangeRateFromResponseModel(ApiServiceResponseModel responseModel, CurrencyTypes quoteCurrency)
        {
            double? exchangeRate = null;
            switch (quoteCurrency)
            {
                case CurrencyTypes.EUR:
                    exchangeRate = responseModel.conversion_rates.EUR;
                    break;
                case CurrencyTypes.GBP:
                    exchangeRate = responseModel.conversion_rates.GBP;
                    break;
                case CurrencyTypes.USD:
                    exchangeRate = responseModel.conversion_rates.USD;
                    break;
                case CurrencyTypes.ILS:
                    exchangeRate = responseModel.conversion_rates.ILS;
                    break;
                default:
                    break;
            }
            return exchangeRate;
        }
    }
}

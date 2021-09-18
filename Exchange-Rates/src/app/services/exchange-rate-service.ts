import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { TransactionResponseModel } from '../models/transaction-response-model';
import { ConvertAmountRequestModel } from '../models/convert-amount-request-model';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRateService {

  apiServiceUrl: string = "http://localhost:5000/api/v1/ExchangeRates/";

  constructor(private http: HttpClient) {

  }

  async ConvertAmount(requestModel: ConvertAmountRequestModel): Promise<TransactionResponseModel>{
    const headerDict = {
      'Content-Type': 'application/json',
      Accept: 'application/json',
      'Access-Control-Allow-Headers': 'Content-Type',
      'Cache-Control': 'no-cache, no-store, must-revalidate, post-check=0, pre-check=0',
      Pragma: 'no-cache',
      Expires: '0',
    };

    let params = new HttpParams();
    params = params.append('BaseAmount', requestModel.baseAmount);
    params = params.append('BaseCurrency', requestModel.baseCurrency);
    params = params.append('QuoteCurrency', requestModel.quoteCurrency);

    const requestOptions = {
      headers: new HttpHeaders(headerDict),
      params: params
    };

    const apiUrl = this.apiServiceUrl + "ConvertAmount";

    return await this.http.get<TransactionResponseModel>(apiUrl, requestOptions).toPromise();
  }
}

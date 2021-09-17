import { CurrencyPipe } from '@angular/common';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ConvertAmountRequestModel } from './models/convert-amount-request-model';
import { ConvertAmountResponseModel } from './models/convert-amount-response-model';
import { ExchangeRateService } from './services/exchange-rate-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  @ViewChild('baseAmountInput') baseAmountInput!: ElementRef;
  baseAmountCurrencySymbol?: string = "";

  currencies = ["USD", "EUR", "GBP", "ILS"]

  formattedAmount: any;
  selectedBaseCurrency: any;
  selectedQuoteCurrency: any;
  resultSectionVisible: boolean = false;

  convertAmountResponse!: ConvertAmountResponseModel;

  constructor(public currencyPipe: CurrencyPipe, private toastr: ToastrService, private exchangeRateService: ExchangeRateService) {

  }

  transformAmount(element: any) {
    if (this.selectedBaseCurrency == undefined) {
      this.formattedAmount = "";
      this.toastr.error("Please select base currency.");
      return;
    }
    if (this.formattedAmount == undefined) {
      return;
    }
    //after the first call, formattedAmount's first char is currency symbol
    if (this.formattedAmount.length > 0 && isNaN(Number(this.formattedAmount[0]))) {
      this.formattedAmount = this.formattedAmount.substr(1);
    }
    //remove thousand comma
    this.formattedAmount = this.formattedAmount.replace(",", "");
    this.formattedAmount = this.currencyPipe.transform(this.formattedAmount, this.selectedBaseCurrency, 'symbol', '1.2-2');

    //remove currency symbol
    this.formattedAmount = this.formattedAmount.substr(1);

    if(element.target != null){
      element.target.value = this.formattedAmount;
    }
    else{
      element.nativeElement.value = this.formattedAmount;
    }
    

    this.checkInputsValidation();
  }

  onChangeBaseCurrency(newValue: string) {
    this.selectedBaseCurrency = newValue;
    this.baseAmountCurrencySymbol = this.currencyPipe.transform("0", this.selectedBaseCurrency, 'symbol')?.toString()[0];
    this.transformAmount(this.baseAmountInput);
  }

  onChangeQuoteCurrency(newValue: string) {
    this.selectedQuoteCurrency = newValue;
    this.checkInputsValidation();
  }

  //Checks base currency, quote currency and base amount, if all are valid call exchangerate api.
  async checkInputsValidation() {
    if (this.formattedAmount != undefined && this.selectedBaseCurrency != undefined && this.selectedQuoteCurrency != undefined) {
      await this.getExchangeRateResponse();
    }
  }

  async getExchangeRateResponse() {
    const requestModel = new ConvertAmountRequestModel();
    requestModel.baseAmount = this.formattedAmount.replace(".", "").replace(",", "");
    requestModel.baseCurrency = this.selectedBaseCurrency;
    requestModel.quoteCurrency = this.selectedQuoteCurrency;
    const response = await this.exchangeRateService.ConvertAmount(requestModel);
    if (!response.isSuccess) {
      this.toastr.error(response.errorMessage);
      return;
    }
    const convertAmountResponse = JSON.parse(response.response);
    this.convertAmountResponse = convertAmountResponse;
    this.convertAmountResponse.QuoteAmount = this.convertAmountResponse.QuoteAmount / 100;

    this.resultSectionVisible = true;
  }

  //user can not enter except number to base amount input. 
  //base amount input type is text instead of number.
  //because transformAmount method writes string (includes thousand seperator, currency symbol) to base amount input.
  public restrictNumeric(e: any) {
    let input;
    if (e.metaKey || e.ctrlKey) {
      return true;
    }
    if (e.which === 32) {
      return false;
    }
    if (e.which === 0) {
      return true;
    }
    if (e.which < 33) {
      return true;
    }
    input = String.fromCharCode(e.which);
    return !!/[\d\s]/.test(input);
  }
}

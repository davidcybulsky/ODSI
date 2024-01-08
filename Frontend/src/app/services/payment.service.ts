import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaymentModel } from '../models/payment.model';
import { Environment } from '../../environments/environment.prod';
import { CreatePaymentModel } from '../models/create.payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  constructor(private httpClient: HttpClient) { }

  getPayments() : Observable<PaymentModel[]> {
    return this.httpClient.get<PaymentModel[]>(`${Environment.apiUrl}/transfer`)
  }

  createPayment(createPaymentModel : CreatePaymentModel) : Observable<void> {
    return this.httpClient.post<void>(`${Environment.apiUrl}/transfer`, createPaymentModel)
  }

}

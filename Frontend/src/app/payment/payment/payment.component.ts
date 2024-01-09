import { Component, OnInit } from '@angular/core';
import { PaymentService } from '../../services/payment.service';
import { Observable } from 'rxjs';
import { PaymentModel } from '../../models/payment.model';
import { LinksComponent } from '../../links/links.component';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [
    LinksComponent
  ],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.css'
})
export class PaymentComponent implements OnInit {

  constructor(private paymentService: PaymentService) { }
  
  payments : Observable<PaymentModel[]> | undefined;

  ngOnInit(): void {
    this.payments = this.paymentService.getPayments()
  }
}

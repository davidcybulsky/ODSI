import { Component, OnInit } from '@angular/core';
import { PaymentService } from '../../services/payment.service';
import { Observable, take } from 'rxjs';
import { PaymentModel } from '../../models/payment.model';
import { LinksComponent } from '../../links/links.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [
    CommonModule,
    LinksComponent
  ],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.css'
})
export class PaymentComponent implements OnInit {

  constructor(private paymentService: PaymentService) { }
  
  payments : PaymentModel[] | undefined;

  ngOnInit(): void {
    this.paymentService.getPayments().pipe(take(1)).subscribe(
      response => {
        this.payments = response
      }
    )
  }
}

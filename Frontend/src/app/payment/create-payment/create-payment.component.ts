import { Component, OnInit } from '@angular/core';
import { PaymentService } from '../../services/payment.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LinksComponent } from '../../links/links.component';

@Component({
  selector: 'app-create-payment',
  standalone: true,
  imports: [
    LinksComponent,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './create-payment.component.html',
  styleUrl: './create-payment.component.css'
})
export class CreatePaymentComponent implements OnInit {

  paymentForm! : FormGroup;

  constructor(private paymentService: PaymentService,
              private formBuilder: FormBuilder,
              private router: Router) { }

  ngOnInit(): void {
    this.paymentForm = this.formBuilder.group({

    })
  }

  onCreatePayment() {
    this.paymentService.createPayment(this.paymentForm.value).subscribe()
  }

  onCancel() {
    this.router.navigateByUrl('/payment')
  }
}

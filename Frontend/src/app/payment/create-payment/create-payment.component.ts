import { Component, OnInit } from '@angular/core';
import { PaymentService } from '../../services/payment.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LinksComponent } from '../../links/links.component';
import { Subject } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-create-payment',
  standalone: true,
  imports: [
    LinksComponent,
    FormsModule,
    ReactiveFormsModule,
    AsyncPipe
  ],
  templateUrl: './create-payment.component.html',
  styleUrl: './create-payment.component.css'
})
export class CreatePaymentComponent implements OnInit {

  paymentForm! : FormGroup;
  error : Subject<string|undefined> = new Subject()
  error$ = this.error.asObservable()

  constructor(private paymentService: PaymentService,
              private formBuilder: FormBuilder,
              private router: Router) { }

  ngOnInit(): void {
    this.paymentForm = this.formBuilder.group({
      title: ['',[Validators.required]],
      receiversAccountNumber: ['',Validators.required],
      amountOfMoney: ['',Validators.required]
    })
  }

  onCreatePayment() {
    if(this.paymentForm.invalid) {
      this.error.next("All fields are required")
      return
    }
    this.paymentService.createPayment(this.paymentForm.value).subscribe(
      success => {
        this.router.navigateByUrl("/account")
      },
      error => {
        this.error.next(error.error)
      }
    )
  }

  onCancel() {
    this.router.navigateByUrl('/payment')
  }
}

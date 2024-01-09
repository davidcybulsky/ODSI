import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = 
[
  {
    path: '',
    loadComponent: () => import('./payment/payment.component').then(c => c.PaymentComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./create-payment/create-payment.component').then(c => c.CreatePaymentComponent)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PaymentRoutingModule { }

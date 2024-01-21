import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./account/account.component').then(c => c.AccountComponent)
  },
  {
    path: 'password',
    loadComponent: () => import('./password/password.component').then(c => c.PasswordComponent)
  },
  {
    path: 'details',
    loadComponent: () => import('./account-details/account-details.component').then(c => c.AccountDetailsComponent)
  },
  {
    path: 'document',
    loadComponent: () => import('./document/document.component').then(c => c.DocumentComponent)
  },
  {
    path: 'cards',
    loadComponent: () => import('./cards/cards.component').then(c => c.CardsComponent)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }

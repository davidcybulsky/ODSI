import { Routes } from '@angular/router';

export const routes: Routes = 
[
    {
        path: 'auth',
        loadChildren: () => import("./auth/auth.module").then(m => m.AuthModule)
    },
    {
        path: 'account',
        loadChildren: () => import("./account/account.module").then(m => m.AccountModule)
    },
    {
        path: 'payment',
        loadChildren: () => import('./payment/payment.module').then(m => m.PaymentModule)
    },
    {
        path: '**',
        redirectTo: 'account',
        pathMatch: 'full'
    }
];

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = 
[
  {
    path: 'login',
    loadComponent: () => import("./login/login.component").then(c => c.LoginComponent)
  },
  {
      path: 'mask',
      loadComponent: () => import("./mask/mask.component").then(c => c.MaskComponent)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }

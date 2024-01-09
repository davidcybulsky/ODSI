import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LinksComponent } from '../../links/links.component';

@Component({
  selector: 'app-password',
  standalone: true,
  imports: [
    LinksComponent,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './password.component.html',
  styleUrl: './password.component.css'
})
export class PasswordComponent implements OnInit {

  passwordForm!: FormGroup

  constructor(private accountService: AccountService,
              private formBuilder: FormBuilder,
              private router: Router) {}

  ngOnInit(): void {
    this.passwordForm = this.formBuilder.group({
      password: [''],
      newPassword: [''],
      repeatPassword: [''] 
    })
  }

  changePassword(): void {
    this.accountService.changePassword(this.passwordForm.value).subscribe()
  }  

    
  onCancel() {
    this.router.navigateByUrl("/account")
  }
}

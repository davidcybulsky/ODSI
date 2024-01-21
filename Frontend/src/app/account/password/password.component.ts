import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LinksComponent } from '../../links/links.component';
import { Subject } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-password',
  standalone: true,
  imports: [
    LinksComponent,
    FormsModule,
    ReactiveFormsModule,
    AsyncPipe    
  ],
  templateUrl: './password.component.html',
  styleUrl: './password.component.css'
})
export class PasswordComponent implements OnInit {

  passwordForm!: FormGroup
  error : Subject<string|undefined> = new Subject()
  error$ = this.error.asObservable()

  constructor(private accountService: AccountService,
              private formBuilder: FormBuilder,
              private router: Router) {}

  ngOnInit(): void {
    this.passwordForm = this.formBuilder.group({
      currentPassword: [''],
      newPassword: [''],
      confirmedPassword: [''] 
    })
  }

  changePassword(): void {
    this.accountService.changePassword(this.passwordForm.value).subscribe(
      success => {
        this.router.navigateByUrl("/account");
      },
      error => {
        this.error.next(error.error)
      }
    )
  }  

    
  onCancel() {
    this.router.navigateByUrl("/account")
  }
}

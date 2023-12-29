import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  
  loginForm!: FormGroup;

  constructor(private authService: AuthService,
              private formBuilder: FormBuilder,
              private router: Router) {
    this.initForm()
  }

  initForm() {
    this.loginForm = this.formBuilder.group({
      login: ['']
    })
  }

  onSubmit() {
    this.authService.getmask(this.loginForm.value)
      .subscribe(response => {
        this.router.navigateByUrl("/auth/mask");
      })
  }
}
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginModel } from '../../models/login.model';

@Component({
  selector: 'app-mask',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './mask.component.html',
  styleUrl: './mask.component.css'
})
export class MaskComponent implements  OnInit {

  maskForm! : FormGroup;

  constructor(private authService: AuthService,
              private formBuilder: FormBuilder) {

  }
  ngOnInit(): void {
    this.initForm()
  }

  initForm() {
    this.maskForm = this.formBuilder.group({
      login : this.authService.username,
      password: ['']
    })
  }

  onSubmit() {
    this.authService.login(this.maskForm.value).subscribe()
  }


}

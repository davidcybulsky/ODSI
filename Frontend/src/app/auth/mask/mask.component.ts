import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginModel } from '../../models/login.model';
import { MaskModel } from '../../models/mask.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-mask',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './mask.component.html',
  styleUrl: './mask.component.css'
})
export class MaskComponent implements  OnInit {

  maskForm! : FormGroup;
  mask: MaskModel | undefined;

  constructor(private authService: AuthService,
              private formBuilder: FormBuilder) {

  }
  
  ngOnInit(): void {
    this.initForm()
    this.mask = this.authService.mask
    console.log(this.mask)
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

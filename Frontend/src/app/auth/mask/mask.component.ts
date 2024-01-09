import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MaskModel } from '../../models/mask.model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

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
              private formBuilder: FormBuilder,
              private router: Router) {

  }
  
  ngOnInit(): void {
    this.initForm()
    this.mask = this.authService.mask
    console.log(this.mask)
  }

  initForm() {
    this.maskForm = this.formBuilder.group({
      login : this.authService.username,
      partialPassword: ['']
    })
  }

  onSubmit() {
    this.authService.login(this.maskForm.value).subscribe(
      response => {
        this.router.navigateByUrl("/account")
      }
    )
  }


}

import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MaskModel } from '../../models/mask.model';
import { AsyncPipe, CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-mask',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AsyncPipe
  ],
  templateUrl: './mask.component.html',
  styleUrl: './mask.component.css'
})
export class MaskComponent implements  OnInit {

  maskForm! : FormGroup;
  mask: MaskModel | undefined;
  error : Subject<string|undefined> = new Subject()
  error$ = this.error.asObservable()

  constructor(private authService: AuthService,
              private formBuilder: FormBuilder,
              private router: Router) { }
  
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
      },
      error => {
        this.error.next(error.error)
      }
    )
  }


}

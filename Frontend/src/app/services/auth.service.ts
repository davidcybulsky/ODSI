import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GetMaskModel } from '../models/get.mask.model';
import { MaskModel } from '../models/mask.model';
import { Observable, map } from 'rxjs';
import { LoginModel } from '../models/login.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  mask: MaskModel | undefined

  constructor(private httpClient: HttpClient) { }

  getmask(getmask: GetMaskModel): Observable<MaskModel> {
    return this.httpClient.post<MaskModel>('', getmask)
      .pipe(
        map( response => {
            this.mask = response
            return response;
          }
        )) 
  }

  login(loginModel: LoginModel): Observable<void> {
    return this.httpClient.post<void>('', loginModel)
  }

  signup() {

  }


}

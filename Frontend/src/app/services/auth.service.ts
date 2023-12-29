import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GetMaskModel } from '../models/get.mask.model';
import { MaskModel } from '../models/mask.model';
import { Observable, map } from 'rxjs';
import { LoginModel } from '../models/login.model';
import { Environment } from '../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  mask: MaskModel | undefined
  username: string | undefined

  constructor(private httpClient: HttpClient) { }

  getmask(getmask: GetMaskModel): Observable<MaskModel> {
    this.username = getmask.login
    return this.httpClient.post<MaskModel>(`${Environment.apiUrl}/auth/mask`, getmask)
      .pipe(
        map( response => {
            this.mask = response
            return response;
    })) 
  }

  login(loginModel: LoginModel): Observable<void> {
    return this.httpClient.post<void>(`${Environment.apiUrl}/login`, loginModel)
  }

  signup() {

  }


}

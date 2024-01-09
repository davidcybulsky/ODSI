import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GetMaskModel } from '../models/get.mask.model';
import { MaskModel } from '../models/mask.model';
import { Observable, map, of } from 'rxjs';
import { LoginModel } from '../models/login.model';
import { Environment } from '../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  mask: MaskModel | undefined
  username: string | undefined

  constructor(private httpClient: HttpClient) { }

  getmask(getmaskModel: GetMaskModel): Observable<MaskModel> {
    this.username = getmaskModel.login
    return this.httpClient.post<MaskModel>(`${Environment.apiUrl}/auth/mask`, getmaskModel)
      .pipe(
        map( response => {
            this.mask = response
            console.log(this.mask)
            return response;
    })) 
  }

  login(loginModel: LoginModel): Observable<void> {
    return this.httpClient.post<void>(`${Environment.apiUrl}/auth/login`, loginModel, { withCredentials: true })
  }

  logout(): Observable<void> {
    return this.httpClient.get<void>(`${Environment.apiUrl}/auth/logout`, { withCredentials: true })
  }

  iSAuthenticated(): Observable<boolean> {
    return this.httpClient.get<boolean>(`${Environment.apiUrl}/auth`, { withCredentials: true })
  }

}

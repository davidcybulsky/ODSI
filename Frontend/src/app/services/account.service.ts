import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountModel } from '../models/account.model';
import { Environment } from '../../environments/environment.prod';
import { ChangePasswordModel } from '../models/change.password.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private httpClient : HttpClient) { }

  getAccount() : Observable<AccountModel> {
    return this.httpClient.get<AccountModel>(`${Environment.apiUrl}/account`, { withCredentials: true })
  }

  changePassword(changePasswordModel : ChangePasswordModel) : Observable<void> {
    return this.httpClient.put<void>(`${Environment.apiUrl}/account`, changePasswordModel, { withCredentials: true })
  }
}

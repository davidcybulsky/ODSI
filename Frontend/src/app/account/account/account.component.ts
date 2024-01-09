import { Component, OnInit } from '@angular/core';
import { LinksComponent } from '../../links/links.component';
import { AccountService } from '../../services/account.service';
import { AccountModel } from '../../models/account.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [
    LinksComponent
  ],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css'
})
export class AccountComponent implements OnInit {

  accountModel : Observable<AccountModel> | undefined

  constructor(private accountService: AccountService) {}

  ngOnInit() {
    this.accountModel = this.accountService.getAccount()
  }

}

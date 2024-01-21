import { Component, OnInit } from '@angular/core';
import { LinksComponent } from '../../links/links.component';
import { AccountService } from '../../services/account.service';
import { AccountModel } from '../../models/account.model';
import { Observable, take } from 'rxjs';

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

  accountModel : AccountModel | undefined

  constructor(private accountService: AccountService) {}

  ngOnInit() {
    this.accountService.getAccount().pipe(take(1))
    .subscribe(response => {
        this.accountModel = response
      })
  }

}

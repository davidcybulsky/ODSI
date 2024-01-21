import { Component, OnDestroy, OnInit } from '@angular/core';
import { LinksComponent } from '../../links/links.component';
import { CardService } from '../../services/card.service';
import { DocumentService } from '../../services/document.service';
import { CardModel } from '../../models/card.model';
import { Observable, take, takeUntil } from 'rxjs';
import { DocumentModel } from '../../models/document.model';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-account-details',
  standalone: true,
  imports: [
    LinksComponent,
    RouterModule
  ],
  templateUrl: './account-details.component.html',
  styleUrl: './account-details.component.css'
})
export class AccountDetailsComponent{
}

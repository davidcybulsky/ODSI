import { Component, OnDestroy, OnInit } from '@angular/core';
import { LinksComponent } from '../../links/links.component';
import { CardService } from '../../services/card.service';
import { DocumentService } from '../../services/document.service';
import { CardModel } from '../../models/card.model';
import { Observable, take, takeUntil } from 'rxjs';
import { DocumentModel } from '../../models/document.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-account-details',
  standalone: true,
  imports: [
    CommonModule,
    LinksComponent
  ],
  templateUrl: './account-details.component.html',
  styleUrl: './account-details.component.css'
})
export class AccountDetailsComponent implements OnInit {

  document: DocumentModel | undefined
  debitCards: CardModel[] | undefined
  isVisable: boolean = false
  isCardVisable: boolean = false

  constructor(private cardService: CardService,
              private documentService: DocumentService) {}
  
  ngOnInit(): void {
    this.cardService.getCards().pipe(take(1)).subscribe(
      response => {
        this.debitCards = response
      }
    )
    this.documentService.getDocuments().pipe(take(1)).subscribe(
      response => {
        this.document = response
      }
    )
  } 

  onChangeVisablility() {
    this.isVisable = !this.isVisable
    if(this.isVisable) {
      setInterval(() => {
        this.isVisable = false
      },
      10000)
    }
  }

  onChangeVisablilityCard() {
    this.isCardVisable = !this.isCardVisable
    if(this.isCardVisable) {
      setInterval(() => {
        this.isCardVisable = false
      },
      10000)
    }
  }
}

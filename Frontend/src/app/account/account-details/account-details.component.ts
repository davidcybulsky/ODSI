import { Component, OnInit } from '@angular/core';
import { LinksComponent } from '../../links/links.component';
import { CardService } from '../../services/card.service';
import { DocumentService } from '../../services/document.service';
import { CardModel } from '../../models/card.model';
import { Observable } from 'rxjs';
import { DocumentModel } from '../../models/document.model';

@Component({
  selector: 'app-account-details',
  standalone: true,
  imports: [
    LinksComponent
  ],
  templateUrl: './account-details.component.html',
  styleUrl: './account-details.component.css'
})
export class AccountDetailsComponent implements OnInit{

  document: Observable<DocumentModel> | undefined
  debitCards: Observable<CardModel[]> | undefined

  constructor(private cardService: CardService,
              private documentService: DocumentService) {}
  
  ngOnInit(): void {
    this.debitCards = this.cardService.getCards()
    this.document = this.documentService.getDocuments()
  } 
}

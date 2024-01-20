import { Component, OnInit } from '@angular/core';
import { CardService } from '../../services/card.service';
import { CardModel } from '../../models/card.model';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';
import { LinksComponent } from '../../links/links.component';

@Component({
  selector: 'app-cards',
  standalone: true,
  imports: [
    CommonModule,
    LinksComponent
  ],
  templateUrl: './cards.component.html',
  styleUrl: './cards.component.css'
})
export class CardsComponent implements OnInit {

  debitCards: CardModel[] | undefined
  isCardVisable: boolean = false

  constructor(private cardService: CardService) { } 

  ngOnInit(): void {
    this.cardService.getCards().pipe(take(1)).subscribe(
      response => {
        this.debitCards = response
      }
    )
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

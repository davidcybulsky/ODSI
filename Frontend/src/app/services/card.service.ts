import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Environment } from '../../environments/environment.prod';
import { CardModel } from '../models/card.model';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CardService {

  constructor(private httpClient : HttpClient) { }

  getCards() : Observable<CardModel[]> {
    return this.httpClient.get<CardModel[]>(`${Environment.apiUrl}/card`, { withCredentials: true }).pipe(
      map(response => {
        console.log(response)
        return response
      })
    )
  }
}

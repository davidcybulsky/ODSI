import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { DocumentModel } from '../models/document.model';
import { Environment } from '../../environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {

  constructor(private httpClient : HttpClient) { }

  getDocuments() : Observable<DocumentModel> {
    return this.httpClient.get<DocumentModel>(`${Environment.apiUrl}/document`, { withCredentials : true }).pipe(
      map(response => {
        console.log(response)
        return response
      }))
  }
}

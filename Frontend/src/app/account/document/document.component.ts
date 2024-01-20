import { Component, OnInit } from '@angular/core';
import { DocumentService } from '../../services/document.service';
import { DocumentModel } from '../../models/document.model';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';
import { LinksComponent } from '../../links/links.component';

@Component({
  selector: 'app-document',
  standalone: true,
  imports: [
    CommonModule,
    LinksComponent
  ],
  templateUrl: './document.component.html',
  styleUrl: './document.component.css'
})
export class DocumentComponent implements OnInit {

  document: DocumentModel | undefined
  isVisable: boolean = false

  constructor(private documentService: DocumentService) {}

  ngOnInit(): void {
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

}

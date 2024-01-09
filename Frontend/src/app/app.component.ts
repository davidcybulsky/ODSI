import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  
  constructor(private authService: AuthService,
              private router: Router) {}

  ngOnInit(): void {
    this.authService.iSAuthenticated().pipe(take(1)).subscribe(
      success => {
        if(success === true) {
          this.router.navigateByUrl("/account")
        }
        else {
          this.router.navigateByUrl("/auth/login")
        }
      },
      error => {
        this.router.navigateByUrl("/auth/login")
      })
  }
  
  title = 'Frontend';
}

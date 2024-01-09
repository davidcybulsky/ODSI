import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-links',
  standalone: true,
  imports: [
    RouterModule
  ],
  templateUrl: './links.component.html',
  styleUrl: './links.component.css'
})
export class LinksComponent {

  constructor(private authService: AuthService,
              private router: Router) {}

  onLogout() {
    this.authService.logout().subscribe(
      success => {
        this.router.navigateByUrl("/auth/login")
      }
    )
  }

}

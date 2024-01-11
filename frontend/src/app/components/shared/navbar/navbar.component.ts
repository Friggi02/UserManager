import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent{

  profilePic: boolean = false;

  constructor(
    public authService: AuthService,
    private router: Router
  ){
    this.profilePic = this.authService.getProfilePic() !== null;
  }

  isAuthenticated() : boolean {
    return !!this.authService.getAccessToken();
  }

  onLogout(){
    this.authService.removeAccessToken();
    this.authService.removeRefreshToken();
    this.authService.removeProfilePic();
    this.router.navigate(['/']);
  }
}
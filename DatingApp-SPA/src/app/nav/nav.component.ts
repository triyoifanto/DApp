import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;

  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login() {
    this.authService.login(this.model)
      .subscribe(next => {
        // using alertify service
        this.alertify.success('login succesfully');
      }, error => {
        // the error message already handled by custom service errorinterceptor
        this.alertify.error(error);
      }, () => {
        // on complete navigate to members page
        this.router.navigate(['/members']);
      }
      );
  }

  loggedIn() {
    // use auth0 jwt service
    return this.authService.loggedIn(); // retun true or false value, based on token value
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');

    this.authService.decodedToken = null;
    this.authService.currentUser = null;

    this.alertify.message('logged out');

    this.router.navigate(['/home']);
  }
}

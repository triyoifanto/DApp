import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model)
      .subscribe(next => {
        // using alertify service
        this.alertify.success('login succesfully');
      }, error => {
        // the error message already handled by custom service errorinterceptor
        this.alertify.error(error);
      }
      );
  }

  loggedIn() {
    // use auth0 jwt service
    return this.authService.loggedIn(); // retun true or false value, based on token value
  }

  logout() {
    localStorage.removeItem('token');
    this.alertify.message('logged out');
  }
}

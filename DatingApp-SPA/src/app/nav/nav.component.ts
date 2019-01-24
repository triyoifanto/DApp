import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model)
    .subscribe(next => {
      console.log('login suceesfully');
    }, error => {
      console.log('error logged in');
    }
    );
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token; // retun true or false value, based on token value
  }

  logout() {
    localStorage.removeItem('token');
    console.log('logged out');
  }

}

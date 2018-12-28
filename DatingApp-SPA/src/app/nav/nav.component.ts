import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {

  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login(): void {
    this.authService.login(this.model)
      .subscribe(next => {
        this.alertify.success('logged in successfully');
      }, error => {
        this.alertify.error(error);
      });
  }

  loggedIn(): boolean {
    return this.authService.loggedIn();
  }

  logout(): void {
    this.authService.loggedOut();
    this.alertify.message('Logged out');
  }

}

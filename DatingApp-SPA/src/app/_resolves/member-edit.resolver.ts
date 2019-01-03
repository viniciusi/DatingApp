import { AuthService } from './../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
    constructor(
        private userService: UserService,
        private router: Router,
        private alertify: AlertifyService,
        // since we're going to edit the current user, we wil need to decode the
        // token to get the ID. So we need to inject the AuthService
        private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        // we don't used the subscribe because when we use a resolver
        // this automatically subscribes to the method
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            // from here, this is purely to catch the error
            // and return out of this method if we a problem
            // if we don't have a problem we're just going to continue
            // on to our roots that we're activating.
            catchError(error => {
                this.alertify.error('Problem retrieving your data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}

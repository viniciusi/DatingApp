import { AuthService } from './../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/message';

@Injectable()
export class MessagesResolver implements Resolve<Message> {
    pageNumber = 1;
    pageSize = 5;
    messageContainer = 'Unread';

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Message> {
        // we don't used the subscribe because when we use a resolver
        // this automatically subscribes to the method
        return this.userService.getMessages(this.authService.decodedToken.nameid,
                this.pageNumber, this.pageSize, this.messageContainer).pipe(
            // from here, this is purely to catch the error
            // and return out of this method if we a problem
            // if we don't have a problem we're just going to continue
            // on to our roots that we're activating.
            catchError(error => {
                this.alertify.error('Problem retrieving messages');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}

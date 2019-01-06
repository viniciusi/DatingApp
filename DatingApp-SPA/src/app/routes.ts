import { MessagesResolver } from './_resolves/messages.resolver';
import { ListsResolver } from './_resolves/lists.resolver';
import { PreventUnsavedChanges } from './_guards/prevents-unsaved-changes.guard';
import { MemberEditResolver } from './_resolves/member-edit.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListResolver } from './_resolves/member-list.resolver';
import { MemberDetailResolver } from './_resolves/member-detail.resolver';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [ AuthGuard ],
        children: [
            { path: 'members', component: MemberListComponent,
                resolve: {users: MemberListResolver} },
            { path: 'members/:id', component: MemberDetailComponent,
                resolve: {user: MemberDetailResolver} },
            { path: 'member/edit', component: MemberEditComponent,
                resolve: { user: MemberEditResolver}, canDeactivate: [ PreventUnsavedChanges ] },
            { path: 'messages', component: MessagesComponent, resolve: {messages: MessagesResolver} },
            { path: 'lists', component: ListsComponent, resolve: {users: ListsResolver} },
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];

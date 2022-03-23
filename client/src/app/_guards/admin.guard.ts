import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private accountService: AccountService, private toastr: ToastrService)
  {

  }

  canActivate(): Observable<boolean> {

    return this.accountService.currentUser$.pipe(
      /* check if the logged in user has Admin or Moderator roles
         and deny access if they don't */
      map(user =>
      {
      if (user.roles.includes('Admin') || user.roles.includes('Moderator'))
      {
        return true;
      }
      this.toastr.error("Denied Access. This account does not have the right permissions");

      })
    );
  }
}

import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {

  //This directive will receives an array of strings, specifying the required roles.
  @Input() appHasRole: string[];

  //Used to store the current user.
  user: User;

  constructor(private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService)
  {
    //Get access to the current user.
    this.accountService.currentUser$.pipe(take(1)).subscribe(user =>
      {
        this.user = user;
      })
  }

  ngOnInit(): void
  {
    //Clear the view if user has no roles or is not authenticated.
    if (!this.user?.roles || this.user == null)
    {
      this.viewContainerRef.clear();
      return;
    }
    /* If user does have all of the required roles, then create
       an embedded view and set the template reference as the html element
       where the directive is used */
    if(this.user?.roles.some(r => this.appHasRole.includes(r)))
    {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else{
            this.viewContainerRef.clear();
          }
  }

}

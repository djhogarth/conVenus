import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;

  //store only one version of the current user using this buffer like object
  private currentUserSource = new ReplaySubject<User>(1);

  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  /*A userDTO is returned from the API.
    We then transform this data inside the
    http post before subscribing to it */

  login(model: any)
  {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(

      /*Get the response from the server and then get the user
        from the response. If there is a user, add the user object
        to local storage within the browser. */

      map((response : User) =>
      {
        const user = response;
        if (user){
        this.setCurrentUser(user);
        this.presenceService.createHubConnection(user);
        }
      })
    );
  }

  /* register a user by sending a http post request
    back to the API's account countroller's register method */

  register(model: any){
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        const user = response;
        if(user) {
        this.setCurrentUser(user);
        this.presenceService.createHubConnection(user);
        }
      })
    )
  }

  //sets the user as the current user in the account service

  setCurrentUser(user: User)
  {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;

    /* The role field within the token can either be a string
      or an array of strings depending on the number of given roles.
      If roles gotten from token is already an array, then populate
      the roles property directly. If not, then push that single role
      from the token into the user object's roles array */

    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    //JSONify the user and store it in browser storage so login can persist
    localStorage.setItem('user',JSON.stringify(user));

    this.currentUserSource.next(user);
  }

  //Get the information inside a token, specifically the payload part
  getDecodedToken(token)
  {
    return JSON.parse(atob(token.split(".")[1]));
  }

  // upon a user logging out, remove them from the broswer local storage
  logOut()
  {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();

  }

}

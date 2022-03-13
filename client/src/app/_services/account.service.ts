import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  //store only one version of the current user using this buffer like object
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  constructor(private http: HttpClient) { }

  /*A userDTO is returned to us from the API.
    We want to transform this data inside the
    http post before we subscribe to it */
login(model: any) {
  return this.http.post(this.baseUrl + 'account/login', model).pipe(
    /*Get the response from the server and then get the user from the response.
      If there is a user, add the user object to local storage within the browser.*/

    map((response : User) => {
      const user = response;
      if (user){
       this.setCurrentUser(user);
      }
    })
  );
}

register(model: any){
  return this.http.post(this.baseUrl + 'account/register', model).pipe(
    map((response: User) => {
      const user = response;
      if(user) {
       this.setCurrentUser(user);
      }
    })
  )
}

//helper method
setCurrentUser(user: User) {
  localStorage.setItem('user',JSON.stringify(user));
  this.currentUserSource.next(user);
}

// upon a user logging out, remove them from the broswer local storage
logOut() {
  localStorage.removeItem('user');
  this.currentUserSource.next(null);

}

}

import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  //Create a hub connection for the given user
  createHubConnection(user: User)
  {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    //Start hub connection
    this.hubConnection
      .start()
      .catch(error => console.log(error));

    /* Listen for server events and show
       notifications when user logs in or out. */
    this.hubConnection.on('UserIsOnline', username =>
    {
      /* Update the list of online users we are tracking
         wth the new user that has just logged on */
      this.onlineUsers$.pipe(take(1)).subscribe(usernames =>
        {
          this.onlineUserSource.next([...usernames, username]);
        })

    });

    this.hubConnection.on('UserIsOffline', username =>
    {
      /* Update the list of online users we are tracking
         by taking out the user who has just logged out */
      this.onlineUsers$.pipe(take(1)).subscribe(usernames =>
        {
          this.onlineUserSource.next([...usernames.filter(x => x !== username)]);
        })
    });

    this.hubConnection.on('GetOnlineUsers', (usernames : string[]) => {
      this.onlineUserSource.next(usernames);
    });

    this.hubConnection.on('NewMessageReceived', ({username, alias}) => {
      this.toastr.info(alias + ' has sent you a new message!')
      .onTap
      .pipe(take(1))
      .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'));
    });
  }

  //Stop hub connection
  stopHubConnection()
  {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}

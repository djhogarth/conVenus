import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { Group } from '../_models/group';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { BusyService } from './busy.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
   baseUrl : string = environment.apiUrl;
   hubUrl = environment.hubUrl;
   private hubConnection : HubConnection;
   private messageThreadSource = new BehaviorSubject<Message[]>([]);
   messageThread$ = this.messageThreadSource.asObservable();


  constructor(private http: HttpClient, private busyService: BusyService) { }

  createHubConnection(user: User, receiverUserName: string)
  {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + receiverUserName, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

      /* whether or not the promise returned from
      the hub connection start is successful or rejects,
       turn off the loading indicator */
      this.hubConnection.start().catch(error =>
        console.log(error)).finally(() =>
          this.busyService.idle());

      /* listen for the "NewMessage" method */
      this.hubConnection.on('ReceiveMessageThread', messages => {
        this.messageThreadSource.next(messages);
      });

       /* listen for the "NewMessage" method */
      this.hubConnection.on('NewMessage', message => {
        this.messageThread$.pipe(take(1)).subscribe(messages =>
          {
            /* Update the BehaviourSubject with a new message array
               that includes the message we've just received. */
            this.messageThreadSource.next([...messages, message]);
          });
      });

      /* listen for the "UpdatedGroup" method. */
      this.hubConnection.on('UpdatedGroup', (group: Group) =>
      {
        /* Look inside the message thread and see if there are
           any unread messages for the user that has just
           joined this group. And if there are, then we
           want to mark them as read. */
        if (group.connections.some(x => x.username === receiverUserName))
        {
          this.messageThread$.pipe(take(1)).subscribe(messages =>
            {
              messages.forEach(message =>
                {
                  if(!message.dateRead)
                  {
                    message.dateRead = new Date(Date.now())
                  }
                })
                this.messageThreadSource.next([...messages]);
            })
        }
      })

  }

  stopHubConnection()
  {
    if(this.hubConnection)
    {
      /* When stopping the hub connection,
       clear the message thread source to an empty array */
      this.messageThreadSource.next([]);
      this.hubConnection.stop();
    }

  }

  getMessages(pageNumber, pageSize, container)
  {
    let params = getPaginationHeaders(pageNumber , pageSize);
    params = params.append('Container', container);

    return getPaginatedResult<Message[]>(this.baseUrl + 'message', params, this.http);
  }


  async sendMessage(username: string, content: string)
  {
    //Use the message hub to send messages instead of using the API
    return this.hubConnection.invoke('SendMessage', {recipientUsername: username, content})
      .catch(error => console.log(error));
  }

  deleteMessage(id: number){
    return this.http.delete(this.baseUrl + 'message/' + id);
  }

  getMessageThread(username: string)
  {
    return this.http.get<Message[]>(this.baseUrl + 'message/thread/' + username);
  }


}

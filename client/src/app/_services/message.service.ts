import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
   baseUrl : string = environment.apiUrl;
   hubUrl = environment.hubUrl;
   private hubConnection : HubConnection;
   private messageThreadSource = new BehaviorSubject<Message[]>([]);
   messageThread$ = this.messageThreadSource.asObservable();


  constructor(private http: HttpClient) { }

  createHubConnection(user: User, receiverUserName: string)
  {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + receiverUserName, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start().catch(error => console.log(error));

      this.hubConnection.on('ReceiveMessageThread', messages => {
        this.messageThreadSource.next(messages);
      });

      this.hubConnection.on('NewMessage', message => {
        this.messageThread$.pipe(take(1)).subscribe(messages =>
          {
            /* Update the BehaviourSubject with a new message array
               that includes the message we've just received. */
            this.messageThreadSource.next([...messages, message]);
          });
      });

  }

  stopHubConnection()
  {
    if(this.hubConnection)
    {
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

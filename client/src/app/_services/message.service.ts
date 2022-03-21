import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
   baseUrl : string = environment.apiUrl ;

  constructor(private http: HttpClient) { }

  getMessages(pageNumber, pageSize, container)
  {
    let params = getPaginationHeaders(pageNumber , pageSize);
    params = params.append('Container', container);

    return getPaginatedResult<Message[]>(this.baseUrl + 'message', params, this.http);
  }

  sendMessage(username: string, content: string)
  {
    return this.http.post<Message>(this.baseUrl + 'message', {recipientUsername: username, content})
  }

  getMessageThread(username: string)
  {
    return this.http.get<Message[]>(this.baseUrl + 'message/thread/' + username);
  }


}

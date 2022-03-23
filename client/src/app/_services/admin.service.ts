import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  /* Get a list of users objects containing their
     username, Id and roles */
     
  getUsersWithRoles()
  {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles');
  }
}

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating Application';
  users: any;

  constructor(private http: HttpClient) {}
  ngOnInit() {
    this.getUsers();
  }

  //set the "users" property in our AppComponent class as the response we get back from our API server
  //display any errors we get back in the console
  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(response => {
      this.users = response;
    },error =>{
      console.log(error);
    });
  }
}

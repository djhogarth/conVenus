import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register', 
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any ={};
  @Output() cancelRegister = new EventEmitter();

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
      //close the form when a user regsiters
      this.cancel();
    },error => {
      console.log(error);
    })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}

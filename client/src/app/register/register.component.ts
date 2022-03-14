import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any ={};
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate: Date;

  constructor(public accountService: AccountService, private toastr: ToastrService, private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.intializeForm();
    this.maxDate = new Date;
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

  intializeForm() {
    this.registerForm = this.formBuilder.group(
      {
      username: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(10)]],
      alias: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
      gender: ['male'],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', [Validators.required, Validators.minLength(4)]],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(12)]],
      confirmPassword: ['', this.matchValues('password')]
      });

    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }

  matchValues(matchTo: string): ValidatorFn{
    return(control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null : {isMatching: true}
    }
  }

  register() {
    console.log(this.registerForm.value);
    // this.accountService.register(this.model).subscribe(response => {
    //   //close the form when a user regsiters
    //   this.cancel();
    // },error => {
    //   console.log(error);
    //   this.toastr.error(error.error);
    // })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}

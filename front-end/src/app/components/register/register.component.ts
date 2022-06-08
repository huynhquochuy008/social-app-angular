import { Component, ElementRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  constructor(private elRef: ElementRef, private accountService: AccountService, private _router: Router) {}

  ngOnInit(): void {}

  public value: {
    firstName: any;
    lastName: any;
    username: any;
    password: any;
    confirmPassword: any;
    dateOfBirth: any;
    gender: any;
    avatar: any;
    email: any;
    phone: any;
  } = {
    firstName: null,
    lastName: null,
    username: null,
    password: null,
    confirmPassword: null,
    dateOfBirth: null,
    gender: null,
    avatar: null,
    email: null,
    phone: null,
  };

  getInput(event: Event, value: any, str: string) {
    value[str] = (event.target as HTMLInputElement).value;
  }
  
  blur(value: any, str: string) {
    if (value[str] === null) value[str] = '';
  }

  navigateToRegister() {
    this._router.navigateByUrl('/login');
  }

  public onSubmitRegister(): void {
    let myDate = new Date(this.value.dateOfBirth);
    let date = myDate.toLocaleDateString('en-AU');

    // console.log(date);

    this.value.dateOfBirth = date;

    // console.log(this.value);

    this.accountService.register(this.value).subscribe(response => {
      // console.log(response);
      
      if(response.success == true)
      {
        this.navigateToRegister();
      }
    });
  }
}

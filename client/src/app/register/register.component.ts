import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  model: any = {};
@Input() usersFromHomeComponent: any;
@Output() cancelRegister = new EventEmitter();

constructor(private accountService: AccountService) {
}
  register(){
    //console.log(this.model);
    this.accountService.register(this.model).subscribe({
    next: response => {
      console.log(response);
      this.cancel();
    },
    error: error => console.log(error)
    })
  }
  cancel(){
    //console.log('cancelled');
    this.cancelRegister.emit(false);
  }
}

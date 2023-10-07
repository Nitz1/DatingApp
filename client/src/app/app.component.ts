import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/User';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating app';
  users: any;
  constructor(private http: HttpClient,private accountService: AccountService){
    //this.getUsers();
    this.setCurrentUser();
  }

  ngOnInit(): void {
    // this.getUsers();
     this.setCurrentUser();
  }
  // getUsers(){
  //   this.http.get('https://localhost:5001/api/users').subscribe({
  //     next: response => this.users = response,
  //     error : e => console.log(e),
  //     complete : () => console.log('Request has been completed')
  //   })
  // }
  setCurrentUser(){
    const userString = localStorage.getItem('user');
    if(!userString) return;
    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}

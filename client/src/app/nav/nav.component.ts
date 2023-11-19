import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, first, of } from 'rxjs';
import { User } from '../_models/User';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
model: any = {};
//currentUser$: Observable<User | null> = of(null);
  constructor(public accountService: AccountService,private router:Router,private toaster: ToastrService) {
    //this.currentUser$ = accountService.currentUser$;
  }

  ngOnInit() : void{
   //this.getCurrentUser();   
  }
 
  //  getCurrentUser(){
  //    this.accountService.currentUser$.subscribe({
  //     next: user=> this.loggedIn = !!user,
  //     error: error => console.log(error)
  //    })
  //  }

  login(){
    //console.log(this.model);
    this.accountService.login(this.model).subscribe({
      next: response => {
        //console.log(response);
        this.router.navigateByUrl('/members');
        this.toaster.success('Login successful');
        this.model = {};
        //this.loggedIn = true;
      },
      error: error =>{
        console.log(error);
      }
    })
  }

  logout(){
    //this.loggedIn = false;
    this.accountService.logout(); 
    this.router.navigateByUrl('/');
  }
}

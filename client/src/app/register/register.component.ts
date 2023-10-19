import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
@Input() usersFromHomeComponent: any;
@Output() cancelRegister = new EventEmitter();
registerForm: FormGroup = new FormGroup({});
maxDate: Date = new Date();

constructor(private accountService: AccountService, 
  private toaster: ToastrService, private fb: FormBuilder,
  private router: Router) {
   this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
}
  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
     /* this.registerForm = new FormGroup({
        username: new FormControl('',Validators.required),
        password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
        confirmPassword: new FormControl('',[Validators.required, this.matchValues('password')])
      });*/
      this.registerForm = this.fb.group({
        gender: ['male'],
        username: ['',Validators.required],
        knownAs: ['',Validators.required],
        dateOfBirth: ['',Validators.required],
        city: ['',Validators.required],
        country: ['',Validators.required],
        password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
        confirmPassword: ['',[Validators.required, this.matchValues('password')]]
      });
      this.registerForm.controls['password']?.valueChanges.subscribe({
        next: _ => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      })
  }

  matchValues(matchTo: string) : ValidatorFn{
    return((control:AbstractControl) => {
        return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching : true}
    })
  }
  register(){
    //console.log(this.model);
    //console.log(this.registerForm.value);
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value)
    const values = {...this.registerForm.value, dateOfBirth: dob};
     this.accountService.register(values).subscribe({
    next: response => {
      this.router.navigateByUrl('/members');
      //console.log(response);
      //this.cancel();
    },
    error: error => {
      console.log(error);
      this.toaster.error(error.error);
    }
    }) 
  }
  cancel(){
    //console.log('cancelled');
    this.cancelRegister.emit(false);
  }

  getDateOnly(dob: string | undefined){
    if (!dob) return;
    var theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).
    toISOString().slice(0,10);
  }
}

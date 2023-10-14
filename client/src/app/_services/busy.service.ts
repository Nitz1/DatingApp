import { Injectable } from '@angular/core';
import { NgxSpinner, NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = 0;
  constructor(private spinnerService: NgxSpinnerService) { }

  busy(){
    debugger;
    this.busyRequestCount++;
    try{
      this.spinnerService.show(undefined, {
        type: 'line-scale-party',
        bdColor: 'rgba(255,255,255,0)',
        color: '#333333'
      });
    }
    catch(e){
      debugger;
      console.log(e);
    }
  
  }

  idle(){
    this.busyRequestCount--;
    if(this.busyRequestCount <=0){
      this.busyRequestCount=0;
      this.spinnerService.hide();
    }
  }
}

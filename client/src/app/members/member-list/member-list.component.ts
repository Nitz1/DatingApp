import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { User } from 'src/app/_models/User';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
    members: Member[] = [];
    pagination: Pagination | undefined;
    userParams: UserParams | undefined ;
    genderList = [{value: 'male', display: 'Male'}, {value:'female', display: 'Female'}]; 
    //members$: Observable<Member[]> | undefined;
    constructor(private memberService: MemberService) { 
      this.userParams = memberService.getParams();
    }

  ngOnInit(): void {
    //this.loadMembers();
    //this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }
    // loadMembers(){
    //     this.memberService.getMembers().subscribe({
    //       next: response => this.members = response
    //     });
    // }
      loadMembers(){
        if(this.userParams){
          this.memberService.setParams(this.userParams);
          this.memberService.getMembers(this.userParams).subscribe({
            next: response => {
              if(response.result && response.pagination){
                this.members = response.result;
                this.pagination = response.pagination;
              }
             
            }
        });
      }
    }

    pageChanged(event: any){
      if(this.userParams && this.userParams?.pageNumber !== event.page){
        this.userParams.pageNumber = event.page;
        this.memberService.setParams(this.userParams);  
        this.loadMembers();
      }
     
    }

    resetFilters(){
        //this.userParams = new UserParams(this.user);
        this.userParams= this.memberService.resetUserParams();
        this.loadMembers();
    }

}

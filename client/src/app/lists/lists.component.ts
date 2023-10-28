import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MemberService } from '../_services/member.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  public members: Member[] | undefined;
  predicate = 'liked';
  pageNumber =1;
  pageSize =5;
  pagination: Pagination | undefined;
  constructor(private memberService: MemberService) {

  }
  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.memberService.getLikes(this.predicate,this.pageNumber,this.pageSize).subscribe({
      next: response => {
          debugger;
            this.members = response.result;
            if(!response.result?.length) {
              this.pagination = undefined;
              return;
            }
            this.pagination = response.pagination;
      }
    });
  }

  pageChanged(event: any){
    if( this.pageNumber !== event.page){
      this.pageNumber = event.page;
      //this.memberService.setParams(this.userParams);  
      this.loadLikes();
    }
   
  }
}

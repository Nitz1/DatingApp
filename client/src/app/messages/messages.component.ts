import { Component, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit{
  pageNumber =1;
  pageSize =5;  
  container = 'Unread';
  messages: Message[] | undefined;
  pagination: Pagination | undefined;
  loading = false;
  constructor(private messageService: MessageService) {
  }
  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading=true;
    this.messageService.getMessage(this.pageNumber,this.pageSize, this.container).subscribe({
      next: response =>{
         this.pagination = response.pagination;
          this.messages=response.result;
          this.loading = false;
      }
    })
  }

  pageChanged(event: any){
    if( this.pageNumber !== event.page){
      this.pageNumber = event.page;
      //this.memberService.setParams(this.userParams);  
      this.loadMessages();
    }
  }

  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe({
      next: _ => {
          this.messages?.splice(this.messages.findIndex(x=>x.id===id),1);
      }
    })
  }
}

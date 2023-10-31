import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-detail',
  standalone:true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule,TabsModule,GalleryModule,TimeagoModule,MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit {
  member: Member = {} as Member;
  images: GalleryItem[]=[];
  @ViewChild('memberTabs',{static:true}) memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  messages: Message[] = [];


  constructor(private memberService: MemberService, private route: ActivatedRoute,
              private messageService: MessageService) {
    
  }
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data =>
      { 
        debugger;
        this.member = data['member']

      }
    });
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })
    this.loadImages();
    //this.loadMembers();
  }

  loadMembers(){
    const userName=this.route.snapshot.paramMap.get('username');
    if(!userName) return;
    this.memberService.getMember(userName).subscribe({
      next: response => {
        this.member = response;
        this.loadImages();
      }
    })
  }

  onTabActivated(activeTab: TabDirective){
    this.activeTab = activeTab;
    if(this.member){
      if(this.activeTab.heading === 'Messages'){
        this.getMessageThread();
      }
    }
  }

  getMessageThread(){
    if(this.member){
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: response => {
          this.messages = response;
        }
      });
    }
 
  }

  loadImages(){
    if(!this.member) return;
      for(const photo of this.member.photos)
      {
        this.images.push(new ImageItem({src: photo.url, thumb:photo.url }));
        this.images.push(new ImageItem({src: photo.url, thumb:photo.url }));
      }
  }

  selectTab(heading: string){
    if(this.memberTabs){
      this.memberTabs.tabs.find(x=>x.heading===heading)!.active =true;
    }
  
  }
}

import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-detail',
  standalone:true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule,TabsModule,GalleryModule,TimeagoModule]
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined;
  images: GalleryItem[]=[];

  constructor(private memberService: MemberService, private route: ActivatedRoute) {
    
  }
  ngOnInit(): void {
    this.loadMembers();
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

  loadImages(){
    if(!this.member) return;
      for(const photo of this.member.photos)
      {
        this.images.push(new ImageItem({src: photo.url, thumb:photo.url }));
        this.images.push(new ImageItem({src: photo.url, thumb:photo.url }));
      }
  }
}

import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { first } from 'rxjs';
import { User } from 'src/app/_models/User';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
 @Input() member: Member | undefined;
 uploader: FileUploader | undefined;
 hasBaseDropZoneOver = false;
 baseUrl = environment.apiUrl;
 user: User | undefined;


 constructor(private accountService: AccountService, private memberService: MemberService) {  
    this.accountService.currentUser$.pipe(first()).subscribe({
    next:  user => {
      if(user){
        this.user = user;
      }
    }
   })
 }

 ngOnInit(): void {
   this.initializeUploader();
 }

 fileOverBase(e: any){
   this.hasBaseDropZoneOver = e;
 }

 initializeUploader(){
  this.uploader = new FileUploader({
    url : this.baseUrl + 'users/add-photo',
    authToken: 'Bearer ' + this.user?.token,
    allowedFileType: ['image'],
    isHTML5: true,
    removeAfterUpload: true,
    autoUpload: false,
    maxFileSize: 10*1024*1024
  })

  this.uploader.onAfterAddingFile = (file) => {
    file.withCredentials = false;
  }

  this.uploader.onSuccessItem = (item, response, status, headers) =>{
      if(response) {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);
      }
  }
 }

 setMainPhoto(photo: Photo){
      this.memberService.setMainPhoto(photo.id).subscribe({
        next: _ => {
          if(this.user && this.member){
            this.user.photoUrl = photo.url;            
            this.member.photoUrl = photo.url;
            this.accountService.setCurrentUser(this.user);
            this.member.photos.forEach(p => {
              if(p.isMain) p.isMain =false;
              if(p.id == photo.id) p.isMain = true;
            })
            
          }
        }
      })
 }

 deletePhoto(photoId: number){
    this.memberService.deletePhoto(photoId).subscribe({
      next: _ => {
        if(this.member)
        this.member.photos = this.member.photos.filter(x=> x.id !== photoId);
      }
    })
 }
}

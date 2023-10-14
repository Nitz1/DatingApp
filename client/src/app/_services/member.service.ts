import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Observable, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.apiUrl; 
  members: Member[] = [];
  constructor(private http: HttpClient) { }

  getMembers(){
    if(this.members.length>0) return of(this.members);

      return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
        map(members=>{
          this.members = members;
          return this.members;
        }));
  }

  getMember(username: string){
    debugger;
    //if(this.members.length>0) return this.members.find(x=>x.userName==username);
    const member = this.members.find(x=>x.userName==username);
    if(member) return of(member);

    return this.http.get<Member> (this.baseUrl + 'users/' + username);
  }

  /*getMembers(){
    return this.http.get<Member[]>(this.baseUrl + 'users',this.getHttpOptions());
  }

  getMember(username: string){
    return this.http.get<Member> (this.baseUrl + 'users/' + username, this.getHttpOptions())
  }

  getHttpOptions(){
    var userString = localStorage.getItem('user');
    if(!userString) return;
    var user = JSON.parse(userString);
    return {
       headers: new HttpHeaders({
        Authorization: 'Bearer ' + user.token 
       })
    }
  }*/
  
  updateMember(member: Member){
        //  return this.http.put(this.baseUrl + "users",member);
    return this.http.put(this.baseUrl + "users",member).pipe(
      map(_ =>{
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      }));

  }
}

import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.apiUrl; 
  constructor(private http: HttpClient) { }

  getMembers(){
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMember(username: string){
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
}

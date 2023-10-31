import { HttpClient, HttpHandler, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Observable, map, of, take } from 'rxjs';
import { PaginationResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/User';
import { getPaginationHeaders, getPaginationResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.apiUrl; 
  members: Member[] = [];
  userParams: UserParams | undefined ;
  user: User | undefined;
  memberCache = new Map();
  constructor(private http: HttpClient, private accountService: AccountService) {
    accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user){
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    });
   }

   getParams(){
    return this.userParams;
   }

   setParams(userParams: UserParams){
    this.userParams = userParams;
   }

   resetUserParams(){
    if(this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
   }

  getMembers<T>(userParams: UserParams){ //Member[]
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return of(response);
    let params: HttpParams = getPaginationHeaders(userParams.pageNumber,userParams.pageSize);
      params = params.append("minAge", userParams.minAge);
      params = params.append("maxAge", userParams.maxAge);
      params = params.append("gender", userParams.gender);
      params = params.append("orderBy", userParams.orderBy);
    return getPaginationResult<Member[]>(this.baseUrl + 'users', params,this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
    //if(this.members.length>0) return of(this.members);

      // return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      //   map(members=>{
      //     this.members = members;
      //     return this.members;
      //   }));

    
  }

  // private getPaginationHeaders(pageNumber: number, pageSize: number) {
  //   let params: HttpParams = new HttpParams();
  //     params = params.append("pageNumber", pageNumber);
  //     params = params.append("pageSize", pageSize);
    
  //     return params;
  // }

  // private getPaginationResult<T>(url: string, params: HttpParams) {
  //   const paginationResult: PaginationResult<T> = new PaginationResult<T>;
  //   return this.http.get<T>(url, { observe: 'response', params }).pipe(
  //     map(response => {
  //       if (response.body) {
  //         paginationResult.result = response.body;
  //       }
  //       const pagination = response.headers.get('Pagination');
  //       if (pagination) {
  //         paginationResult.pagination = JSON.parse(pagination);
  //       }
  //       return paginationResult;
  //     }));
  // }

  getMember(username: string){
    //if(this.members.length>0) return this.members.find(x=>x.userName==username);   
    const member = [...this.memberCache.values()].
    reduce((arr,elem) => arr.concat(elem.result),[])
    .find((member: Member) => member.userName === username);

    //const member = this.members.find(x=>x.userName==username);
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

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId,{});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl +'users/delete-photo/' +photoId,{});
  }

  addLike(username: string){
    return this.http.post(this.baseUrl + 'likes/' + username,{});
  }

  getLikes(predicate: string,pageNumber: number, pageSize: number){
    let params: HttpParams = getPaginationHeaders(pageNumber,pageSize);
    params= params.append("predicate", predicate);
    //return this.http.get<Member[]>(this.baseUrl + 'likes?predicate='+ predicate);
    return getPaginationResult<Member[]>(this.baseUrl + 'likes',params,this.http);
  }
}

import { HttpClient, HttpSentEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginationHeaders, getPaginationResult } from './paginationHelper';
import { Message } from '../_models/message';
import { User } from '../_models/User';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, first, take } from 'rxjs';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
baseUrl = environment.apiUrl;
hubUrl = environment.hubUrl;
private hubConnection?: HubConnection;
private messageThreadSource = new BehaviorSubject<Message[]>([]);
messageThreads$ = this.messageThreadSource.asObservable();


  constructor(private http: HttpClient) { } 

  getMessage(pageNumber: number, pageSize: number,container: string){
    let params = getPaginationHeaders(pageNumber,pageSize);
    params= params.append('Container',container);
    return getPaginationResult<Message[]>(this.baseUrl+ 'messages',params,this.http);
  }

  getMessageThread(username: string){
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/'+ username);
  }

  async sendMessage(username: string,content: string){
    //return this.http.post<Message>(this.baseUrl + 'messages',{recepientUsername:username, content});
    return this.hubConnection?.invoke('SendMessage',{recepientUsername: username, content})
    .catch(error=> {
      console.log('in send message');
      console.log(error);
    });
  }

  deleteMessage(id: number){
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }

  createHubConnection(user: User, anotherUsername: string){
     this.hubConnection = new HubConnectionBuilder()
                          .withUrl(this.hubUrl + 'message?user=' + anotherUsername,
                          { accessTokenFactory: () => user.token }).
                          withAutomaticReconnect().build();
    this.hubConnection.start().catch(error=> console.log(error));

    this.hubConnection.on('ReceiveMessageThread', messages =>{
        this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('UpdatedGroup',(group: Group)=>{
      if(group.connections.some(x=>x.username===anotherUsername)){
        this.messageThreads$.pipe(take(1)).subscribe({
          next: messages => {
            messages.forEach(message => {
              if(!message.dateRead){
                message.dateRead = new Date(Date.now());
              }
            })
            this.messageThreadSource.next([...messages]);
          }
        })
      }
    });

    this.hubConnection.on('NewMessage', message =>{
      this.messageThreads$.pipe(first()).subscribe({
        next: messages =>{
          this.messageThreadSource.next([...messages,message]);
        }
      })
      
  })
  }

  stopHubConnection()
  {
    if(this.hubConnection){
      this.hubConnection.stop();
    }
  }
}

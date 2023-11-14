﻿using API.Entities;

namespace API;

public class Message
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername {get;set;}
    public AppUser Sender {get;set;}
    public int RecepientId { get; set; }
    public string RecepientUsername {get;set;}
    public AppUser Recepient {get;set;}
    public string Content {get;set;}
    public DateTime MessageSent {get;set;} = DateTime.UtcNow;
    public DateTime? DateRead {get;set;}
    public bool SenderDeleted {get;set;}
    public bool RecepientDeleted {get;set;}
}
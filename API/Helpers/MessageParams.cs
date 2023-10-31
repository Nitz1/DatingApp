namespace API;

public class MessageParams : PaginationParams
{
    public string Container { get; set; } = "Unread";
    public string UserName {get;set;}
}

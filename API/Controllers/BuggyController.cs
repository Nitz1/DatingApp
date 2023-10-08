using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class BuggyController : BaseApiController
{
    private readonly DataContext _context;

    public BuggyController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("auth")]
    [Authorize]
    public ActionResult<string> GetSecrets(){
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser>  GetNotFound(){
       var thing =  _context.Users.Find(-1);
       if(thing ==null) return NotFound();

       return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        //try{
        var thing =  _context.Users.Find(-1);
       
        var thingToReturn = thing.ToString();

        return thingToReturn;
        // }
        // catch(Exception ex)
        // {
        //     return StatusCode(500, "server says no");
        // }
       
    }
    
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
       return BadRequest("This was not a goood request"); 
    }
}

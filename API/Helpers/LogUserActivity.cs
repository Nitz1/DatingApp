using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace API;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;  

        var userId = resultContext.HttpContext.User.GetUserId();
        // var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user =await uow.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;
        //await repo.SaveAllAsync();
        await uow.Complete();
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;

namespace Turnament.Authorization;

public class TournamentMatchAuthorizationAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var matchId = (int)context.ActionArguments["matchId"];
        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var match = await dbContext.Matches
            .Include(m => m.Tournament)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null)
        {
            context.Result = new NotFoundResult();
            return;
        }

        // Sprawdź czy użytkownik jest organizatorem turnieju
        if (match.Tournament.CreatorId != int.Parse(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
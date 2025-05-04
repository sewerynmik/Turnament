using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Services;

namespace Turnament.Controllers;

public class TournamentBracketController(TournamentBracketService bracketService, AppDbContext context)
    : Controller
{
    public async Task<IActionResult> Generate(int tournamentId)
    {
        try
        {
            await bracketService.GenerateBracketAsync(tournamentId);
            return RedirectToAction("ViewBracket", new { tournamentId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Tournaments");
        }
    }

    [Route("Tournament/{tournamentId}/Bracket")]
    public async Task<IActionResult> ViewBracket(int tournamentId)
    {
        var matches = await context.Matches
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.Id)
            .ToListAsync();

        return View(matches);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMatchResult(int matchId, int winnerId)
    {
        try
        {
            await bracketService.UpdateMatchResultAsync(matchId, winnerId);
            var match = await context.Matches.FindAsync(matchId);
            return RedirectToAction("ViewBracket", new { tournamentId = match.TournamentId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Tournaments");
        }
    }
}
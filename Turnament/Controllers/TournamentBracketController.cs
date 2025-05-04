using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnament.Authorization;
using Turnament.Data;
using Turnament.Services;
using System.Security.Claims;

namespace Turnament.Controllers;

public partial class TournamentBracketController(TournamentBracketService bracketService, AppDbContext context)
    : Controller
{
    [TournamentCreatorAuthorization]
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
        var tournament = await context.Tournaments
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }

        var matches = await context.Matches
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.Id)
            .ToListAsync();

        // Sprawdź czy zalogowany użytkownik jest organizatorem
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ViewBag.IsOrganizer = userId != null && tournament.CreatorId == int.Parse(userId);

        return View(matches);
    }

    [HttpPost]
    [TournamentMatchAuthorization]
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
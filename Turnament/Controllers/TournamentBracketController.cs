using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Services;

namespace Turnament.Controllers;

public class TournamentBracketController : Controller
{
    private readonly TournamentBracketService _bracketService;
    private readonly AppDbContext _context;

    public TournamentBracketController(TournamentBracketService bracketService, AppDbContext context)
    {
        _bracketService = bracketService;
        _context = context;
    }

    public async Task<IActionResult> Generate(int tournamentId)
    {
        try
        {
            await _bracketService.GenerateBracketAsync(tournamentId);
            return RedirectToAction("ViewBracket", new { tournamentId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Tournaments");
        }
    }

    public async Task<IActionResult> ViewBracket(int tournamentId)
    {
        var matches = await _context.Matches
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
            await _bracketService.UpdateMatchResultAsync(matchId, winnerId);
            var match = await _context.Matches.FindAsync(matchId);
            return RedirectToAction("ViewBracket", new { tournamentId = match.TournamentId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Tournaments");
        }
    }
}
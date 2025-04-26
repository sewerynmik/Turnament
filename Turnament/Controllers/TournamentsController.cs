using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;
using Turnament.ViewModel.Tournament;

namespace Turnament.Controllers;

[Route("Tournament")]
public class TournamentsController(AppDbContext context) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var appDbContext = context.Tournaments
            .Include(t => t.BracketType)
            .Include(t => t.Sport);

        return View(await appDbContext.ToListAsync());
    }

    [Route("{id:int}/Details")]
    public async Task<IActionResult> Details(int? id)
    {
        var tournament = await context.Tournaments
            .Include(t => t.BracketType)
            .Include(t => t.Creator)
            .Include(t => t.Sport)
            .Include(t => t.WinnerTeam)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (tournament == null)
        {
            return NotFound();
        }

        return View(tournament);
    }

    [Authorize]
    [HttpGet("Create")]
    public IActionResult Create()
    {
        ViewData["BracketTypeId"] = new SelectList(context.BracketTypes, "Id", "Name");
        ViewData["SportId"] = new SelectList(context.Sports, "Id", "Name");

        return View();
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        ViewData["BracketTypeId"] = new SelectList(context.BracketTypes, "Id", "Id", model.BracketTypeId);
        ViewData["SportId"] = new SelectList(context.Sports, "Id", "Id", model.SportId);

        if (model.StartDate <= DateTime.UtcNow)
        {
            ModelState.AddModelError(nameof(model.StartDate), "Turniej powinien zaczynać później niż teraz");
            return View(model);
        }

        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "Data końca musi być późniejsza niż data rozpoczęcia");
            return View(model);
        }

        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (value == null)
        {
            return NotFound();
        }

        var tournament = new Tournament
        {
            Name = model.Name,
            Description = model.Description,
            SportId = model.SportId,
            BracketTypeId = model.BracketTypeId,
            CreatorId = Int32.Parse(value),
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        await context.Tournaments.AddAsync(tournament);
        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int? id)
    {
        var tournament = await context.Tournaments.FindAsync(id);

        if (tournament == null)
        {
            return NotFound();
        }

        ViewData["BracketTypeId"] = new SelectList(context.BracketTypes, "Name", "Name", tournament.BracketTypeId);
        ViewData["SportId"] = new SelectList(context.Sports, "Name", "Name", tournament.SportId);

        var winner = await context.Tournaments
            .Include(t => t.TournamentTeams)
            .ThenInclude(t => t.Team)
            .Where(t => t.Id == id)
            .SelectMany(t => t.TournamentTeams.Select(tt => tt.Team))
            .ToListAsync();

        if (winner.Count != 0)
        {
            ViewData["WinnerTeamId"] = new SelectList(winner, "Id", "Id", tournament.WinnerTeamId);
        }

        var model = new EditViewModel
        {
            Id = tournament.Id,
            Name = tournament.Name,
            BracketTypeId = tournament.BracketTypeId,
            Description = tournament.Description,
            SportId = tournament.SportId,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
        };

        return View(model);
    }

    [Authorize]
    [HttpPost("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Name,Description,SportId,BracketTypeId,CreatorId,StartDate,EndDate,WinnerTeamId")]
        Tournament tournament)
    {
        if (id != tournament.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(tournament);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(tournament.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["BracketTypeId"] = new SelectList(context.BracketTypes, "Id", "Id", tournament.BracketTypeId);
        ViewData["CreatorId"] = new SelectList(context.Users, "Id", "Id", tournament.CreatorId);
        ViewData["SportId"] = new SelectList(context.Sports, "Id", "Id", tournament.SportId);
        ViewData["WinnerTeamId"] = new SelectList(context.Teams, "Id", "Id", tournament.WinnerTeamId);
        return View(tournament);
    }

    [Authorize]
    [HttpGet("{id:int}/Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        var tournament = await context.Tournaments
            .Include(t => t.BracketType)
            .Include(t => t.Creator)
            .Include(t => t.Sport)
            .Include(t => t.WinnerTeam)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (tournament == null)
        {
            return NotFound();
        }

        return View(tournament);
    }

    [Authorize]
    [HttpPost("{id:int}/Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tournament = await context.Tournaments.FindAsync(id);

        if (tournament != null)
        {
            context.Tournaments.Remove(tournament);
        }

        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet("{id:int}/Teams")]
    public async Task<IActionResult> Teams(int id)
    {
        return View();
    }

    private bool TournamentExists(int id)
    {
        return context.Tournaments.Any(e => e.Id == id);
    }
}
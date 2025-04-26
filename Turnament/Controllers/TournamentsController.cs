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

        ViewData["BracketTypeId"] = new SelectList(context.BracketTypes, "Id", "Name", tournament.BracketTypeId);
        ViewData["SportId"] = new SelectList(context.Sports, "Id", "Name", tournament.SportId);

        var winner = await context.Tournaments
            .Include(t => t.TournamentTeams)
            .ThenInclude(t => t.Team)
            .Where(t => t.Id == id)
            .SelectMany(t => t.TournamentTeams.Select(tt => tt.Team))
            .ToListAsync();

        if (winner.Any())
        {
            var items = winner.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = t.Id == tournament.WinnerTeamId
            }).ToList();
    
            // Dodaje opcję "Brak zwycięzcy"
            items.Insert(0, new SelectListItem { Value = "", Text = "-- Brak zwycięzcy --" });
    
            ViewData["WinnerTeamId"] = items;
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
            WinnerTeamId = tournament.WinnerTeamId,
        };

        return View(model);
    }

    [Authorize]
    [HttpPost("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id, EditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["BracketTypeId"] = new SelectList(context.BracketTypes, "Id", "Name", model.BracketTypeId);
            ViewData["SportId"] = new SelectList(context.Sports, "Id", "Name", model.SportId);
            
            return View(model);
        }

        
        if (id != model.Id)
        {
            return NotFound();
        }
        
        var tournament = await context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);

        if (tournament == null)
        {
            return NotFound();
        }
        
        if (model.WinnerTeamId.HasValue)
        {
            var teamExists = await context.TournamentTeams
                .AnyAsync(tt => tt.TournamentId == id && tt.TeamId == model.WinnerTeamId);
            
            if (!teamExists)
            {
                ModelState.AddModelError(nameof(model.WinnerTeamId), "Wybrana drużyna nie jest uczestnikiem turnieju");
                return View(model);
            }
        }

        
        tournament.Name = model.Name;
        tournament.Description = model.Description;
        tournament.SportId = model.SportId;
        tournament.BracketTypeId = model.BracketTypeId;
        tournament.StartDate = model.StartDate;
        tournament.EndDate = model.EndDate;
        tournament.WinnerTeamId = model.WinnerTeamId;

        await context.SaveChangesAsync();
        
        return RedirectToAction("Index");
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
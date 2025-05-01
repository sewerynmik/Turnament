using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;

namespace Turnament.Controllers;

[Route("Match")]
public class MatchesController(AppDbContext context) : Controller
{
    // GET: Match
    public async Task<IActionResult> Index()
    {
        var appDbContext = context.Matches.Include(m => m.Team1).Include(m => m.Team2).Include(m => m.Tournament).Include(m => m.Winner);
        return View(await appDbContext.ToListAsync());
    }

    // GET: Match/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var match = await context.Matches
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .Include(m => m.Tournament)
            .Include(m => m.Winner)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (match == null)
        {
            return NotFound();
        }

        return View(match);
    }

    // GET: Match/Create
    public IActionResult Create()
    {
        ViewData["Team1Id"] = new SelectList(context.Teams, "Id", "Id");
        ViewData["Team2Id"] = new SelectList(context.Teams, "Id", "Id");
        ViewData["TournamentId"] = new SelectList(context.Tournaments, "Id", "Id");
        ViewData["WinnerId"] = new SelectList(context.Teams, "Id", "Id");
        return View();
    }

    // POST: Match/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,TournamentId,Round,Team1Id,Team2Id,WinnerId,ScheduledAt,FinishedAt")] Match match)
    {
        if (ModelState.IsValid)
        {
            context.Add(match);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["Team1Id"] = new SelectList(context.Teams, "Id", "Id", match.Team1Id);
        ViewData["Team2Id"] = new SelectList(context.Teams, "Id", "Id", match.Team2Id);
        ViewData["TournamentId"] = new SelectList(context.Tournaments, "Id", "Id", match.TournamentId);
        ViewData["WinnerId"] = new SelectList(context.Teams, "Id", "Id", match.WinnerId);
        return View(match);
    }

    // GET: Match/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var match = await context.Matches.FindAsync(id);
        if (match == null)
        {
            return NotFound();
        }
        ViewData["Team1Id"] = new SelectList(context.Teams, "Id", "Id", match.Team1Id);
        ViewData["Team2Id"] = new SelectList(context.Teams, "Id", "Id", match.Team2Id);
        ViewData["TournamentId"] = new SelectList(context.Tournaments, "Id", "Id", match.TournamentId);
        ViewData["WinnerId"] = new SelectList(context.Teams, "Id", "Id", match.WinnerId);
        return View(match);
    }

    // POST: Match/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TournamentId,Round,Team1Id,Team2Id,WinnerId,ScheduledAt,FinishedAt")] Match match)
    {
        if (id != match.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(match);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchExists(match.Id))
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
        ViewData["Team1Id"] = new SelectList(context.Teams, "Id", "Id", match.Team1Id);
        ViewData["Team2Id"] = new SelectList(context.Teams, "Id", "Id", match.Team2Id);
        ViewData["TournamentId"] = new SelectList(context.Tournaments, "Id", "Id", match.TournamentId);
        ViewData["WinnerId"] = new SelectList(context.Teams, "Id", "Id", match.WinnerId);
        return View(match);
    }

    // GET: Match/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var match = await context.Matches
            .Include(m => m.Team1)
            .Include(m => m.Team2)
            .Include(m => m.Tournament)
            .Include(m => m.Winner)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (match == null)
        {
            return NotFound();
        }

        return View(match);
    }

    // POST: Match/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var match = await context.Matches.FindAsync(id);
        if (match != null)
        {
            context.Matches.Remove(match);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MatchExists(int id)
    {
        return context.Matches.Any(e => e.Id == id);
    }
}
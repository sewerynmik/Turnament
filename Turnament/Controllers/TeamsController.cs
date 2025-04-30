using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;
using Turnament.ViewModel.Team;

namespace Turnament.Controllers;

[Route("Teams")]
public class TeamsController(AppDbContext context) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var appDbContext = context.Teams
            .Include(t => t.Creator);

        return View(await appDbContext.ToListAsync());
    }

    [Route("{id:int}/Details")]
    public async Task<IActionResult> Details(int? id)
    {
        var team = await context.Teams
            .Include(t => t.Creator)
            .Include(t => t.Members)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (team == null) return NotFound();

        return View(team);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        var creatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (creatorId == null) return NotFound();

        var team = new Team
        {
            Name = model.Name,
            CreatorId = int.Parse(creatorId),
            CreatedAt = DateTime.UtcNow,
        };

        await context.Teams.AddAsync(team);
        await context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = team.Id });
    }

    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int? id)
    {
        var team = await context.Teams.FindAsync(id);

        if (team == null) return NotFound();

        var model = new EditViewModel
        {
            Id = team.Id,
            Name = team.Name
        };

        return View(model);
    }

    [HttpPost("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id, EditViewModel model)
    {
        if (id != model.Id) return NotFound();

        var team = await context.Teams.FirstOrDefaultAsync(t => t.Id == id);

        if (team == null) return NotFound();

        team.Name = model.Name;

        await context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = team.Id });
    }

    [HttpGet("{id:int}/Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        var team = await context.Teams
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (team == null) return NotFound();

        return View(team);
    }
    
    [HttpPost("{id:int}/Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var team = await context.Teams.FindAsync(id);
        
        if (team != null) context.Teams.Remove(team);

        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Route("{id}/Members")]
    public async Task<IActionResult> TeamMembers(int? id)
    {
        var team = await context.Teams
            .Include(t => t.Members)
            .ThenInclude(teamMember => teamMember.User)
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (team == null) return NotFound();

        var teamMembers = team.Members.Select(m => m.User).ToList() ?? throw new ArgumentNullException(nameof(id));

        teamMembers.Add(team.Creator);

        return View(teamMembers);
    }

    [HttpGet("{id:int}/Invitations")]
    public async Task<IActionResult> TeamInvitation(int? id)
    {
        var team = await context.Teams
            .Include(t => t.Invitations)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (team == null) return NotFound();
        
        
        return View();
    }
}
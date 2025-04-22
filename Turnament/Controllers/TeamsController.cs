using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;
using Turnament.ViewModel.Team;

namespace Turnament.Controllers
{
    public class TeamsController(AppDbContext context) : Controller
    {
        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var appDbContext = context.Teams.Include(t => t.Creator);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Teams/Details/5
        [Route("Teams/{id}/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await context.Teams
                .Include(t => t.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            var creatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (creatorId == null)
            {
                return NotFound();
            }

            var team = new Team
            {
                Name = model.Name,
                CreatorId = int.Parse(creatorId),
                CreatedAt = DateTime.UtcNow,
            };

            await context.Teams.AddAsync(team);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            var model = new EditViewModel
            {
                Id = team.Id,
                Name = team.Name
            };

            return View(model);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var team = await context.Teams.FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            team.Name = model.Name;

            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await context.Teams
                .Include(t => t.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await context.Teams.FindAsync(id);
            if (team != null)
            {
                context.Teams.Remove(team);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return context.Teams.Any(e => e.Id == id);
        }

        [Route("/Teams/{id}/Members")]
        public async Task<IActionResult> TeamMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await context.Teams
                .Include(t => t.Members)
                .ThenInclude(teamMember => teamMember.User)
                .Include(t => t.Creator)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            var teamMembers = team.Members.Select(m => m.User).ToList() ?? throw new ArgumentNullException(nameof(id));

            teamMembers.Add(team.Creator);

            return View(teamMembers);
        }

        public async Task<IActionResult> TeamInvitation()
        {
            return View();
        }
    }
}
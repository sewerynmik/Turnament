using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;
using Turnament.ViewModel.User;

namespace Turnament.Controllers;

public class UsersController(AppDbContext context) : Controller
{
    
    // GET: Users
    public async Task<IActionResult> Index()
    {
        return View(await context.Users.ToListAsync());
    }

    // GET: Users/Details/5
    [Route("/Users/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    /*// GET: Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Username,Email,PassHash,CreatedAt")] User user)
    {
        if (ModelState.IsValid)
        {
            context.Add(user);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }*/

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await context.Users.FindAsync(id);
        
        if (user == null)
        {
            return NotFound();
        }

        var model = new EditViewModel
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username
        };
        
        return View(model);
    }

    // POST: Users/Edit/5
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

        if (await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Id != id) != null)
        {
            ModelState.AddModelError("", "Użytkownik z takim adresem email jest zarejestrowany.");
            return View(model);
        }
        
        if (await context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.Id != id) != null)
        {
            ModelState.AddModelError("", "Użytkownik z takim adresem email jest zarejestrowany.");
            return View(model);
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        user.Username = model.Username;
        user.Email = model.Email;
        user.PassHash = BCrypt.Net.BCrypt.HashPassword(model.Pass);

        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(int id)
    {
        return context.Users.Any(e => e.Id == id);
    }
        
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            ModelState.AddModelError(nameof(model.Email), "Użytkowanik z takim adresem email nie istnieje.");
            return View(model);
        }

        var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Pass, user.PassHash);

        if (!isPasswordCorrect)
        {
            ModelState.AddModelError(nameof(model.Pass), "Nieprawidłowe hasło");
            return View(model);
        }

        await LoginUser(user.Id, user.Username, user.Email);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyAuth");

        return RedirectToAction("Login", "Users");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        if (await context.Users.FirstOrDefaultAsync(u => u.Username == model.Username) != null)
        {
            ModelState.AddModelError("", "Nazwa użytkownika jest już zajęta.");
            return View();
        }

        if (await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email) != null)
        {
            ModelState.AddModelError("", "Email jest już zarejestrowany");
            return View();
        }

        if (model.Username == null || model.Email == null || model.Pass == null)
        {
            ModelState.AddModelError("", "Usp... Coś poszło nie tak spróbuj później");
            return View();
        }

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PassHash = BCrypt.Net.BCrypt.HashPassword(model.Pass),
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        await LoginUser(user.Id, user.Username, user.Email);

        return RedirectToAction("Index", "Home");
    }

    private async Task LoginUser(int id, string username, string email)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, username)
        };

        var identity = new ClaimsIdentity(claims, "MyAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("MyAuth", principal);
    }
}
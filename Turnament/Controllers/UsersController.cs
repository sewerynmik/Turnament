using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnament.Data;
using Turnament.Models;

namespace Turnament.Controllers;

public class UsersController(AppDbContext context) : Controller
{
    // GET: Users
    public async Task<IActionResult> Index()
    {
        return View(await context.Users.ToListAsync());
    }

    // GET: Users/Details/5
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

    // GET: Users/Create
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
    }

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
        return View(user);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,PassHash,CreatedAt")] User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(user);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
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
        return View(user);
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
    public async Task<IActionResult> Login(string email, string pass)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(pass, user.PassHash);

        if (!isPasswordCorrect)
        {
            return RedirectToAction("Index", "Home");
        }
        
        HttpContext.Session.SetString("userId", user.Id.ToString());
        
        return View();
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("userId");

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string email, string pass)
    {
        if (await context.Users.FirstOrDefaultAsync(u => u.Username == username) != null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (await context.Users.FirstOrDefaultAsync(u => u.Email == email) != null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PassHash = BCrypt.Net.BCrypt.HashPassword(pass),
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}
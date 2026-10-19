using HomeWork_66_Sydykbekov_Talgat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class UserController : Controller
{
    private MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public UserController(MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userid = int.Parse(_userManager.GetUserId(User));
        var userr = await _userManager.GetUserAsync(User);
            //var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userid);
      return View(userr);
    }
    
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string userName,
        string email,
        string phoneNumber)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(userName))
            ModelState.AddModelError("", "Введите имя.");

        if (string.IsNullOrWhiteSpace(email) ||
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                .IsValid(email))
        {
            ModelState.AddModelError("", "Введите корректный email.");
        }

        if (!ModelState.IsValid)
            return View("Index", user);

        user.UserName = userName.Trim();

        var newEmail = email.Trim();

        if (user.Email != newEmail)
        {
            user.Email = newEmail;
            user.EmailConfirmed = false;
        }

        var newPhone = phoneNumber?.Trim();

        if (user.PhoneNumber != newPhone)
        {
            user.PhoneNumber = newPhone;
            user.PhoneNumberConfirmed = false;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return RedirectToAction(nameof(Index));

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View("Index", user);
    }
    
}
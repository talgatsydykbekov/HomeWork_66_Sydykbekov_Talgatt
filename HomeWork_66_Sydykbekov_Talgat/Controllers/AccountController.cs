using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class AccountController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<MyUser> _userManager;
    private readonly SignInManager<MyUser> _signInManager;

    public AccountController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager,
        IWebHostEnvironment env)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _env = env;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, IFormFile file)
    {
        if (ModelState.IsValid)
        {
            string? relativePath = null;
            string uploadedFileName = Path.Combine(_env.WebRootPath, "images/avatars");
            if (!Directory.Exists(uploadedFileName))
            {
                Directory.CreateDirectory(uploadedFileName);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string filesavePath = Path.Combine(uploadedFileName, fileName);
            using (var stream = new FileStream(filesavePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            relativePath = Path.Combine("images/avatars", fileName.Replace("\\", "/"));

            string roleName = model.RoleId == 2 ? "User" : "Employer";
            MyUser user = new MyUser
            {
                UserName = model.NicknameOrNameCompany,
                Email = model.Email,
                AvatarUrl = relativePath,
                PhoneNumber = model.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleName);
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Privacy", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            MyUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.Email);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не найден");
                return View(model);
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(
                user, model.Password,
                model.RememberMe,
                false
            );
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неправильный логин или пароль");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
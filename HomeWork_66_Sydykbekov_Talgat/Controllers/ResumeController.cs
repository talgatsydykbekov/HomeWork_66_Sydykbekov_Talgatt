using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "User")]
public class ResumeController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public ResumeController(MyUserContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateResumeViewModel();
        await LoadCategories(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateResumeViewModel model)
    {
        if (model.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == model.CategoryId.Value);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(model.CategoryId),"Выбранная категория не существует");
            }
        }
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(
                nameof(model.Title),
                "Введите должность.");
        }

        if (!ModelState.IsValid)
        {
            await LoadCategories(model);
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var now = DateTime.UtcNow;

        var resume = new Resume
        {
            Title = model.Title.Trim(),
            ExpectedSalary = model.ExpectedSalary!.Value,
            CategoryId = model.CategoryId!.Value,
            ApplicantId = user.Id,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Резюме создано.";

        return RedirectToAction("Index", "User");
    }
    
    private async Task LoadCategories(CreateResumeViewModel model)
    {
        model.Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.NameCategory)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            }).ToListAsync();
    }
    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return NotFound();

        return View(resume);
    }
}
using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "User")]
public class WorkExperienceController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public WorkExperienceController(
        MyUserContext context,
        UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int resumeId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var ownsResume = await _context.Resumes
            .AnyAsync(r =>
                r.Id == resumeId &&
                r.ApplicantId == user.Id);

        if (!ownsResume)
            return NotFound();

        var model = new CreateWorkExperienceViewModel
        {
            ResumeId = resumeId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateWorkExperienceViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r =>
                r.Id == model.ResumeId &&
                r.ApplicantId == user.Id);

        if (resume == null)
            return NotFound();

        var currentYear = DateTime.UtcNow.Year;

        if (model.StartYear.HasValue &&
            model.StartYear.Value > currentYear)
        {
            ModelState.AddModelError(
                nameof(model.StartYear),
                "Год начала не может быть в будущем.");
        }

        if (model.EndYear.HasValue &&
            model.EndYear.Value > currentYear)
        {
            ModelState.AddModelError(
                nameof(model.EndYear),
                "Год окончания не может быть в будущем.");
        }

        if (model.StartYear.HasValue &&
            model.EndYear.HasValue &&
            model.EndYear.Value < model.StartYear.Value)
        {
            ModelState.AddModelError(
                nameof(model.EndYear),
                "Год окончания не может быть раньше года начала.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var experience = new WorkExperience
        {
            ResumeId = resume.Id,
            CompanyName = model.CompanyName.Trim(),
            Position = model.Position.Trim(),
            Responsibilities = model.Responsibilities.Trim(),
            StartYear = model.StartYear!.Value,
            EndYear = model.EndYear
        };

        _context.WorkExperiences.Add(experience);

        resume.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(
            "Details",
            "Resume",
            new { id = resume.Id });
    }
}
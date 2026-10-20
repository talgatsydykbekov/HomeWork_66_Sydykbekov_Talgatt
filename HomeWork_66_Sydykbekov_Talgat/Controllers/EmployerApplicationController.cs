using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

[Authorize(Roles = "Employer")]
public class EmployerApplicationController : Controller
{
    private readonly MyUserContext _context;
    private readonly UserManager<MyUser> _userManager;

    public EmployerApplicationController(
        MyUserContext context,
        UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int vacancyId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .FirstOrDefaultAsync(v =>
                v.Id == vacancyId &&
                v.EmployerId == user.Id);

        if (vacancy == null)
            return NotFound();

        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.Resume)
                .ThenInclude(r => r.Applicant)
            .Where(a => a.VacancyId == vacancy.Id)
            .OrderByDescending(a => a.CratedAt)
            .ThenByDescending(a => a.Id)
            .ToListAsync();

        var model = new VacancyApplicationsViewModel
        {
            VacancyTitle = vacancy.Title,
            Applications = applications
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Resume(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var application = await _context.JobApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                a.Vacancy.EmployerId == user.Id);

        if (application == null)
            return NotFound();

        var resume = await _context.Resumes
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.WorkExperiences)
            .Include(r => r.Educations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r =>
                r.Id == application.ResumeId);

        if (resume == null)
            return NotFound();

        return View(
            "~/Views/ResumeCatalog/Details.cshtml",
            resume);
    }
}
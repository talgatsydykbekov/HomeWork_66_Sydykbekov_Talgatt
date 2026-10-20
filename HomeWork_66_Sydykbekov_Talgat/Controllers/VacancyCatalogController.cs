using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class VacancyCatalogController : Controller
{
    private readonly MyUserContext _context;

    public VacancyCatalogController(MyUserContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 20;

        var query = _context.Vacancies
            .AsNoTracking()
            .Where(v => v.IsPublished);

        var totalCount = await query.CountAsync();

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        page = Math.Clamp(page, 1, totalPages);

        var vacancies = await query
            .Include(v => v.Employer)
            .Include(v => v.Category)
            .OrderByDescending(v => v.UpdatedAt)
            .ThenByDescending(v => v.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new VacancyListViewModel
        {
            Vacancies = vacancies,
            Page = page,
            TotalPages = totalPages
        };

        return View(model);
    }
}
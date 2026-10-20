using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public async Task<IActionResult> Index(int? categoryId,string? search,string sort = "updated",int page = 1)
    {
        const int pageSize = 20;
        search = search?.Trim();
        if(sort != "salaryAsk" && sort != "SalaryDesc")
            sort = "updated";
        var query = _context.Vacancies
            .AsNoTracking()
            .Where(v => v.IsPublished);
        if (categoryId.HasValue)
        {
            query = query.Where(v =>
                v.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(v => v.Title.Contains(search));
        }
        var totalCount = await query.CountAsync();

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        page = Math.Clamp(page, 1, totalPages);

        var sortedQuary = sort switch
        {
            "salaryAsk" => query.OrderBy(v => v.Salary)
                .ThenByDescending(v => v.Id),

            "salaryDesc" => query.OrderByDescending(v => v.Salary)
                .ThenByDescending(v => v.Id),
            _ => query.OrderByDescending(v => v.UpdatedAt)
                .ThenByDescending(v => v.Id)
        };

        var vacancies = await query
            .Include(v => v.Employer)
            .Include(v => v.Category)
            .OrderByDescending(v => v.UpdatedAt)
            .ThenByDescending(v => v.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.NameCategory)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            }).ToListAsync();
        var model = new VacancyListViewModel
        {
            Vacancies = vacancies,
            Page = page,
            TotalPages = totalPages,
            Categories = categories,
            CategoryId = categoryId,
            Search = search
        };

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var vacancy = await _context.Vacancies
            .AsNoTracking()
            .Include(v => v.Category)
            .Include(v => v.Employer)
            .FirstOrDefaultAsync(v =>
                v.Id == id &&
                v.IsPublished);

        if (vacancy == null)
            return NotFound();

        return View(vacancy);
    }
}
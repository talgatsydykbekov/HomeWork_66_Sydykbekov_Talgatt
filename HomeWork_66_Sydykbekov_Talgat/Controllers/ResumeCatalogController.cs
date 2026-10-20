using HomeWork_66_Sydykbekov_Talgat.Models;
using HomeWork_66_Sydykbekov_Talgat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Controllers;

public class ResumeCatalogController : Controller
{
    private readonly MyUserContext _context;

    public ResumeCatalogController(MyUserContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? categoryId,
        int page = 1)
    {
        const int pageSize = 20;

        var query = _context.Resumes
            .AsNoTracking()
            .Where(r => r.IsPublished);

        if (categoryId.HasValue)
        {
            query = query.Where(r =>
                r.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync();

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        page = Math.Clamp(page, 1, totalPages);

        var resumes = await query
            .Include(r => r.Category)
            .OrderByDescending(r => r.UpdatedAt)
            .ThenByDescending(r => r.Id)
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
            })
            .ToListAsync();

        var model = new ResumeListViewModel
        {
            Resumes = resumes,
            CategoryId = categoryId,
            Categories = categories,
            Page = page,
            TotalPages = totalPages
        };

        return View(model);
    }
}
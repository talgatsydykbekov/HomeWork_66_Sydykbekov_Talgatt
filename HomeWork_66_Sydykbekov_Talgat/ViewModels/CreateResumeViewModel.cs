using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class CreateResumeViewModel
{
    [Required(ErrorMessage = "Введите должность")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите ожидаемую зарплату")]
    [Range(0, 1_000_000_000,
        ErrorMessage = "Укажите зарплату от 0 до 1 миллиарда")]
    public decimal? ExpectedSalary { get; set; }

    [Required(ErrorMessage = "Выберите категорию")]
    public int? CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
}
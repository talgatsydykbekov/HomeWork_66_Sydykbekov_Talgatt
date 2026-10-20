using HomeWork_66_Sydykbekov_Talgat.Models;

namespace HomeWork_66_Sydykbekov_Talgat.ViewModels;

public class VacancyListViewModel
{
    public List<Vacancy> Vacancies { get; set; } = new();

    public int Page { get; set; }

    public int TotalPages { get; set; }
}
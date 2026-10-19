using System.ComponentModel.DataAnnotations;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class Resume
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название должности")]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 1_000_000_000,
        ErrorMessage = "Укажите зарплату от 0 до 1 миллиарда")]
    public decimal ExpectedSalary { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int ApplicantId { get; set; }

    public MyUser Applicant { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
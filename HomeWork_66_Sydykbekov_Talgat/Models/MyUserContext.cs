using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class MyUserContext : IdentityDbContext<MyUser, IdentityRole<int>, int>
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; } = null!;
    public DbSet<Education> Educations { get; set; } = null!;
    public MyUserContext(DbContextOptions<MyUserContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Resume>()
            .Property(r => r.ExpectedSalary)
            .HasPrecision(18, 2);

        builder.Entity<Resume>()
            .HasOne(r => r.Applicant)
            .WithMany()
            .HasForeignKey(r => r.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Resume>()
            .HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<WorkExperience>()
            .HasOne(w => w.Resume)
            .WithMany(r => r.WorkExperiences)
            .HasForeignKey(w => w.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Education>()
            .HasOne(e => e.Resume)
            .WithMany(r => r.Educations)
            .HasForeignKey(e => e.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(builder);
    }
}
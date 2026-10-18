using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeWork_66_Sydykbekov_Talgat.Models;

public class MyUserContext : IdentityDbContext<MyUser, IdentityRole<int>, int>
{
    public DbSet<Category> Categories { get; set; }
    public MyUserContext(DbContextOptions<MyUserContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
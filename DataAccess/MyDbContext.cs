using CountryAPI.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CountryAPI.DataAccess
{
    /// <summary>
    /// Db Context
    /// </summary>
    public class MyDbContext : IdentityDbContext<IdentityUser, AppRole, string>
    {
        public MyDbContext()
        {

        }
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("Server=localhost;Database=JwtDemo;Port=5432;User Id=postgres;Password=fatih123;");
        }

    }
}

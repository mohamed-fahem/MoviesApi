using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MoviesApi.Models
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }


        public DbSet<Genre> Genres { get; set; }

        public DbSet<Movie> Movies { get; set; }
    }


}

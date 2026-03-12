using Microsoft.EntityFrameworkCore;
//using fut7Manager.Models;

namespace fut7Manager.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }

        //public DbSet<User> Users { get; set; }
    }
}
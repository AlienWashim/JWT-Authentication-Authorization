using JWTAuthenticationAuthorization.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthenticationAuthorization
{
    public class MyDBContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }

        // Constructor with DbContextOptions parameter
        public MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options)
        {
        }

        // Optional: Override OnConfiguring if needed
        // This can be removed if you're configuring the connection string in Program.cs
        // protected override void OnConfiguring(DbContextOptionsBuilder options)
        //     => options.UseSqlServer("YourConnectionStringHere");
    }
}

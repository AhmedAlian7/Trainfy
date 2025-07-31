using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Entities;

namespace mvcFirstApp.Models.Data
{
    public class AppDbContext : IdentityDbContext<AppUser> // DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public AppDbContext()
        {
        }
        public DbSet<Trainee> Trainees { get; set; } = null!;
        public DbSet<Instructor> Instructors { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<CourseResult> CourseResults { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionstring = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings:HostingConnection").Value;

            optionsBuilder.UseSqlServer(connectionstring);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvcFirstApp.Models.Entities;
namespace mvcFirstApp.Models.Data.config
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Credits)
                .IsRequired();

            builder.Property(c => c.Degree)
                .IsRequired();

            builder.Property(c => c.MinDegree)
                .IsRequired();

            // Foreign key relationship with Department
            builder.HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign key relationship with Instructor
            builder.HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship with CourseResults
            builder.HasMany(c => c.CourseResults)
                .WithOne(cr => cr.Course)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add constraints using ToTable
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Course_Credits", "Credits > 0");
                t.HasCheckConstraint("CK_Course_Degree", "Degree >= 0");
                t.HasCheckConstraint("CK_Course_MinDegree", "MinDegree >= 0 AND MinDegree <= Degree");
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvcFirstApp.Models.Entities;

namespace mvcFirstApp.Models.Data.config
{
    public class TraineeConfiguration : IEntityTypeConfiguration<Trainee>
    {
        public void Configure(EntityTypeBuilder<Trainee> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(55);

            builder.Property(t => t.DateOfBirth)
                .IsRequired();

            builder.Property(t => t.Address)
                .HasMaxLength(200);

            builder.Property(t => t.ImageUrl)
                .HasMaxLength(200);

            builder.HasOne(t => t.Department)
                .WithMany(d => d.Trainees)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.CourseResults)
                .WithOne(cr => cr.Trainee)
                .HasForeignKey(cr => cr.TraineeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

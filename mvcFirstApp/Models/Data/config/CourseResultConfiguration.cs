using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvcFirstApp.Models.Entities;

namespace mvcFirstApp.Models.Data.config
{
    public class CourseResultConfiguration : IEntityTypeConfiguration<CourseResult>
    {
        public void Configure(EntityTypeBuilder<CourseResult> builder)
        {
            builder.HasKey(cr => cr.Id);

            builder.Property(cr => cr.Degree)
                .IsRequired();

            // Foreign key relationship with Trainee
            builder.HasOne(cr => cr.Trainee)
                .WithMany(t => t.CourseResults)
                .HasForeignKey(cr => cr.TraineeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key relationship with Course
            builder.HasOne(cr => cr.Course)
                .WithMany(c => c.CourseResults)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add constraint for degree using ToTable
            builder.ToTable(t => t.HasCheckConstraint("CK_CourseResult_Degree", "Degree >= 0"));
        }
    }
}

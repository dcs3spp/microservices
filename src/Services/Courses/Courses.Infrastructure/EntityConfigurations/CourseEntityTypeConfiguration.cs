using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure;

namespace Courses.Infrastructure.EntityConfigurations
{
    /**
     * Use FluentAPI to map domain model properties to table columns and
     * map relationships
     */
    class CourseEntityTypeConfiguration
        : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> courseConfiguration)
        {
            courseConfiguration.ToTable("course", CourseContext.DEFAULT_SCHEMA);

            courseConfiguration.HasKey(c => c.Id);

            courseConfiguration.Ignore(c => c.DomainEvents);

            courseConfiguration.Property(c => c.Id)
                .HasColumnName("CourseID");

            courseConfiguration.Property(c => c.Description)
                .HasColumnName("CourseName")
                .HasMaxLength(200)
                .IsRequired();

            // courseConfiguration.HasMany(c => c.CourseUnits)
            //    .WithOne()
            //    .HasForeignKey("CourseId")
            //    .OnDelete(DeleteBehavior.Cascade);

            // var navigation = courseConfiguration.Metadata.FindNavigation(nameof(Course.CourseUnits));

            // navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
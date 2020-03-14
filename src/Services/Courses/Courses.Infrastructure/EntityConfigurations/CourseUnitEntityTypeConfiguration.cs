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
    class CourseUnitEntityTypeConfiguration
        : IEntityTypeConfiguration<CourseUnit>
    {
        public void Configure(EntityTypeBuilder<CourseUnit> configuration)
        {
            configuration.ToTable("courseunit", CourseContext.DEFAULT_SCHEMA);

            configuration.HasKey(cu => new { cu.CourseId, cu.UnitId });

            configuration.Property(cu => cu.CourseId)
                .HasColumnName("CourseID");

            configuration.Property(cu => cu.UnitId)
                .HasColumnName("UnitID");
        }
    }
}
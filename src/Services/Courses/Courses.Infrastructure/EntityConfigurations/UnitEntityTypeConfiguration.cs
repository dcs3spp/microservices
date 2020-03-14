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
    class UnitEntityTypeConfiguration
        : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> configuration)
        {
            configuration.ToTable("unit", CourseContext.DEFAULT_SCHEMA);

            configuration.HasKey(u => u.Id);

            configuration.Property(u => u.Id)
                .HasColumnName("UnitID")
                .ValueGeneratedOnAdd();

            configuration.Property(u => u.Code)
                .HasColumnName("UnitCode")
                .IsRequired();

            configuration.Property(u => u.Description)
                .HasColumnName("UnitName")
                .HasMaxLength(200)
                .IsRequired();

            configuration.Ignore(u => u.DomainEvents);

            configuration.HasIndex(i => i.Code);
        }
    }
}

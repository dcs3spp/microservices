using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using dcs3spp.courseManagementContainers.BuildingBlocks.IntegrationEventLogEF;

namespace Courses.API.Infrastructure.IntegrationEventMigrations
{
    public class IntegrationEventLogContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
    {
        public IntegrationEventLogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

            optionsBuilder.UseNpgsql(".", npgsqlOptionsAction: o => o.MigrationsAssembly(GetType().Assembly.GetName().Name));
            
            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}
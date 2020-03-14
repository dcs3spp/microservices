using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;
using Polly.Retry;
using System;
using System.Threading.Tasks;

using dcs3spp.courseManagementContainers.BuildingBlocks.IntegrationEventLogEF;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure;
using dcs3spp.Testing.Data.Seed;

namespace Courses.FunctionalTests
{
    /// <summary>
    /// Seed courses database with test data
    /// </summary>
    public class TestSeed : DataSeedBase
    {           
        private AsyncRetryPolicy _retryPolicy;
        private ILogger<TestSeed> _logger;
     

        /// <summary>
        /// Initialise <see cref="TestServer"/>, logger and default asynch retry policy with 3 retries
        /// </summary>
        public TestSeed (TestServer server) : base(server)
        {
            _server = server;
            _logger = server.Services.GetRequiredService<ILogger<TestSeed>>();
            _retryPolicy = CreatePolicy(nameof(TestSeed));
        }

        /// <summary>
        /// Initialise <see cref="TestServer"/>,logger and asynch retry policy
        /// </summary>
        public TestSeed(TestServer server, AsyncRetryPolicy retryPolicy) : this(server)
        {
            _retryPolicy = retryPolicy;
        }


        /// <summary>
        /// Seed the courses database with test data
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        /// <remarks>Migrates the courses database and integration event log</remarks>
        public override async Task<bool> UpAsync()
        {
            _logger.LogInformation("------ Seeding test data");
            
            try
            {
                await SeedCourseTestData();
                await SeedIntegrationEventLog();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateServer seeding the DB.");
                return false;
            }

            return true;
        }
      

        /// <summary>
        /// Deseed the database
        /// </summary>
        /// <returns>true if successful, otherwise false</returns>
        public override async Task<bool> DownAsync()
        {
            _logger.LogInformation("------ Deseeding test data");

            bool success = false;
            try 
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    using (var scope = _server.Host.Services.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var context = scopedServices.GetRequiredService<CourseContext>();

                        using (context)
                        {
                            // await context.Database.EnsureDeletedAsync();
                            context.Courses.RemoveRange(context.Courses);
                            context.Units.RemoveRange(context.Units);
                            await context.SaveChangesAsync();
                            
                            using (var command = context.Database.GetDbConnection().CreateCommand())
                            {
                                command.CommandText = @"ALTER TABLE CourseManagement.Unit ALTER COLUMN ""UnitID"" RESTART WITH 1";
                                context.Database.OpenConnection();
                                await command.ExecuteNonQueryAsync();
                            }

                            _logger.LogInformation($"------ {nameof(TestSeed)} deseeded test data");
                        }
                    }
                });
                success = true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"------ {nameof(TestSeed)} encountered exception while deseeding test data");
            }

            return success;
        }

        private async Task SeedCourseTestData()
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using (var scope = _server.Host.Services.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<CourseContext>();

                    using (context)
                    {
                        _logger.LogInformation("------DatabaseSeed.SeedCoursesDb migrating database");
                        context.Database.Migrate();

                        try 
                        {
                            CreateCourses(context);
                            await context.SaveChangesAsync();

                        }
                        catch(Exception e)
                        {
                            Console.WriteLine($"While seeding the courses table encountered an error {e.InnerException.Message}");
                            _logger.LogError(e, $"Error seeding test data");
                        }
                    }
                }
            });
        }

        private async Task SeedIntegrationEventLog()
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using (var scope = _server.Host.Services.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var integrationContext = scopedServices.GetRequiredService<IntegrationEventLogContext>();

                    using (integrationContext)
                    {
                        _logger.LogInformation("------DatabaseSeed.SeedCoursesDb applying IntegrationEventLog migrations");
                        await integrationContext.Database.MigrateAsync();
                    }
                }
            });
        }

        private void CreateCourses(CourseContext context) 
        {
            Course courseL3 = new Course(1014760, @"BTEC L3 ICT");
            courseL3.AddUnit(2, "Computer Systems");
            courseL3.AddUnit(42, "Advanced Spreadsheet Modelling");

            Course courseL3_90 = new Course(1013760, @"BTEC L3 90 ICT");

            Course courseL2 = new Course(1015423, @"BTEC L2 ICT");
            courseL2.AddUnit(11, "Spreadsheet Modelling");
           
            context.Courses.Add(courseL3);
            context.Courses.Add(courseL3_90);
            context.Courses.Add(courseL2);
        }

        private AsyncRetryPolicy CreatePolicy(string prefix, int retries = 3)
        {
            return Policy.Handle<NpgsqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        _logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
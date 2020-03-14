namespace dcs3spp.courseManagementContainers.Services.Courses.API.Infrastructure
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Npgsql;
    using Polly;
    using Polly.Retry;
    using System;
    using System.Threading.Tasks;

    using Courses.Infrastructure;
    
    public class CoursesContextSeed
    {
        public async Task SeedAsync(CourseContext context, IWebHostEnvironment env,IOptions<CourseSettings> settings, ILogger<CoursesContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(CoursesContextSeed));

            await policy.ExecuteAsync(async () =>
            {

                var useCustomizationData = settings.Value
                .UseCustomizationData;

                var contentRootPath = env.ContentRootPath;


                using (context)
                {
                    context.Database.Migrate();

                    // if (!context.CriteriaType.Any())
                    // {
                    //     context.CriteriaTypes.AddRange(useCustomizationData
                    //                             ? GetCriteriaTypesFromFile(contentRootPath, logger)
                    //                             : GetPredefinedCardTypes());

                    //     await context.SaveChangesAsync();
                    // }

                    await context.SaveChangesAsync();
                }
            });
        }
     
        private AsyncRetryPolicy CreatePolicy( ILogger<CoursesContextSeed> logger, string prefix, int retries =3)
        {
            return Policy.Handle<NpgsqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
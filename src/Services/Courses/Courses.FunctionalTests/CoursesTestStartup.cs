using Autofac;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using dcs3spp.courseManagementContainers.Services.Courses.API;

namespace Courses.FunctionalTests
{
    public class CoursesTestStartup : Startup
    {
        public CoursesTestStartup(IConfiguration env) : base(env)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // When SuppressCheckForUnhandledSecurityMetadata in config is set
            // avoids checking the Authorize annotation in test environment. 
            services.Configure<RouteOptions>(Configuration);
            base.ConfigureServices(services);
        }

        public override void ConfigureContainer(ContainerBuilder builder) 
        {
            base.ConfigureContainer(builder);
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}
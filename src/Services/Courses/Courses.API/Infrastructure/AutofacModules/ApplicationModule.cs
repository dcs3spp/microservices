using Autofac;
using System.Reflection;

using dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Abstractions;
using dcs3spp.courseManagementContainers.Services.Courses.API.Application.Commands;
using dcs3spp.courseManagementContainers.Services.Courses.API.Application.Queries;
using dcs3spp.courseManagementContainers.Services.Courses.Domain.AggregatesModel.CourseAggregate;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure.Repositories;

namespace dcs3spp.courseManagementContainers.Services.Courses.API.Infrastructure.AutofacModules
{
    public class ApplicationModule
        :Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c => new CourseQueries(QueriesConnectionString))
                .As<ICourseQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CourseRepository>()
                .As<ICourseRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(CreateCourseCommandHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

        }
    }
}

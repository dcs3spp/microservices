using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

using dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Abstractions;
using dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Events;
using dcs3spp.courseManagementContainers.BuildingBlocks.IntegrationEventLogEF;
using dcs3spp.courseManagementContainers.BuildingBlocks.IntegrationEventLogEF.Services;
using dcs3spp.courseManagementContainers.Services.Courses.API;
using dcs3spp.courseManagementContainers.Services.Courses.Infrastructure;

namespace Courses.API.Application.IntegrationEvents
{
    public class CoursesIntegrationEventService : ICoursesIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly CourseContext _courseContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<CoursesIntegrationEventService> _logger;

        public CoursesIntegrationEventService(IEventBus eventBus,
            CourseContext courseContext,
            IntegrationEventLogContext eventLogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<CoursesIntegrationEventService> logger)
        {
            _courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_courseContext.Database.GetDbConnection());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, Program.AppName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt, _courseContext.GetCurrentTransaction());
        }
    }
}
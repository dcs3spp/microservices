using dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Threading.Tasks;

namespace Courses.API.Application.IntegrationEvents
{
    public interface ICoursesIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}

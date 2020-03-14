using System.Threading.Tasks;

using dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Events;

namespace dcs3spp.courseManagementContainers.BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler 
        where TIntegrationEvent: IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}

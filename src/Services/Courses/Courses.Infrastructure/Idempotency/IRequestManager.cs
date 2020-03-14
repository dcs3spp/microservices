using System;
using System.Threading.Tasks;

namespace dcs3spp.courseManagementContainers.Services.Courses.Infrastructure.Idempotency
{
    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);

        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}
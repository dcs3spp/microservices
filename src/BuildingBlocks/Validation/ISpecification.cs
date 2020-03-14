namespace dcs3spp.courseManagementContainers.BuildingBlocks.Validation.Abstractions
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);
    }
}
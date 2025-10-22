
namespace AT2Soft.RAGEngine.Application.Interfaces;
public interface IUnitOfWorkRelational
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

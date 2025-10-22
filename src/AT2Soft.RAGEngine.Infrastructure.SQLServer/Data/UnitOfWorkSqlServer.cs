

using AT2Soft.RAGEngine.Application.Interfaces;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;

public class UnitOfWorkSqlServer : IUnitOfWorkRelational
{
    private readonly RAGSqlServerDbContext _context;

    public UnitOfWorkSqlServer(RAGSqlServerDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}

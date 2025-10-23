using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

public class KnowledgeRepository(RAGSqlServerDbContext context) : RepositoryBase<Knowledge, int>(context), IKnowledgeRepository
{
    
}

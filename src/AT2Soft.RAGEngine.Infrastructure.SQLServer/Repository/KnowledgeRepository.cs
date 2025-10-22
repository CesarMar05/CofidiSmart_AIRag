using AT2Soft.RAGEngine.Application.Interfaces.Repositories;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

public class KnowledgeRepository(RAGSqlServerDbContext context) : BaseRepository<Knowledge, int>(context), IKnowledgeRepository
{
    
}

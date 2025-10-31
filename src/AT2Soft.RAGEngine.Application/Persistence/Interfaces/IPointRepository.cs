using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Models;

namespace AT2Soft.RAGEngine.Application.Persistence.Interfaces;

public interface IPointRepository
{
    Task InsertAsync(List<Point> points, CancellationToken cancellationToken = default);

    Task<List<VectorialSearchResult>> SearchSimilarTextsAsync(Guid appId, string tenant, IReadOnlyList<string> divisions, float[] vector, int limit, CancellationToken cancellationToken = default);

}

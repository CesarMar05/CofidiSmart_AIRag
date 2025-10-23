using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Models;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Data;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.APIClient;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;
using Refit;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Repository;

public class PointRepository : IPointRepository
{
    private readonly RAGVectorDbContext _context;
    private readonly IQdrantApiClient _qdrantApiClient;

    public PointRepository(IQdrantApiClient qdrantApiClient, RAGVectorDbContext context)
    {
        _qdrantApiClient = qdrantApiClient;
        _context = context;
    }

    public async Task InsertAsync(List<Point> points, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new QdrantPointsRequest
            {
                Points = points
                    .Select(p => new QdrantPoint<Payload> { Id = p.Id, Vector = p.Vector, Payload = p.Payload })
                    .ToList()
            };

            var result = await _qdrantApiClient.PointInsert(_context.Collection, request);
        }
        catch (ApiException ex)
        {
            var result = APIClientHandlerExceptions.ApiExceptionHandler<QdrantPointInsertResult?>(ex);
        }
        catch (Exception ex)
        {
            var result = APIClientHandlerExceptions.ExceptionHandler<QdrantPointInsertResult?>(ex);
        }
    }

    public async Task<List<VectorialSearchResult>> SearchSimilarTextsAsync(Guid appId, string TenantId, float[] vector, int limit, CancellationToken cancellationToken = default)
    {
        QdrantResponse<List<QdrantSearchResult<Payload>>?>? result = new();
        try
        {
            var filter = new Filter
            {
                Must =
                [
                    new FieldCondition("application", new MatchValue(appId.ToString())),
                    new FieldCondition("tenant", new MatchValue(TenantId))
                ]  
            };

            var request = new QdrantSearchRequest { Limit = limit, WithPayload = true, Vector = vector, WithVector = false, Filter = filter };
            result = await _qdrantApiClient.PointSearch(_context.Collection, request);

            if (result == null || result.Result == null)
                return [];

            return result.Result
                .Select(r => new VectorialSearchResult(r.Id, r.Version, r.Score, r.Payload))
                .ToList();
        }
        catch (ApiException ex)
        {
            result = APIClientHandlerExceptions.ApiExceptionHandler<List<QdrantSearchResult<Payload>>?>(ex);
            return [];
        }
        catch (Exception ex)
        {
            result = APIClientHandlerExceptions.ExceptionHandler<List<QdrantSearchResult<Payload>>?>(ex);
            return [];
        } 
    }
}

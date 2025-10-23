using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Enums;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.APIClient;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Enums;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;
using Refit;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Repository;

public class CollectionRepository : ICollectionRepository
{
    private readonly IQdrantApiClient _qdrantApiClient;

    public CollectionRepository(IQdrantApiClient qdrantApiClient)
    {
        _qdrantApiClient = qdrantApiClient;
    }


    
    public async Task<List<string>> GetListAsync(CancellationToken cancellationToken = default)
    {
        QdrantResponse<QdrantCollectionGetListResponse?> result = new();
        try
        {
            result = await _qdrantApiClient.CollectionGetList();
        }
        catch (ApiException ex)
        {
            result = APIClientHandlerExceptions.ApiExceptionHandler<QdrantCollectionGetListResponse?>(ex);
        }
        catch (Exception ex)
        {
            result = APIClientHandlerExceptions.ExceptionHandler<QdrantCollectionGetListResponse?>(ex);
        }

        if (result.Result == null)
            return [];

        return result.Result.Collections
            .Select(c => c.Name )
            .ToList();
    }

    public async Task<bool> CreateAsync(string collectionName, int vectorSize, DistanceType vectorDistance, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new QdrantCollectionCreateRequest
            {
                Vectors = new()
                {
                    Size = vectorSize,
                    Distance = vectorDistance switch
                    {
                        DistanceType.Euclidean => QdrantDistanceType.Euclidean,
                        DistanceType.Cosine => QdrantDistanceType.Cosine,
                        DistanceType.Dot => QdrantDistanceType.Dot,
                        _ => QdrantDistanceType.Cosine
                    }
                }
            };

            var result = await _qdrantApiClient.CollectionCreate(collectionName, request);
            if (result.Result != null)
                return (bool)result.Result;

            return false;
        }
        catch (ApiException ex)
        {
            var result = APIClientHandlerExceptions.ApiExceptionHandler<bool?>(ex);
            return false;
        }
        catch (Exception ex)
        {
            var result = APIClientHandlerExceptions.ExceptionHandler<bool?>(ex);
            return false;
        }
    }
}

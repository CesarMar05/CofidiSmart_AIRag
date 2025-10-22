using System;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;
using Refit;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.APIClient;

public interface IQdrantApiClient
{
    //
    // Collections
    //

    [Get("/collections")]
    Task<QdrantResponse<QdrantCollectionGetListResponse?>> CollectionGetList();

    [Put("/collections/{collection}")]
    Task<QdrantResponse<bool?>> CollectionCreate(string collection, [Body] QdrantCollectionCreateRequest request);


    //
    // Points
    //

    [Put("/collections/{collection}/points?wait=true")]
    Task<QdrantResponse<QdrantPointInsertResult?>> PointInsert(string collection, [Body] QdrantPointsRequest request);

    [Post("/collections/{collection}/points/search")]
    Task<QdrantResponse<List<QdrantSearchResult<Payload>>?>> PointSearch(string collection, [Body] QdrantSearchRequest request);
}

namespace AT2Soft.RAGEngine.Domain.Interfaces.Services;

public interface IEmbeddingServicessss
{
    Task StoreAsync(string collection, List<(string chunk, float[] vector)> data);
    Task<List<float[]>> GetListAsync(List<string> chunks);
    Task<float[]> GetAsync(string text);
}


namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Data;

public class RAGVectorDbContext
{
    public string URLBase { get; private set; }
    public string Collection { get; private set; }

    public RAGVectorDbContext(string urlBase, string collecion)
    {
        URLBase = urlBase;
        Collection = collecion;
    }
}

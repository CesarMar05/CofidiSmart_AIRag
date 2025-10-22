using System;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Enums;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantVector
{
    public int Size { get; set; }
    public QdrantDistanceType Distance { get; set; }
}

using System;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantInsertResponse
{
    public string Status { get; set; } = string.Empty;
    public float Time { get; set; }
    public int Status_Code { get; set; }
    public string Message { get; set; }  = string.Empty;
    public InsertResult? Result { get; set; }

    public class InsertResult
    {
        public long OperationId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

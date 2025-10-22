using System;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class Point
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public float[] Vector { get; set; } = [];
    public Payload Payload { get; set; } = new();
}

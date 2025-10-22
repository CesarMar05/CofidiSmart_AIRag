using AT2Soft.RAGEngine.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Converts;

public sealed class CompressedStringConverter : ValueConverter<string, byte[]>
{
    public CompressedStringConverter()
        : base(
            v => TextCompression.CompressToBytes(v),       // string -> byte[] (comprimido)
            v => TextCompression.DecompressToString(v))    // byte[] -> string (descomprimido)
    { }
}

// Variante si prefieres GZip:
public sealed class GZipCompressedStringConverter : ValueConverter<string, byte[]>
{
    public GZipCompressedStringConverter()
        : base(
            v => TextCompression.CompressToBytesGZip(v),
            v => TextCompression.DecompressToStringGZip(v))
    { }
}
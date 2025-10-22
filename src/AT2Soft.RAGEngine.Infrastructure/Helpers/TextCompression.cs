using System;
using System.IO.Compression;
using System.Text;

namespace AT2Soft.RAGEngine.Infrastructure.Helpers;

public static class TextCompression
{
    public static byte[] CompressToBytes(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return [];

        var inputBytes = Encoding.UTF8.GetBytes(text);

        using var output = new MemoryStream();
        // Brotli: buen ratio y soporte moderno (.NET Core/.NET)
        using (var brotli = new BrotliStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            brotli.Write(inputBytes, 0, inputBytes.Length);
        }
        return output.ToArray();
    }

    public static string DecompressToString(byte[]? data)
    {
        if (data is null || data.Length == 0)
            return string.Empty;

        using var input = new MemoryStream(data);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }

    // ——— Variante GZip (si prefieres) ———
    public static byte[] CompressToBytesGZip(string? text)
    {
        if (string.IsNullOrEmpty(text)) return Array.Empty<byte>();
        var inputBytes = Encoding.UTF8.GetBytes(text);
        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(inputBytes, 0, inputBytes.Length);
        }
        return ms.ToArray();
    }

    public static string DecompressToStringGZip(byte[]? data)
    {
        if (data is null || data.Length == 0) return string.Empty;
        using var ms = new MemoryStream(data);
        using var gzip = new GZipStream(ms, CompressionMode.Decompress);
        using var outMs = new MemoryStream();
        gzip.CopyTo(outMs);
        return Encoding.UTF8.GetString(outMs.ToArray());
    }
}

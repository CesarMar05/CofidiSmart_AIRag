using System.Text.RegularExpressions;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;


namespace AT2Soft.RAGEngine.Infrastructure.Embedding;

public class TextChunkerService : ITextChunkerService
{
    // Estimación simple: ~1.3 tokens por palabra (suele funcionar bien con ES/EN)
    private static int EstimateTokens(string text)
        => (int)Math.Ceiling(SplitWords(text).Count * 1.3);

    private static List<string> SplitWords(string text)
        => Regex.Matches(text, @"\p{L}[\p{L}\p{Mn}\p{Pd}’']*|\d+").Select(m => m.Value).ToList();

    // Divide en frases preservando signos y saltos de línea duros como límites
    private static List<string> SplitSentences(string text)
    {
        // Cortes en ., !, ?, ;, : seguidos de espacio/salto o fin; también doble salto de línea
        var parts = Regex.Split(text, @"(?<=[\.!\?;:])\s+|\r?\n\r?\n")
                         .Select(s => s.Trim())
                         .Where(s => s.Length > 0)
                         .ToList();
        return parts.Count > 0 ? parts : [text];
    }

    public  List<ChunkResult> Chunk(string text, TextChunkerOptions opt)
    {
        var sentences = SplitSentences(text);
        var chunks = new List<ChunkResult>();
        var buffer = new List<string>();
        int bufferTokens = 0;
        int idx = 0;

        foreach (var sentence in sentences)
        {
            var sentTokens = EstimateTokens(sentence);

            // Si una sola frase ya excede MaxTokens, la troceamos por palabras
            if (sentTokens > opt.MaxTokens)
            {
                // vaciamos lo que tengamos antes
                FlushBufferIfNeeded(chunks, ref buffer, ref bufferTokens, ref idx, opt);

                var words = SplitWords(sentence);
                var current = new List<string>();
                int currentTok = 0;

                foreach (var w in words)
                {
                    var wTok = (int)Math.Ceiling(1.3); // ~1 palabra = 1.3 tokens
                    if (currentTok + wTok > opt.MaxTokens && current.Count > 0)
                    {
                        chunks.Add(MakeChunk(current, idx++, currentTok));
                        // overlap a nivel palabra
                        var overlapCount = Math.Max(0, (int)Math.Round(opt.OverlapTokens / 1.3));
                        current = current.Skip(Math.Max(0, current.Count - overlapCount)).ToList();
                        currentTok = EstimateTokens(string.Join(" ", current));
                    }
                    current.Add(w);
                    currentTok += wTok;
                }
                if (current.Count > 0)
                    chunks.Add(MakeChunk(current, idx++, currentTok));

                continue;
            }

            // Intentamos agregar la frase al buffer
            if (bufferTokens + sentTokens <= opt.TargetTokens)
            {
                buffer.Add(sentence);
                bufferTokens += sentTokens;
            }
            else
            {
                // Si ya tenemos suficiente, cortamos y aplicamos overlap
                FlushBufferIfNeeded(chunks, ref buffer, ref bufferTokens, ref idx, opt, force: true);

                // Iniciar siguiente con la frase actual (si cabe sola, que suele ser el caso)
                buffer.Add(sentence);
                bufferTokens = sentTokens;
            }
        }

        // último flush
        FlushBufferIfNeeded(chunks, ref buffer, ref bufferTokens, ref idx, opt, force: true);

        return chunks;
    }

    private static void FlushBufferIfNeeded(List<ChunkResult> chunks,
        ref List<string> buffer, ref int bufferTokens, ref int idx, TextChunkerOptions opt, bool force = false)
    {
        if (buffer.Count == 0) return;

        if (force || bufferTokens >= opt.MinTokens || bufferTokens > opt.MaxTokens)
        {
            chunks.Add(new ChunkResult(idx++, string.Join(" ", buffer), bufferTokens));

            // construir overlap para el próximo buffer
            var words = SplitWords(string.Join(" ", buffer));
            var overlapWordCount = Math.Max(0, (int)Math.Round(opt.OverlapTokens / 1.3)); // tokens -> palabras
            var carry = words.TakeLast(overlapWordCount).ToList();

            buffer = carry.Count > 0 ? new List<string> { string.Join(" ", carry) } : new List<string>();
            bufferTokens = buffer.Count > 0 ? EstimateTokens(buffer[0]) : 0;
        }
    }

    private static ChunkResult MakeChunk(List<string> words, int idx, int estimatedTokens)
        => new(idx, string.Join(" ", words), estimatedTokens);
}

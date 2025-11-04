using System.Text.RegularExpressions;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;


namespace AT2Soft.RAGEngine.Infrastructure.Embedding;

public class TextChunkerServiceOld : ITextChunkerService
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

    public List<ChunkResult> Chunk(string text, TextChunkerOptions opt)
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


public class TextChunkerService : ITextChunkerService
{
    public List<ChunkResult> Chunk(string text, TextChunkerOptions opt)
    {
        text = NormalizeText(text);

        var sentences = SplitSentences(text);
        var chunks = new List<ChunkResult>();
        var buffer = new List<string>();
        int bufferTokens = 0;
        int bufferChars = 0;
        int idx = 0;

        foreach (var sentence in sentences)
        {
            if (string.IsNullOrWhiteSpace(sentence)) continue;

            var sentTokens = EstimateTokens(sentence);
            var sentChars = sentence.Length;

            // Si una sola frase excede MaxTokens -> troceo por palabras
            if (sentTokens > opt.MaxTokens || sentChars > opt.MaxChars)
            {
                FlushBufferIfNeeded(chunks, ref buffer, ref bufferTokens, ref bufferChars, ref idx, opt);

                foreach (var hard in HardSplitByWords(sentence, opt))
                    chunks.Add(new ChunkResult(idx++, hard.text, hard.tokens));

                continue;
            }

            // Intentar agregar al buffer
            if (bufferTokens + sentTokens <= opt.TargetTokens &&
                bufferChars + sentChars <= opt.MaxChars)
            {
                buffer.Add(sentence);
                bufferTokens += sentTokens;
                bufferChars += sentChars;
            }
            else
            {
                // cerrar chunk actual con overlap
                FlushBufferIfNeeded(chunks, ref buffer, ref bufferTokens, ref bufferChars, ref idx, opt, force:true);

                // iniciar buffer con la frase actual
                buffer.Add(sentence);
                bufferTokens = sentTokens;
                bufferChars = sentChars;
            }
        }

        FlushBufferIfNeeded(chunks, ref buffer, ref bufferTokens, ref bufferChars, ref idx, opt, force:true);
        return chunks;
    }

    private static string NormalizeText(string text)
    {
        // Normaliza “ . ”, múltiples espacios y puntos suspensivos
        var s = text.Replace("\r\n", "\n");
        s = Regex.Replace(s, @"\s*\.\s*", ". ");     // colapsa " . " -> ". "
        s = Regex.Replace(s, @"\.{3,}", "… ");       // “...” -> “…” + espacio
        s = Regex.Replace(s, @"[ \t]{2,}", " ");     // espacios múltiples
        s = Regex.Replace(s, @"\n{3,}", "\n\n");     // demasiados saltos de línea
        return s.Trim();
    }

    private static IReadOnlyList<string> SplitSentences(string text)
    {
        // Divide por fin de oración: . ! ? … seguidos de espacio o fin de línea
        var parts = Regex.Split(text, @"(?<=[\.!\?…])\s+");
        // Limpia elementos vacíos o mínimos
        return parts.Select(p => p.Trim())
                    .Where(p => p.Length > 0)
                    .ToList();
    }

    // Heurística: ~1 token ≈ 3.8 chars (mezcla en/es)
    private static int EstimateTokens(string s)
        => (int)Math.Ceiling(s.Length / 3.8);

    private static IEnumerable<(string text, int tokens)> HardSplitByWords(string sentence, TextChunkerOptions opt)
    {
        var words = Regex.Split(sentence.Trim(), @"\s+")
                        .Where(w => w.Length > 0)
                        .ToList();

        var curr = new List<string>();
        int currTokens = 0;

        foreach (var w in words)
        {
            int wTok = EstimateTokens(w.Length <= 20 ? w : w[..20]); // bound simple
            if (currTokens + wTok > opt.MaxTokens && curr.Count > 0)
            {
                var text = string.Join(" ", curr);
                yield return (text, EstimateTokens(text));

                // overlap por tokens
                var keepTokens = Math.Max(0, opt.OverlapTokens);
                curr = TakeTailByTokens(curr, keepTokens).ToList();
                currTokens = EstimateTokens(string.Join(" ", curr));
            }
            curr.Add(w);
            currTokens += wTok;
        }

        if (curr.Count > 0)
        {
            var text = string.Join(" ", curr);
            yield return (text, EstimateTokens(text));
        }
    }

    private static IEnumerable<string> TakeTailByTokens(List<string> words, int tokenBudget)
    {
        // toma desde el final hasta cubrir tokenBudget aprox.
        int sum = 0;
        for (int i = words.Count - 1; i >= 0; i--)
        {
            sum += EstimateTokens(words[i]);
            if (sum >= tokenBudget) return words.Skip(i);
        }
        return words; // si no alcanza, devuelve todo
    }

    private static void FlushBufferIfNeeded(
        List<ChunkResult> chunks,
        ref List<string> buffer,
        ref int bufferTokens,
        ref int bufferChars,
        ref int idx,
        TextChunkerOptions opt,
        bool force = false)
    {
        if ((force && buffer.Count > 0) ||
            bufferTokens >= opt.TargetTokens ||
            bufferChars >= opt.MaxChars)
        {
            var text = string.Join(" ", buffer).Trim();
            if (text.Length > 0)
            {
                var tokens = EstimateTokens(text);
                chunks.Add(new ChunkResult(idx++, text, tokens));

                // overlap a nivel de oraciones (con presupuesto en tokens)
                var keep = TakeTailByTokens(buffer, opt.OverlapTokens).ToList();
                buffer = keep;
                var keepText = string.Join(" ", buffer);
                bufferTokens = EstimateTokens(keepText);
                bufferChars = keepText.Length;
            }
            else
            {
                buffer.Clear();
                bufferTokens = 0;
                bufferChars = 0;
            }
        }
    }
}

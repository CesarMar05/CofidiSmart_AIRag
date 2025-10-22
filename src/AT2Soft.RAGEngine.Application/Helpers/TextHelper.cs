
namespace AT2Soft.RAGEngine.Application.Helpers;

public class TextHelper 
{
    public static List<string> TextCharacterSpliter(string text, int size, int overlap)
    {
        var chunks = new List<string>();
        for (int i = 0; i < text.Length; i += size - overlap)
        {
            var end = Math.Min(i + size, text.Length);
            chunks.Add(text.Substring(i, end - i));
            if (end == text.Length) break;
        }
        return chunks;
    }
}

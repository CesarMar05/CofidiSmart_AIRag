using System;

namespace AT2Soft.RAGEngine.Application.Features.Prompt.Functions;

public class PromptFunctions
{
    public static string BuildPrompt(string query, List<string> contextChunks)
    {
        var context = string.Join("\n", contextChunks);

        return $"""
        Usa el contexto proporcionado para responder la siguiente pregunta de forma clara y completa.
            - No repitas textualmente el contenido del contexto.
            - Resume o razona si es necesario.
            - Si no encuentras suficiente información, responde solamente "No tengo la información suficiente para responder a la pregunta"
            - Si la pregunta incluye que puedas usar tu conocimiento, responde usando tu base de conocimiento general en caso de no tener suficiente información para responder.

        Contexto:
        {context}

        Pregunta:
        {query}

        Respuesta:
            - Deberas responder en el idioma en el que se relizó la pregunta.
        """;
    }
}

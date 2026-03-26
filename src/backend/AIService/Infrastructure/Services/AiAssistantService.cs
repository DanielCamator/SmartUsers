using ALService.Application.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Pinecone;
using System.Diagnostics;
using System.Text;

namespace ALService.Infrastructure.Services;

public class AiAssistantService : IAiAssistantService
{
    private readonly Kernel _kernel;
    private readonly ITextEmbeddingGenerationService _embeddingService;
    private readonly PineconeClient _pineconeClient;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    public AiAssistantService(
        Kernel kernel,
        ITextEmbeddingGenerationService embeddingService,
        PineconeClient pineconeClient,
        IConfiguration config,
        IMemoryCache cache)
    {
        _kernel = kernel;
        _embeddingService = embeddingService;
        _pineconeClient = pineconeClient;
        _config = config;
        _cache = cache;
    }

    public async Task<AiResponseDto> AskQuestionAsync(string userQuestion, string sessionId, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!_cache.TryGetValue(sessionId, out ChatHistory chatHistory))
        {
            chatHistory = new ChatHistory();
        }

        var indexName = _config["Pinecone:IndexName"] ?? "smartusers-index";
        var index = await _pineconeClient.GetIndex(indexName, ct);
        var questionEmbedding = await _embeddingService.GenerateEmbeddingAsync(userQuestion, cancellationToken: ct);
        var searchResults = await index.Query(questionEmbedding.ToArray(), 3u, includeMetadata: true, ct: ct);

        var contextBuilder = new StringBuilder();
        foreach (var match in searchResults)
        {
            if (match.Metadata != null && match.Metadata.TryGetValue("text", out var textValue))
                contextBuilder.AppendLine($"- {textValue}");
        }

        var requestHistory = new ChatHistory();

        var systemPrompt = $@"Eres el asistente técnico avanzado del sistema de gestión de usuarios 'SmartUsers'. 
            Tu objetivo es ayudar a los administradores a entender el funcionamiento del sistema, políticas y auditorías basándote ESTRICTAMENTE en el contexto proporcionado.

            <contexto_sistema>
            {contextBuilder}
            </contexto_sistema>

            REGLAS DE OPERACIÓN:
            1. Responde de forma profesional, técnica y concisa.
            2. Usa la información en <contexto_sistema> para responder. Si la respuesta está ahí, sé directo.
            3. Si la pregunta NO puede responderse con el contexto proporcionado (ej. preguntas personales, clima, definiciones genéricas de IT), responde cortésmente que tu conocimiento se limita a la administración de SmartUsers y no inventes información.
            4. No menciones a Daniel Camacho, céntrate en el sistema.";

        requestHistory.AddSystemMessage(systemPrompt);

        requestHistory.AddUserMessage("¿Cuál es la política de contraseñas?");
        requestHistory.AddAssistantMessage("Según el contexto del sistema, las contraseñas deben tener al menos 12 caracteres, incluir una mayúscula, una minúscula, un número y un carácter especial, y expiran cada 90 días.");

        requestHistory.AddUserMessage("¿Cómo arreglo mi lavadora?");
        requestHistory.AddAssistantMessage("Lo siento, soy un asistente especializado exclusivamente en el sistema SmartUsers. No poseo información sobre reparación de electrodomésticos. ¿Te puedo ayudar con alguna consulta sobre roles o usuarios?");
        
        requestHistory.AddRange(chatHistory);
        requestHistory.AddUserMessage(userQuestion);
        chatHistory.AddUserMessage(userQuestion);

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.0
        };

        var chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

        var response = await chatCompletion.GetChatMessageContentAsync(
            chatHistory: requestHistory,
            executionSettings: executionSettings,
            cancellationToken: ct);

        stopwatch.Stop();

        var responseContent = response.Content ?? string.Empty;

        chatHistory.AddAssistantMessage(responseContent);
        _cache.Set(sessionId, chatHistory, TimeSpan.FromMinutes(30));

        int promptTokens = 0;
        int completionTokens = 0;
        int totalTokens = 0;

        if (response.Metadata != null && response.Metadata.TryGetValue("Usage", out var usageObj) && usageObj != null)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(usageObj);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Extraemos Input/Prompt
                if (root.TryGetProperty("PromptTokens", out var promptProp) || root.TryGetProperty("InputTokenCount", out promptProp))
                {
                    promptTokens = promptProp.GetInt32();
                }

                // Extraemos Output/Completion
                if (root.TryGetProperty("CompletionTokens", out var compProp) || root.TryGetProperty("OutputTokenCount", out compProp))
                {
                    completionTokens = compProp.GetInt32();
                }

                // Extraemos Total
                if (root.TryGetProperty("TotalTokens", out var totalProp) || root.TryGetProperty("TotalTokenCount", out totalProp))
                {
                    totalTokens = totalProp.GetInt32();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MÉTRICAS] Error extrayendo tokens: {ex.Message}");
            }
        }

        // Cálculo de Costos para Groq (Llama-3.1-8b-instant)
        // Costo aproximado: Input: $0.05 USD / 1M | Output: $0.08 USD / 1M
        decimal costoInput = (promptTokens / 1_000_000m) * 0.05m;
        decimal costoOutput = (completionTokens / 1_000_000m) * 0.08m;
        decimal costoTotal = costoInput + costoOutput;

        return new AiResponseDto
        {
            Answer = responseContent,
            LatencyMs = stopwatch.ElapsedMilliseconds,
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            TotalTokens = totalTokens,
            EstimatedCostUsd = costoTotal // <-- Asegúrate de tener esta propiedad en tu clase AiResponseDto
        };
    }
}
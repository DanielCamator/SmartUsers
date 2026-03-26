using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Embeddings;
using Pinecone;
using ALService.Application.Interfaces;

namespace ALService.Infrastructure.Services;

public class IngestionService : IIngestionService
{
    private readonly ITextEmbeddingGenerationService _embeddingService;
    private readonly PineconeClient _pineconeClient;
    private readonly IConfiguration _config;

    public IngestionService(
        ITextEmbeddingGenerationService embeddingService,
        PineconeClient pineconeClient,
        IConfiguration config)
    {
        _embeddingService = embeddingService;
        _pineconeClient = pineconeClient;
        _config = config;
    }

    public async Task CreateIndexAndUploadAsync(CancellationToken ct = default)
    {
        var indexName = _config["Pinecone:IndexName"] ?? "smartusers-index";

        var path = Path.Combine(Directory.GetCurrentDirectory(), "reglas_smartusers.json");
        var content = await File.ReadAllTextAsync(path, ct);

        var data = JsonSerializer.Deserialize<SmartUsersData>(content);

        if (data?.RulesChunks == null) return;

        var index = await _pineconeClient.GetIndex(indexName, ct);

        var vectors = new List<Vector>();

        for (int i = 0; i < data.RulesChunks.Count; i++)
        {
            var text = data.RulesChunks[i];
            var embedding = await _embeddingService.GenerateEmbeddingAsync(text, cancellationToken: ct);

            vectors.Add(new Vector
            {
                Id = $"chunk-{i}",
                Values = embedding.ToArray(),
                Metadata = new MetadataMap { { "text", text } }
            });
        }

        await index.Upsert(vectors, ct: ct);
    }
    private class SmartUsersData
    {
        public List<string>? RulesChunks { get; set; }
    }
}
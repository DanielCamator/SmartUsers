using Microsoft.AspNetCore.RateLimiting;
using Microsoft.SemanticKernel;
using Pinecone;
using ALService.Application.Interfaces;
using ALService.Infrastructure.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaSamrtUsers", policy =>
    {
        policy.WithOrigins(
              "http://localhost:3000"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ChatLimit", opt => {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("EmailLimit", opt => {
        opt.Window = TimeSpan.FromHours(1);
        opt.PermitLimit = 3;
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Has superado el límite de peticiones. Intenta más tarde.", token);
    };
});

var groqHttpClient = new HttpClient
{
    BaseAddress = new Uri("https://api.groq.com/openai/v1/")
};

#pragma warning disable CS0618
builder.Services.AddKernel()
    .AddOpenAIChatCompletion(
        modelId: "llama-3.1-8b-instant",
        apiKey: builder.Configuration["Groq:ApiKey"]!,
        httpClient: groqHttpClient)

    .AddOpenAITextEmbeddingGeneration(
        modelId: "text-embedding-3-small",
        apiKey: builder.Configuration["OpenAI:ApiKey"]!);
#pragma warning restore CS0618

string pineconeApiKey = builder.Configuration["Pinecone:ApiKey"] 
    ?? throw new InvalidOperationException("Falta la API Key de Pinecone.");

builder.Services.AddSingleton(new PineconeClient(pineconeApiKey));
builder.Services.AddScoped<IAiAssistantService, AiAssistantService>();
builder.Services.AddScoped<IIngestionService, IngestionService>();

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

var app = builder.Build();

app.UseStaticFiles();

app.UseCors("PoliticaSamrtUsers");
app.UseRateLimiter();

app.MapControllers();

app.Run();
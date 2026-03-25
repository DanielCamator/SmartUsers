using Audit.Application.Consumers;
using Audit.Application.Interfaces;
using Audit.Infrastructure.Persistence;
using Audit.Infrastructure.Repositories;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;
using Shared.Contracts.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var mongoSection = builder.Configuration.GetSection("MongoDb");
var connectionString = mongoSection["ConnectionString"];
var databaseName = mongoSection["DatabaseName"] ?? "AuditDb";

Log.Information("🔌 MongoDB Connection: {ConnectionString}", connectionString);
Log.Information("📦 MongoDB Database: {DatabaseName}", databaseName);

// Registrar MongoDB como singleton ANTES de MassTransit
var mongoClient = new MongoClient(connectionString);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(databaseName);

// Verificar conexión a MongoDB en startup
try
{
    var adminDatabase = mongoClient.GetDatabase("admin");
    var pingCommand = new MongoDB.Bson.BsonDocument("ping", 1);
    adminDatabase.RunCommand<MongoDB.Bson.BsonDocument>(pingCommand);
    Log.Information("✅ MongoDB conectado exitosamente");
}
catch (Exception ex)
{
    Log.Error(ex, "❌ Error al conectar con MongoDB");
}

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
        });

        cfg.Message<UserCreatedEvent>(x =>
        {
            x.SetEntityName("user-created");
        });

        cfg.ReceiveEndpoint("audit-user-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<UserCreatedConsumer>(context);

            // Suscribir al evento
            e.Bind<UserCreatedEvent>();
        });
    });
});

Log.Information("🚀 MassTransit configurado con RabbitMQ");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
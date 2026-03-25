using Microsoft.EntityFrameworkCore;
using MassTransit;
using Serilog;
using Shared.Contracts.Events;
using Auth.Infrastructure.Persistence;
using Auth.Application.Interfaces;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Security;
using Auth.Application.Consumers;
using Auth.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDbConnection")));

builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Auth.Application.Commands.Login.LoginHandler).Assembly));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SyncUserConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "rabbitmq", "/", h => {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
    });

        cfg.Message<UserCreatedEvent>(x =>
        {
            x.SetEntityName("user-created");
});

        cfg.ReceiveEndpoint("auth-registration-sync-queue", e =>
        {
            e.ConfigureConsumer<SyncUserConsumer>(context);
            e.Bind<UserCreatedEvent>();
        });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",      // Docker
                "http://localhost:5173",      // Vite dev server
                "http://127.0.0.1:3000",
                "http://127.0.0.1:5173"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("AllowFrontend");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    int retries = 10;
    while (retries > 0)
    {
        try { context.Database.Migrate(); break; }
        catch { retries--; Thread.Sleep(5000); }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
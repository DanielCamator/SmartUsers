using Microsoft.EntityFrameworkCore;
using MassTransit;
using Serilog;
using Auth.Infrastructure.Persistence;
using Auth.Application.Interfaces;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Security;
using Auth.Application.Consumers;

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
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("auth-registration-sync-queue", e =>
        {
            e.ConfigureConsumer<SyncUserConsumer>(context);
        });
    });
});

var app = builder.Build();

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
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "API funcionando correctamente.");
app.Run();
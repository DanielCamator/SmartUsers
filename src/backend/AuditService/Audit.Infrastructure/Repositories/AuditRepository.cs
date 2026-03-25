using Audit.Application.Interfaces;
using Audit.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Audit.Infrastructure.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly IMongoCollection<AuditLog> _auditLogs;

    public AuditRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _auditLogs = database.GetCollection<AuditLog>("Logs");
    }

    public async Task AddAsync(AuditLog log)
    {
        await _auditLogs.InsertOneAsync(log);
    }
}
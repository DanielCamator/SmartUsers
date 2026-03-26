
namespace ALService.Application.Interfaces
{
    public interface IIngestionService
    {
        Task CreateIndexAndUploadAsync(CancellationToken ct = default);
    }
}

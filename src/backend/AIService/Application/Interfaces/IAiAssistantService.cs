
using Domain.Models;
using System.Runtime.CompilerServices;

namespace ALService.Application.Interfaces
{
    public interface IAiAssistantService
    {
        Task<AiResponseDto> AskQuestionAsync(string userQuestion, string sessionId, CancellationToken ct = default);
    }
}
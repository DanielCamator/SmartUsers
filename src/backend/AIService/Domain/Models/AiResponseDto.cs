using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;
public class AiResponseDto
{
    public string Answer { get; set; } = string.Empty;

    public long LatencyMs { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }

    public decimal EstimatedCostUsd { get; set; }
}

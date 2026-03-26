using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs;

public class AiRequestDto
{
    [Required(ErrorMessage = "La pregunta es obligatoria.")]
    [MaxLength(500, ErrorMessage = "La pregunta no puede exceder los 500 caracteres.")]
    public string Question { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}
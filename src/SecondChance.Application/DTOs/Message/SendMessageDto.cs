using System.ComponentModel.DataAnnotations;

namespace SecondChance.Application.DTOs.Message;

public sealed class SendMessageDto
{
    [Required, StringLength(4_000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}

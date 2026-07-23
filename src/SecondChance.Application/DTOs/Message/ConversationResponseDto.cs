using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Message
{
    public class ConversationResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string LastMessageContent { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty;
    }
}
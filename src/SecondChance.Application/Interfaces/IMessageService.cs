using SecondChance.Application.DTOs.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponseDto> SendMessageAsync(Guid productId, string senderId, string content);
        Task<IEnumerable<ConversationResponseDto>> GetConversationsAsync(string userId);
        Task<IEnumerable<MessageResponseDto>> GetMessageHistoryAsync(Guid conversationId, string userId);
    }
}

using SecondChance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<Conversation?> GetConversationAsync(string buyerId, string sellerId, Guid productId);
        Task<Conversation?> GetConversationByIdAsync(Guid conversationId);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
        Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId);
        Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(string userId);
        Task AddConversationAsync(Conversation conversation);
        Task AddMessageAsync(Message message);
    }
}

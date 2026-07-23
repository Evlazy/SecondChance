using Microsoft.EntityFrameworkCore;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;
using SecondChance.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Infrastructure.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddConversationAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Conversation?> GetConversationAsync(string buyerId, string sellerId, Guid productId)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.BuyerId == buyerId && 
                                        c.SellerId == sellerId && 
                                        c.ProductId == productId);
        }

        public async Task<Conversation?> GetConversationByIdAsync(Guid conversationId)
        {
            return await _context.Conversations
                .Include(c => c.Product)
                .Include(c => c.Buyer)
                .Include(c => c.Seller)
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId)
        {
            return await _context.Conversations
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images)
                .Include(c => c.Buyer)
                .Include(c => c.Seller)
                .Where(c => c.BuyerId == userId || c.SellerId == userId)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(string userId)
        {
            return await _context.Conversations
                .Include(c => c.Messages)
                .Where(c => c.BuyerId == userId || c.SellerId == userId)
                .OrderByDescending(c => c.Messages.Max(m => m.CreatedAt))
                .ToListAsync();
        }
    }
}

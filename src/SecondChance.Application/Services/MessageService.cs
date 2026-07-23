using SecondChance.Application.DTOs.Message;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SecondChance.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IProductRepository _productRepo; // 需要查商品持有者
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(
            IMessageRepository messageRepo,
            IProductRepository productRepo,
            IUnitOfWork unitOfWork)
        {
            _messageRepo = messageRepo;
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ConversationResponseDto>> GetConversationsAsync(string userId)
        {
            var conversations = await _messageRepo.GetConversationsByUserIdAsync(userId);

            return conversations.Select(c =>
            {
                var lastMsg = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

                var otherUserId = c.BuyerId == userId ? c.SellerId : c.BuyerId;

                return new ConversationResponseDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    LastMessageContent = lastMsg?.Content ?? "No message",
                    LastMessageTime = lastMsg?.CreatedAt ?? c.CreatedAt,
                    OtherUserId = otherUserId
                };
            });
        }

        public async Task<IEnumerable<MessageResponseDto>> GetMessageHistoryAsync(Guid conversationId, string userId)
        {
            var conversation = await _messageRepo.GetConversationByIdAsync(conversationId);
            if(conversation == null)
            {
                throw new KeyNotFoundException("Message did not found.");
            }

            if(conversation.BuyerId != userId && conversation.SellerId != userId)
            {
                throw new UnauthorizedAccessException("You don't have permission to see the chat history.");
            }

            var messages = await _messageRepo.GetMessagesByConversationIdAsync(conversationId);

            var unreadMessage = messages.Where(m => m.SenderId != userId && !m.IsRead).ToList();
            if (unreadMessage.Any())
            {
                foreach(var msg in unreadMessage)
                {
                    msg.IsRead = true;
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return messages.Select(m => new MessageResponseDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            });
        }

        public async Task<MessageResponseDto> SendMessageAsync(Guid productId, string senderId, string content)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if(product == null)
            {
                throw new KeyNotFoundException("Product do not exist, can not start conversation");
            }

            if(product.SellerId == senderId)
            {
                throw new InvalidOperationException("You can not talk to yourself");
            }

            string buyerId = senderId;
            string sellerId = product.SellerId;

            var conversation = await _messageRepo.GetConversationAsync(buyerId, sellerId, productId);

            if(conversation == null)
            {
                conversation = new Conversation
                {
                    ProductId = productId,
                    BuyerId = buyerId,
                    SellerId = sellerId,
                    LastMessageAt = DateTime.UtcNow
                };

                await _messageRepo.AddConversationAsync(conversation);
            }
            else
            {
                conversation.LastMessageAt = DateTime.UtcNow;
            }

            var newMessage = new Message
            {
                Conversation = conversation,
                SenderId = senderId,
                Content = content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepo.AddMessageAsync(newMessage);

            await _unitOfWork.SaveChangesAsync();

            return new MessageResponseDto
            {
                Id = newMessage.Id,
                ConversationId = conversation.Id,
                SenderId = newMessage.SenderId,
                Content = newMessage.Content,
                CreatedAt = newMessage.CreatedAt,
                IsRead = newMessage.IsRead
            };
        }
    }
}

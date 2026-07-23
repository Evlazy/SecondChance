using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondChance.Application.DTOs.Message;
using SecondChance.Application.Interfaces;
using SecondChance.Application.Services;
using System.Security.Claims;

namespace SecondChance.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("products/{productId}")]
        public async Task<IActionResult> SendMessage([FromRoute] Guid productId, [FromBody] SendMessageDto dto)
        {

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("You must loggin to send message");
            }

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest(new { message = "Message can not be empty" });
            }

            try
            {
                var response = await _messageService.SendMessageAsync(productId, currentUserId, dto.Content);
                return Ok(response);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("conversation")]
        public async Task<IActionResult> GetMyConversation()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var conversations = await _messageService.GetConversationsAsync(currentUserId);
            return Ok(conversations);
        }

        [HttpGet("conversations/{conversationId}/history")]
        public async Task<IActionResult> GetMessageHistory([FromRoute] Guid conversationId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            try
            {
                var message = await _messageService.GetMessageHistoryAsync(conversationId,currentUserId);
                return Ok(message);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch(UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }
    }
}

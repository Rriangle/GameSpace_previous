using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 聊天控制器
    /// </summary>
    public class ChatController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public ChatController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 聊天列表頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var conversations = await _context.DM_Conversations
                .Include(c => c.Party1)
                .Include(c => c.Party2)
                .Include(c => c.DM_Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(c => (c.Party1Id == userId || c.Party2Id == userId) && c.IsActive)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            return View(conversations);
        }

        /// <summary>
        /// 聊天對話頁面
        /// </summary>
        public async Task<IActionResult> Conversation(int conversationId)
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var conversation = await _context.DM_Conversations
                .Include(c => c.Party1)
                .Include(c => c.Party2)
                .Include(c => c.DM_Messages.OrderBy(m => m.SentAt))
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId && 
                                        (c.Party1Id == userId || c.Party2Id == userId));

            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }

        /// <summary>
        /// 開始新對話
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> StartConversation(int otherUserId)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                // 檢查是否已經有對話
                var existingConversation = await _context.DM_Conversations
                    .FirstOrDefaultAsync(c => ((c.Party1Id == userId && c.Party2Id == otherUserId) ||
                                             (c.Party1Id == otherUserId && c.Party2Id == userId)) && c.IsActive);

                if (existingConversation != null)
                {
                    return Json(new { success = true, conversationId = existingConversation.ConversationId });
                }

                // 創建新對話
                var conversation = new DM_Conversations
                {
                    IsManagerDm = false,
                    Party1Id = userId,
                    Party2Id = otherUserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.DM_Conversations.Add(conversation);
                await _context.SaveChangesAsync();

                return Json(new { success = true, conversationId = conversation.ConversationId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"創建對話失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 發送消息
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageRequest request)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                // 檢查對話是否存在
                var conversation = await _context.DM_Conversations
                    .FirstOrDefaultAsync(c => c.ConversationId == request.ConversationId && 
                                            (c.Party1Id == userId || c.Party2Id == userId) && c.IsActive);

                if (conversation == null)
                {
                    return Json(new { success = false, message = "對話不存在" });
                }

                var message = new DM_Messages
                {
                    ConversationId = request.ConversationId,
                    SenderUserId = userId,
                    MessageContent = request.Content,
                    MessageType = request.MessageType,
                    SentAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.DM_Messages.Add(message);

                // 更新對話的最後消息時間
                conversation.LastMessageAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, messageId = message.MessageId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"發送失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 獲取消息歷史
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMessages(int conversationId, int page = 1, int pageSize = 50)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                // 檢查對話權限
                var conversation = await _context.DM_Conversations
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId && 
                                            (c.Party1Id == userId || c.Party2Id == userId) && c.IsActive);

                if (conversation == null)
                {
                    return Json(new { success = false, message = "對話不存在" });
                }

                var messages = await _context.DM_Messages
                    .Include(m => m.SenderUser)
                    .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                    .OrderByDescending(m => m.SentAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new
                    {
                        m.MessageId,
                        m.MessageContent,
                        m.MessageType,
                        m.SentAt,
                        m.ReadAt,
                        m.IsEdited,
                        m.EditedAt,
                        SenderName = m.SenderUser.Username,
                        SenderId = m.SenderUserId
                    })
                    .ToListAsync();

                return Json(new { success = true, messages = messages.Reverse() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"獲取消息失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 標記消息為已讀
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            try
            {
                var message = await _context.DM_Messages
                    .FirstOrDefaultAsync(m => m.MessageId == messageId);

                if (message == null)
                {
                    return Json(new { success = false, message = "消息不存在" });
                }

                message.ReadAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "消息已標記為已讀" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"標記失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 刪除消息
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                var message = await _context.DM_Messages
                    .FirstOrDefaultAsync(m => m.MessageId == messageId && m.SenderUserId == userId);

                if (message == null)
                {
                    return Json(new { success = false, message = "消息不存在或無權限" });
                }

                message.IsDeleted = true;
                message.DeletedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "消息已刪除" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"刪除失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 編輯消息
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EditMessage(int messageId, string newContent)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                var message = await _context.DM_Messages
                    .FirstOrDefaultAsync(m => m.MessageId == messageId && m.SenderUserId == userId);

                if (message == null)
                {
                    return Json(new { success = false, message = "消息不存在或無權限" });
                }

                message.MessageContent = newContent;
                message.IsEdited = true;
                message.EditedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "消息已編輯" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"編輯失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 獲取未讀消息數量
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            try
            {
                var unreadCount = await _context.DM_Messages
                    .Include(m => m.Conversation)
                    .Where(m => m.Conversation.Party1Id == userId || m.Conversation.Party2Id == userId)
                    .Where(m => m.SenderUserId != userId && m.ReadAt == null && !m.IsDeleted)
                    .CountAsync();

                return Json(new { unreadCount });
            }
            catch (Exception ex)
            {
                return Json(new { unreadCount = 0, error = ex.Message });
            }
        }
    }

    /// <summary>
    /// 發送消息請求模型
    /// </summary>
    public class SendMessageRequest
    {
        public int ConversationId { get; set; }
        public string Content { get; set; } = null!;
        public string MessageType { get; set; } = "Text";
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
    }
}
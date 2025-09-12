using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameSpace.Services.Forum
{
    public class ForumService : IForumService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<ForumService> _logger;

        public ForumService(GameSpacedatabaseContext context, ILogger<ForumService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ForumResult> CreateForumAsync(string name, string description, int? gameId)
        {
            try
            {
                var forum = new Forum
                {
                    Name = name,
                    Description = description,
                    GameId = gameId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Forums.Add(forum);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Forum created: {ForumName} (ID: {ForumId})", name, forum.ForumId);
                return new ForumResult { Success = true, Message = "論壇創建成功", Forum = forum };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create forum: {ForumName}", name);
                return new ForumResult { Success = false, Message = "論壇創建失敗" };
            }
        }

        public async Task<ForumResult> GetForumAsync(int forumId)
        {
            try
            {
                var forum = await _context.Forums
                    .Include(f => f.Game)
                    .Include(f => f.Threads)
                    .FirstOrDefaultAsync(f => f.ForumId == forumId);

                if (forum == null)
                {
                    return new ForumResult { Success = false, Message = "論壇不存在" };
                }

                return new ForumResult { Success = true, Message = "論壇獲取成功", Forum = forum };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get forum: {ForumId}", forumId);
                return new ForumResult { Success = false, Message = "論壇獲取失敗" };
            }
        }

        public async Task<List<Forum>> GetAllForumsAsync()
        {
            try
            {
                return await _context.Forums
                    .Include(f => f.Game)
                    .Include(f => f.Threads)
                    .OrderBy(f => f.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all forums");
                return new List<Forum>();
            }
        }

        public async Task<ForumResult> UpdateForumAsync(int forumId, string name, string description)
        {
            try
            {
                var forum = await _context.Forums.FindAsync(forumId);
                if (forum == null)
                {
                    return new ForumResult { Success = false, Message = "論壇不存在" };
                }

                forum.Name = name;
                forum.Description = description;

                _context.Forums.Update(forum);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Forum updated: {ForumId}", forumId);
                return new ForumResult { Success = true, Message = "論壇更新成功", Forum = forum };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update forum: {ForumId}", forumId);
                return new ForumResult { Success = false, Message = "論壇更新失敗" };
            }
        }

        public async Task<ForumResult> DeleteForumAsync(int forumId)
        {
            try
            {
                var forum = await _context.Forums.FindAsync(forumId);
                if (forum == null)
                {
                    return new ForumResult { Success = false, Message = "論壇不存在" };
                }

                _context.Forums.Remove(forum);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Forum deleted: {ForumId}", forumId);
                return new ForumResult { Success = true, Message = "論壇刪除成功" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete forum: {ForumId}", forumId);
                return new ForumResult { Success = false, Message = "論壇刪除失敗" };
            }
        }

        public async Task<ThreadResult> CreateThreadAsync(int forumId, int userId, string title, string content)
        {
            try
            {
                var thread = new Thread
                {
                    ForumId = forumId,
                    UserId = userId,
                    Title = title,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Threads.Add(thread);
                await _context.SaveChangesAsync();

                // Create the first post
                var post = new Post
                {
                    ThreadId = thread.ThreadId,
                    UserId = userId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Thread created: {ThreadTitle} (ID: {ThreadId})", title, thread.ThreadId);
                return new ThreadResult { Success = true, Message = "討論串創建成功", Thread = thread };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create thread: {ThreadTitle}", title);
                return new ThreadResult { Success = false, Message = "討論串創建失敗" };
            }
        }

        public async Task<ThreadResult> GetThreadAsync(int threadId)
        {
            try
            {
                var thread = await _context.Threads
                    .Include(t => t.Forum)
                    .Include(t => t.User)
                    .Include(t => t.Posts)
                    .FirstOrDefaultAsync(t => t.ThreadId == threadId);

                if (thread == null)
                {
                    return new ThreadResult { Success = false, Message = "討論串不存在" };
                }

                return new ThreadResult { Success = true, Message = "討論串獲取成功", Thread = thread };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get thread: {ThreadId}", threadId);
                return new ThreadResult { Success = false, Message = "討論串獲取失敗" };
            }
        }

        public async Task<List<Thread>> GetForumThreadsAsync(int forumId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.Threads
                    .Where(t => t.ForumId == forumId)
                    .Include(t => t.User)
                    .Include(t => t.Posts)
                    .OrderByDescending(t => t.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get forum threads: {ForumId}", forumId);
                return new List<Thread>();
            }
        }

        public async Task<PostResult> CreatePostAsync(int threadId, int userId, string content, int? parentPostId = null)
        {
            try
            {
                var post = new Post
                {
                    ThreadId = threadId,
                    UserId = userId,
                    Content = content,
                    ParentPostId = parentPostId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Post created: {PostId} in thread {ThreadId}", post.PostId, threadId);
                return new PostResult { Success = true, Message = "文章發布成功", Post = post };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create post in thread: {ThreadId}", threadId);
                return new PostResult { Success = false, Message = "文章發布失敗" };
            }
        }

        public async Task<PostResult> GetPostAsync(int postId)
        {
            try
            {
                var post = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Thread)
                    .Include(p => p.Reactions)
                    .FirstOrDefaultAsync(p => p.PostId == postId);

                if (post == null)
                {
                    return new PostResult { Success = false, Message = "文章不存在" };
                }

                return new PostResult { Success = true, Message = "文章獲取成功", Post = post };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get post: {PostId}", postId);
                return new PostResult { Success = false, Message = "文章獲取失敗" };
            }
        }

        public async Task<List<Post>> GetThreadPostsAsync(int threadId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.Posts
                    .Where(p => p.ThreadId == threadId)
                    .Include(p => p.User)
                    .Include(p => p.Reactions)
                    .OrderBy(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get thread posts: {ThreadId}", threadId);
                return new List<Post>();
            }
        }

        public async Task<ReactionResult> AddReactionAsync(int postId, int userId, string reactionType)
        {
            try
            {
                // Check if user already reacted to this post
                var existingReaction = await _context.Reactions
                    .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

                if (existingReaction != null)
                {
                    return new ReactionResult { Success = false, Message = "您已經對這篇文章做出反應" };
                }

                var reaction = new Reaction
                {
                    PostId = postId,
                    UserId = userId,
                    ReactionType = reactionType,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reactions.Add(reaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reaction added: {ReactionType} to post {PostId} by user {UserId}", reactionType, postId, userId);
                return new ReactionResult { Success = true, Message = "反應添加成功", Reaction = reaction };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add reaction to post: {PostId}", postId);
                return new ReactionResult { Success = false, Message = "反應添加失敗" };
            }
        }

        public async Task<ReactionResult> RemoveReactionAsync(int postId, int userId, string reactionType)
        {
            try
            {
                var reaction = await _context.Reactions
                    .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId && r.ReactionType == reactionType);

                if (reaction == null)
                {
                    return new ReactionResult { Success = false, Message = "反應不存在" };
                }

                _context.Reactions.Remove(reaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reaction removed: {ReactionType} from post {PostId} by user {UserId}", reactionType, postId, userId);
                return new ReactionResult { Success = true, Message = "反應移除成功" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove reaction from post: {PostId}", postId);
                return new ReactionResult { Success = false, Message = "反應移除失敗" };
            }
        }

        public async Task<List<Reaction>> GetPostReactionsAsync(int postId)
        {
            try
            {
                return await _context.Reactions
                    .Where(r => r.PostId == postId)
                    .Include(r => r.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get post reactions: {PostId}", postId);
                return new List<Reaction>();
            }
        }

        public async Task<SearchResult> SearchPostsAsync(string keyword, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Posts
                    .Where(p => p.Content.Contains(keyword))
                    .Include(p => p.User)
                    .Include(p => p.Thread);

                var totalCount = await query.CountAsync();
                var posts = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new SearchResult
                {
                    Success = true,
                    Message = "搜尋成功",
                    Posts = posts,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search posts with keyword: {Keyword}", keyword);
                return new SearchResult { Success = false, Message = "搜尋失敗" };
            }
        }

        public async Task<SearchResult> SearchThreadsAsync(string keyword, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Threads
                    .Where(t => t.Title.Contains(keyword) || t.Posts.Any(p => p.Content.Contains(keyword)))
                    .Include(t => t.User)
                    .Include(t => t.Forum);

                var totalCount = await query.CountAsync();
                var threads = await query
                    .OrderByDescending(t => t.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new SearchResult
                {
                    Success = true,
                    Message = "搜尋成功",
                    Threads = threads,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search threads with keyword: {Keyword}", keyword);
                return new SearchResult { Success = false, Message = "搜尋失敗" };
            }
        }
    }
}
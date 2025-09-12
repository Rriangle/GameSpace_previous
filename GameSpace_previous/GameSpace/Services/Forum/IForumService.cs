using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameSpace.Services.Forum
{
    public interface IForumService
    {
        Task<ForumResult> CreateForumAsync(string name, string description, int? gameId);
        Task<ForumResult> GetForumAsync(int forumId);
        Task<List<Forum>> GetAllForumsAsync();
        Task<ForumResult> UpdateForumAsync(int forumId, string name, string description);
        Task<ForumResult> DeleteForumAsync(int forumId);
        Task<ThreadResult> CreateThreadAsync(int forumId, int userId, string title, string content);
        Task<ThreadResult> GetThreadAsync(int threadId);
        Task<List<Thread>> GetForumThreadsAsync(int forumId, int page = 1, int pageSize = 20);
        Task<PostResult> CreatePostAsync(int threadId, int userId, string content, int? parentPostId = null);
        Task<PostResult> GetPostAsync(int postId);
        Task<List<Post>> GetThreadPostsAsync(int threadId, int page = 1, int pageSize = 20);
        Task<ReactionResult> AddReactionAsync(int postId, int userId, string reactionType);
        Task<ReactionResult> RemoveReactionAsync(int postId, int userId, string reactionType);
        Task<List<Reaction>> GetPostReactionsAsync(int postId);
        Task<SearchResult> SearchPostsAsync(string keyword, int page = 1, int pageSize = 20);
        Task<SearchResult> SearchThreadsAsync(string keyword, int page = 1, int pageSize = 20);
    }

    public class ForumResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Forum? Forum { get; set; }
    }

    public class ThreadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Thread? Thread { get; set; }
    }

    public class PostResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Post? Post { get; set; }
    }

    public class ReactionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Reaction? Reaction { get; set; }
    }

    public class SearchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<Post>? Posts { get; set; }
        public List<Thread>? Threads { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
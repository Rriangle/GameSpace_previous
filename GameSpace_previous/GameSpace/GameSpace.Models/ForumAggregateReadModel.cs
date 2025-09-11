namespace GameSpace.Models
{
    /// <summary>
    /// 論壇列表聚合讀取模型 - Stage 2 廣度切片
    /// 提供論壇列表視圖所需的聚合資訊
    /// </summary>
    public class ForumListReadModel
    {
        /// <summary>
        /// 論壇 ID
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        /// 論壇名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 論壇描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 總主題數
        /// </summary>
        public int TotalThreads { get; set; }

        /// <summary>
        /// 總回覆數
        /// </summary>
        public int TotalPosts { get; set; }

        /// <summary>
        /// 最新主題資訊
        /// </summary>
        public LatestThreadInfo? LatestThread { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 最新主題資訊
    /// </summary>
    public class LatestThreadInfo
    {
        public long ThreadId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// 論壇詳情聚合讀取模型 - Stage 2 廣度切片
    /// 提供論壇詳情頁面所需的聚合資訊
    /// </summary>
    public class ForumDetailReadModel
    {
        /// <summary>
        /// 論壇基本資訊
        /// </summary>
        public ForumReadModel Forum { get; set; } = new ForumReadModel();

        /// <summary>
        /// 主題列表（分頁）
        /// </summary>
        public List<ThreadSummaryReadModel> Threads { get; set; } = new List<ThreadSummaryReadModel>();

        /// <summary>
        /// 置頂主題列表
        /// </summary>
        public List<ThreadSummaryReadModel> PinnedThreads { get; set; } = new List<ThreadSummaryReadModel>();

        /// <summary>
        /// 總主題數
        /// </summary>
        public int TotalThreads { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 當前頁數
        /// </summary>
        public int CurrentPage { get; set; }
    }

    /// <summary>
    /// 主題摘要讀取模型
    /// </summary>
    public class ThreadSummaryReadModel
    {
        public long ThreadId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 回覆數量
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// 最後回覆者名稱
        /// </summary>
        public string? LastPosterName { get; set; }

        /// <summary>
        /// 最後回覆時間
        /// </summary>
        public DateTime? LastPostTime { get; set; }

        /// <summary>
        /// 是否為置頂主題
        /// </summary>
        public bool IsPinned { get; set; }
    }

    /// <summary>
    /// 主題詳情聚合讀取模型 - Stage 2 廣度切片
    /// 提供主題詳情頁面所需的聚合資訊
    /// </summary>
    public class ThreadDetailReadModel
    {
        /// <summary>
        /// 主題基本資訊
        /// </summary>
        public ThreadReadModel Thread { get; set; } = new ThreadReadModel();

        /// <summary>
        /// 作者資訊
        /// </summary>
        public AuthorInfo Author { get; set; } = new AuthorInfo();

        /// <summary>
        /// 回覆列表（分頁）
        /// </summary>
        public List<PostDetailReadModel> Posts { get; set; } = new List<PostDetailReadModel>();

        /// <summary>
        /// 總回覆數
        /// </summary>
        public int TotalPosts { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 當前頁數
        /// </summary>
        public int CurrentPage { get; set; }
    }

    /// <summary>
    /// 作者資訊
    /// </summary>
    public class AuthorInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserNickName { get; set; }
    }

    /// <summary>
    /// 回覆詳情讀取模型
    /// </summary>
    public class PostDetailReadModel
    {
        public long PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public AuthorInfo Author { get; set; } = new AuthorInfo();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 父回覆 ID（用於巢狀回覆）
        /// </summary>
        public long? ParentPostId { get; set; }

        /// <summary>
        /// 反應統計
        /// </summary>
        public List<ReactionSummary> Reactions { get; set; } = new List<ReactionSummary>();
    }

    /// <summary>
    /// 反應統計
    /// </summary>
    public class ReactionSummary
    {
        public string Kind { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

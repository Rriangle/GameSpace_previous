namespace GameSpace.Models
{
    /// <summary>
    /// �׾¦C��E�XŪ���ҫ� - Stage 2 �s�פ���
    /// ���ѽ׾¦C����ϩһݪ��E�X��T
    /// </summary>
    public class ForumListReadModel
    {
        /// <summary>
        /// �׾� ID
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        /// �׾¦W��
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// �׾´y�z
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// �`�D�D��
        /// </summary>
        public int TotalThreads { get; set; }

        /// <summary>
        /// �`�^�м�
        /// </summary>
        public int TotalPosts { get; set; }

        /// <summary>
        /// �̷s�D�D��T
        /// </summary>
        public LatestThreadInfo? LatestThread { get; set; }

        /// <summary>
        /// �إ߮ɶ�
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// �̷s�D�D��T
    /// </summary>
    public class LatestThreadInfo
    {
        public long ThreadId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// �׾¸Ա��E�XŪ���ҫ� - Stage 2 �s�פ���
    /// ���ѽ׾¸Ա������һݪ��E�X��T
    /// </summary>
    public class ForumDetailReadModel
    {
        /// <summary>
        /// �׾°򥻸�T
        /// </summary>
        public ForumReadModel Forum { get; set; } = new ForumReadModel();

        /// <summary>
        /// �D�D�C��]�����^
        /// </summary>
        public List<ThreadSummaryReadModel> Threads { get; set; } = new List<ThreadSummaryReadModel>();

        /// <summary>
        /// �m���D�D�C��
        /// </summary>
        public List<ThreadSummaryReadModel> PinnedThreads { get; set; } = new List<ThreadSummaryReadModel>();

        /// <summary>
        /// �`�D�D��
        /// </summary>
        public int TotalThreads { get; set; }

        /// <summary>
        /// �`����
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// ��e����
        /// </summary>
        public int CurrentPage { get; set; }
    }

    /// <summary>
    /// �D�D�K�nŪ���ҫ�
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
        /// �^�мƶq
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// �̫�^�Ъ̦W��
        /// </summary>
        public string? LastPosterName { get; set; }

        /// <summary>
        /// �̫�^�Юɶ�
        /// </summary>
        public DateTime? LastPostTime { get; set; }

        /// <summary>
        /// �O�_���m���D�D
        /// </summary>
        public bool IsPinned { get; set; }
    }

    /// <summary>
    /// �D�D�Ա��E�XŪ���ҫ� - Stage 2 �s�פ���
    /// ���ѥD�D�Ա������һݪ��E�X��T
    /// </summary>
    public class ThreadDetailReadModel
    {
        /// <summary>
        /// �D�D�򥻸�T
        /// </summary>
        public ThreadReadModel Thread { get; set; } = new ThreadReadModel();

        /// <summary>
        /// �@�̸�T
        /// </summary>
        public AuthorInfo Author { get; set; } = new AuthorInfo();

        /// <summary>
        /// �^�ЦC��]�����^
        /// </summary>
        public List<PostDetailReadModel> Posts { get; set; } = new List<PostDetailReadModel>();

        /// <summary>
        /// �`�^�м�
        /// </summary>
        public int TotalPosts { get; set; }

        /// <summary>
        /// �`����
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// ��e����
        /// </summary>
        public int CurrentPage { get; set; }
    }

    /// <summary>
    /// �@�̸�T
    /// </summary>
    public class AuthorInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserNickName { get; set; }
    }

    /// <summary>
    /// �^�иԱ�Ū���ҫ�
    /// </summary>
    public class PostDetailReadModel
    {
        public long PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public AuthorInfo Author { get; set; } = new AuthorInfo();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// ���^�� ID�]�Ω�_���^�С^
        /// </summary>
        public long? ParentPostId { get; set; }

        /// <summary>
        /// �����έp
        /// </summary>
        public List<ReactionSummary> Reactions { get; set; } = new List<ReactionSummary>();
    }

    /// <summary>
    /// �����έp
    /// </summary>
    public class ReactionSummary
    {
        public string Kind { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

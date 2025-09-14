using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 文件上傳資料表
    /// </summary>
    public partial class FileUpload
    {
        public int FileId { get; set; }
        public int? UserId { get; set; }
        public string FileName { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public string? Description { get; set; }
        public string UploadType { get; set; } = "General"; // Avatar, Product, Game, General
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users? User { get; set; }
    }
}
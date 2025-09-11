using System.Net;
using System.Text.Json;

namespace GameSpace.Models
{
    /// <summary>
    /// 統一錯誤回應模型
    /// </summary>
    public class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public Dictionary<string, object> Extensions { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string TraceId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 業務結果封裝
    /// </summary>
    /// <typeparam name="T">結果類型</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> ValidationErrors { get; set; } = new();

        public static Result<T> Success(T data)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static Result<T> Failure(string errorMessage, string? errorCode = null)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }

        public static Result<T> ValidationFailure(List<string> validationErrors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ValidationErrors = validationErrors,
                ErrorMessage = "驗證失敗"
            };
        }
    }

    /// <summary>
    /// 分頁結果封裝
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }
}

using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Services
{
    /// <summary>
    /// 遊戲熱度追蹤服務
    /// </summary>
    public class GameHeatTrackingService
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<GameHeatTrackingService> _logger;
        private readonly HttpClient _httpClient;

        public GameHeatTrackingService(GameSpaceDbContext context, ILogger<GameHeatTrackingService> logger, HttpClient httpClient)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// 計算遊戲熱度分數
        /// </summary>
        public decimal CalculateHeatScore(GameMetricDaily metric)
        {
            // 熱度分數計算公式
            // 權重：玩家數 40%，觀看數 25%，點讚數 15%，分享數 10%，評論數 5%，遊戲時間 5%
            var heatScore = 
                (metric.PlayerCount * 0.4m) +
                (metric.ViewCount * 0.25m) +
                (metric.LikeCount * 0.15m) +
                (metric.ShareCount * 0.10m) +
                (metric.CommentCount * 0.05m) +
                (metric.PlayTime * 0.05m);

            return Math.Round(heatScore, 2);
        }

        /// <summary>
        /// 更新遊戲每日指標
        /// </summary>
        public async Task<bool> UpdateDailyMetricsAsync(int gameId, int sourceId, GameMetricData data)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                
                var existingMetric = await _context.GameMetricDailies
                    .FirstOrDefaultAsync(m => m.GameId == gameId && m.SourceId == sourceId && m.MetricDate == today);

                if (existingMetric != null)
                {
                    // 更新現有記錄
                    existingMetric.PlayerCount = data.PlayerCount;
                    existingMetric.ViewCount = data.ViewCount;
                    existingMetric.LikeCount = data.LikeCount;
                    existingMetric.ShareCount = data.ShareCount;
                    existingMetric.CommentCount = data.CommentCount;
                    existingMetric.PlayTime = data.PlayTime;
                    existingMetric.Revenue = data.Revenue;
                    existingMetric.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // 創建新記錄
                    var newMetric = new GameMetricDaily
                    {
                        GameId = gameId,
                        SourceId = sourceId,
                        MetricDate = today,
                        PlayerCount = data.PlayerCount,
                        ViewCount = data.ViewCount,
                        LikeCount = data.LikeCount,
                        ShareCount = data.ShareCount,
                        CommentCount = data.CommentCount,
                        PlayTime = data.PlayTime,
                        Revenue = data.Revenue,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.GameMetricDailies.Add(newMetric);
                }

                await _context.SaveChangesAsync();

                // 重新計算熱度分數
                await RecalculateHeatScoresAsync(gameId, today);

                _logger.LogInformation("成功更新遊戲 {GameId} 的每日指標", gameId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新遊戲 {GameId} 每日指標時發生錯誤", gameId);
                return false;
            }
        }

        /// <summary>
        /// 重新計算熱度分數
        /// </summary>
        public async Task RecalculateHeatScoresAsync(int gameId, DateTime date)
        {
            var metrics = await _context.GameMetricDailies
                .Where(m => m.GameId == gameId && m.MetricDate == date)
                .ToListAsync();

            foreach (var metric in metrics)
            {
                metric.HeatScore = CalculateHeatScore(metric);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成排行榜快照
        /// </summary>
        public async Task GenerateLeaderboardSnapshotAsync(string snapshotType)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var startDate = snapshotType switch
                {
                    "Daily" => today,
                    "Weekly" => today.AddDays(-7),
                    "Monthly" => today.AddDays(-30),
                    "AllTime" => DateTime.MinValue,
                    _ => today
                };

                var games = await _context.GameMetricDailies
                    .Where(m => m.MetricDate >= startDate)
                    .GroupBy(m => m.GameId)
                    .Select(g => new
                    {
                        GameId = g.Key,
                        TotalHeatScore = g.Sum(m => m.HeatScore),
                        TotalPlayerCount = g.Sum(m => m.PlayerCount),
                        TotalViewCount = g.Sum(m => m.ViewCount),
                        TotalLikeCount = g.Sum(m => m.LikeCount),
                        TotalShareCount = g.Sum(m => m.ShareCount),
                        TotalCommentCount = g.Sum(m => m.CommentCount),
                        TotalPlayTime = g.Sum(m => m.PlayTime),
                        TotalRevenue = g.Sum(m => m.Revenue)
                    })
                    .OrderByDescending(x => x.TotalHeatScore)
                    .ToListAsync();

                // 清除舊的快照
                var oldSnapshots = await _context.LeaderboardSnapshots
                    .Where(s => s.SnapshotType == snapshotType && s.SnapshotDate == today)
                    .ToListAsync();

                _context.LeaderboardSnapshots.RemoveRange(oldSnapshots);

                // 創建新的快照
                for (int i = 0; i < games.Count; i++)
                {
                    var game = games[i];
                    var snapshot = new LeaderboardSnapshot
                    {
                        GameId = game.GameId,
                        SnapshotType = snapshotType,
                        Rank = i + 1,
                        HeatScore = game.TotalHeatScore,
                        PlayerCount = game.TotalPlayerCount,
                        ViewCount = game.TotalViewCount,
                        LikeCount = game.TotalLikeCount,
                        ShareCount = game.TotalShareCount,
                        CommentCount = game.TotalCommentCount,
                        PlayTime = game.TotalPlayTime,
                        Revenue = game.TotalRevenue,
                        SnapshotDate = today,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.LeaderboardSnapshots.Add(snapshot);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("成功生成 {SnapshotType} 排行榜快照", snapshotType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成 {SnapshotType} 排行榜快照時發生錯誤", snapshotType);
            }
        }

        /// <summary>
        /// 獲取遊戲熱度排行榜
        /// </summary>
        public async Task<List<LeaderboardItem>> GetLeaderboardAsync(string snapshotType, int page = 1, int pageSize = 20)
        {
            var today = DateTime.UtcNow.Date;
            
            var snapshots = await _context.LeaderboardSnapshots
                .Include(s => s.Game)
                .Where(s => s.SnapshotType == snapshotType && s.SnapshotDate == today)
                .OrderBy(s => s.Rank)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return snapshots.Select(s => new LeaderboardItem
            {
                GameId = s.GameId,
                GameName = s.Game.GameName,
                Rank = s.Rank,
                HeatScore = s.HeatScore,
                PlayerCount = s.PlayerCount,
                ViewCount = s.ViewCount,
                LikeCount = s.LikeCount,
                ShareCount = s.ShareCount,
                CommentCount = s.CommentCount,
                PlayTime = s.PlayTime,
                Revenue = s.Revenue
            }).ToList();
        }

        /// <summary>
        /// 獲取遊戲熱度趨勢
        /// </summary>
        public async Task<List<HeatTrendItem>> GetHeatTrendAsync(int gameId, int days = 30)
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-days);

            var trends = await _context.GameMetricDailies
                .Where(m => m.GameId == gameId && m.MetricDate >= startDate && m.MetricDate <= endDate)
                .OrderBy(m => m.MetricDate)
                .Select(m => new HeatTrendItem
                {
                    Date = m.MetricDate,
                    HeatScore = m.HeatScore,
                    PlayerCount = m.PlayerCount,
                    ViewCount = m.ViewCount,
                    LikeCount = m.LikeCount,
                    ShareCount = m.ShareCount,
                    CommentCount = m.CommentCount,
                    PlayTime = m.PlayTime,
                    Revenue = m.Revenue
                })
                .ToListAsync();

            return trends;
        }

        /// <summary>
        /// 從外部API獲取遊戲數據
        /// </summary>
        public async Task<GameMetricData?> FetchExternalDataAsync(int sourceId, int gameId)
        {
            try
            {
                var source = await _context.MetricSources
                    .FirstOrDefaultAsync(s => s.SourceId == sourceId && s.IsActive);

                if (source == null || string.IsNullOrEmpty(source.ApiEndpoint))
                {
                    return null;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"{source.ApiEndpoint}/game/{gameId}/metrics");
                if (!string.IsNullOrEmpty(source.ApiKey))
                {
                    request.Headers.Add("Authorization", $"Bearer {source.ApiKey}");
                }

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // 這裡需要根據實際API響應格式進行解析
                    // 暫時返回模擬數據
                    return new GameMetricData
                    {
                        PlayerCount = Random.Shared.Next(100, 10000),
                        ViewCount = Random.Shared.Next(1000, 100000),
                        LikeCount = Random.Shared.Next(50, 5000),
                        ShareCount = Random.Shared.Next(10, 1000),
                        CommentCount = Random.Shared.Next(20, 2000),
                        PlayTime = Random.Shared.Next(100, 10000),
                        Revenue = Random.Shared.Next(1000, 100000)
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "從外部API獲取遊戲 {GameId} 數據時發生錯誤", gameId);
                return null;
            }
        }

        /// <summary>
        /// 自動更新所有活躍遊戲的指標
        /// </summary>
        public async Task UpdateAllGamesMetricsAsync()
        {
            var activeSources = await _context.MetricSources
                .Where(s => s.IsActive)
                .ToListAsync();

            var activeGames = await _context.Games
                .Where(g => g.IsActive)
                .ToListAsync();

            foreach (var source in activeSources)
            {
                foreach (var game in activeGames)
                {
                    var data = await FetchExternalDataAsync(source.SourceId, game.GameId);
                    if (data != null)
                    {
                        await UpdateDailyMetricsAsync(game.GameId, source.SourceId, data);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 遊戲指標數據
    /// </summary>
    public class GameMetricData
    {
        public decimal PlayerCount { get; set; }
        public decimal ViewCount { get; set; }
        public decimal LikeCount { get; set; }
        public decimal ShareCount { get; set; }
        public decimal CommentCount { get; set; }
        public decimal PlayTime { get; set; }
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// 排行榜項目
    /// </summary>
    public class LeaderboardItem
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public int Rank { get; set; }
        public decimal HeatScore { get; set; }
        public decimal PlayerCount { get; set; }
        public decimal ViewCount { get; set; }
        public decimal LikeCount { get; set; }
        public decimal ShareCount { get; set; }
        public decimal CommentCount { get; set; }
        public decimal PlayTime { get; set; }
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// 熱度趨勢項目
    /// </summary>
    public class HeatTrendItem
    {
        public DateTime Date { get; set; }
        public decimal HeatScore { get; set; }
        public decimal PlayerCount { get; set; }
        public decimal ViewCount { get; set; }
        public decimal LikeCount { get; set; }
        public decimal ShareCount { get; set; }
        public decimal CommentCount { get; set; }
        public decimal PlayTime { get; set; }
        public decimal Revenue { get; set; }
    }
}
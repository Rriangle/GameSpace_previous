using GameSpace.Core.Models;
using GameSpace.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 排行榜 API 控制器 - Stage 2 廣度切片
    /// 提供排行榜聚合查詢端點
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardReadOnlyRepository _leaderboardRepository;
        private readonly ILogger<LeaderboardController> _logger;

        public LeaderboardController(
            ILeaderboardReadOnlyRepository leaderboardRepository,
            ILogger<LeaderboardController> logger)
        {
            _leaderboardRepository = leaderboardRepository;
            _logger = logger;
        }

        /// <summary>
        /// 取得排行榜總覽
        /// </summary>
        /// <param name="userId">用戶 ID（可選，用於查詢個人排名）</param>
        /// <returns>排行榜總覽聚合資訊</returns>
        [HttpGet("overview")]
        public async Task<ActionResult<LeaderboardOverviewReadModel>> GetLeaderboardOverview(
            [FromQuery] int? userId = null)
        {
            try
            {
                _logger.LogInformation("正在查詢排行榜總覽 UserId: {UserId}", userId);

                var overview = await _leaderboardRepository.GetLeaderboardOverviewAsync(userId);
                
                _logger.LogInformation("成功取得排行榜總覽 UserId: {UserId}, DailyCount: {DailyCount}, WeeklyCount: {WeeklyCount}, MonthlyCount: {MonthlyCount}", 
                    userId, overview.DailyLeaderboard.Count, overview.WeeklyLeaderboard.Count, overview.MonthlyLeaderboard.Count);

                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢排行榜總覽時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得指定時間週期的排行榜
        /// </summary>
        /// <param name="period">時間週期（daily, weekly, monthly）</param>
        /// <param name="gameId">遊戲 ID（可選）</param>
        /// <param name="limit">取得數量限制（預設 50）</param>
        /// <returns>排行榜條目列表</returns>
        [HttpGet("period/{period}")]
        public async Task<ActionResult<List<LeaderboardEntryReadModel>>> GetLeaderboardByPeriod(
            string period,
            [FromQuery] int? gameId = null,
            [FromQuery] int limit = 50)
        {
            try
            {
                // 驗證時間週期參數
                var validPeriods = new[] { "daily", "weekly", "monthly" };
                if (!validPeriods.Contains(period.ToLower()))
                {
                    return BadRequest(new { Message = "無效的時間週期，支援：daily, weekly, monthly" });
                }

                // 驗證限制參數
                if (limit <= 0 || limit > 500) limit = 50;

                _logger.LogInformation("正在查詢排行榜 Period: {Period}, GameId: {GameId}, Limit: {Limit}", 
                    period, gameId, limit);

                var leaderboard = await _leaderboardRepository.GetLeaderboardByPeriodAsync(period, gameId, limit);
                
                _logger.LogInformation("成功取得排行榜 Period: {Period}, Count: {Count}", period, leaderboard.Count);

                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢排行榜時發生錯誤 Period: {Period}, GameId: {GameId}", period, gameId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得指定遊戲的排行榜
        /// </summary>
        /// <param name="gameId">遊戲 ID</param>
        /// <param name="period">時間週期（daily, weekly, monthly）</param>
        /// <param name="limit">取得數量限制（預設 50）</param>
        /// <returns>遊戲排行榜聚合資訊</returns>
        [HttpGet("game/{gameId:int}")]
        public async Task<ActionResult<GameLeaderboardReadModel>> GetGameLeaderboard(
            int gameId,
            [FromQuery] string period = "daily",
            [FromQuery] int limit = 50)
        {
            try
            {
                // 驗證時間週期參數
                var validPeriods = new[] { "daily", "weekly", "monthly" };
                if (!validPeriods.Contains(period.ToLower()))
                {
                    return BadRequest(new { Message = "無效的時間週期，支援：daily, weekly, monthly" });
                }

                // 驗證限制參數
                if (limit <= 0 || limit > 500) limit = 50;

                _logger.LogInformation("正在查詢遊戲排行榜 GameId: {GameId}, Period: {Period}, Limit: {Limit}", 
                    gameId, period, limit);

                var gameLeaderboard = await _leaderboardRepository.GetGameLeaderboardAsync(gameId, period, limit);
                
                if (gameLeaderboard == null)
                {
                    _logger.LogWarning("找不到遊戲排行榜資料 GameId: {GameId}", gameId);
                    return NotFound(new { Message = "找不到指定遊戲的排行榜資料" });
                }

                _logger.LogInformation("成功取得遊戲排行榜 GameId: {GameId}, Period: {Period}, Count: {Count}", 
                    gameId, period, gameLeaderboard.Entries.Count);

                return Ok(gameLeaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢遊戲排行榜時發生錯誤 GameId: {GameId}, Period: {Period}", gameId, period);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得用戶排名資訊
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>用戶排名資訊</returns>
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<UserRankingInfo>> GetUserRankingInfo(int userId)
        {
            try
            {
                _logger.LogInformation("正在查詢用戶排名資訊 UserId: {UserId}", userId);

                var userRanking = await _leaderboardRepository.GetUserRankingInfoAsync(userId);
                
                if (userRanking == null)
                {
                    _logger.LogWarning("找不到用戶排名資料 UserId: {UserId}", userId);
                    return NotFound(new { Message = "找不到指定用戶的排名資料" });
                }

                _logger.LogInformation("成功取得用戶排名資訊 UserId: {UserId}", userId);

                return Ok(userRanking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢用戶排名資訊時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得用戶排名歷史趨勢
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="gameId">遊戲 ID</param>
        /// <param name="period">時間週期</param>
        /// <param name="days">查詢天數（預設 30 天）</param>
        /// <returns>排名歷史列表</returns>
        [HttpGet("user/{userId:int}/history")]
        public async Task<ActionResult<List<RankingDetail>>> GetUserRankingHistory(
            int userId,
            [FromQuery] int gameId,
            [FromQuery] string period = "daily",
            [FromQuery] int days = 30)
        {
            try
            {
                // 驗證時間週期參數
                var validPeriods = new[] { "daily", "weekly", "monthly" };
                if (!validPeriods.Contains(period.ToLower()))
                {
                    return BadRequest(new { Message = "無效的時間週期，支援：daily, weekly, monthly" });
                }

                // 驗證天數參數
                if (days <= 0 || days > 365) days = 30;

                _logger.LogInformation("正在查詢用戶排名歷史 UserId: {UserId}, GameId: {GameId}, Period: {Period}, Days: {Days}", 
                    userId, gameId, period, days);

                var history = await _leaderboardRepository.GetUserRankingHistoryAsync(userId, gameId, period, days);
                
                _logger.LogInformation("成功取得用戶排名歷史 UserId: {UserId}, Count: {Count}", userId, history.Count);

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢用戶排名歷史時發生錯誤 UserId: {UserId}, GameId: {GameId}", userId, gameId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }
    }
}
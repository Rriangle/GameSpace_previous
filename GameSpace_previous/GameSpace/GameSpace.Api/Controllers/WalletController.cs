using GameSpace.Core.Models;
using GameSpace.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 錢包 API 控制器 - Stage 2 廣度切片
    /// 提供錢包總覽聚合查詢端點
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletReadOnlyRepository _walletRepository;
        private readonly ILogger<WalletController> _logger;

        public WalletController(
            IWalletReadOnlyRepository walletRepository,
            ILogger<WalletController> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        /// <summary>
        /// 取得用戶錢包總覽
        /// 聚合用戶積分、優惠券、電子禮券資訊
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>錢包總覽資訊</returns>
        [HttpGet("overview/{userId:int}")]
        public async Task<ActionResult<WalletOverviewReadModel>> GetWalletOverview(int userId)
        {
            try
            {
                _logger.LogInformation("正在查詢用戶錢包總覽 UserId: {UserId}", userId);

                var overview = await _walletRepository.GetWalletOverviewAsync(userId);
                
                if (overview == null)
                {
                    _logger.LogWarning("找不到用戶錢包資料 UserId: {UserId}", userId);
                    return NotFound(new { Message = "找不到指定用戶的錢包資料" });
                }

                _logger.LogInformation("成功取得用戶錢包總覽 UserId: {UserId}, Points: {Points}", 
                    userId, overview.CurrentPoints);

                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢用戶錢包總覽時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得用戶積分餘額
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>積分餘額</returns>
        [HttpGet("points/{userId:int}")]
        public async Task<ActionResult<int>> GetUserPoints(int userId)
        {
            try
            {
                _logger.LogInformation("正在查詢用戶積分餘額 UserId: {UserId}", userId);

                var points = await _walletRepository.GetUserPointsAsync(userId);
                
                _logger.LogInformation("成功取得用戶積分餘額 UserId: {UserId}, Points: {Points}", userId, points);

                return Ok(points);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢用戶積分餘額時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得用戶錢包異動歷史
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="pageIndex">頁數索引（預設 0）</param>
        /// <param name="pageSize">每頁筆數（預設 10）</param>
        /// <returns>錢包異動歷史列表</returns>
        [HttpGet("history/{userId:int}")]
        public async Task<ActionResult<List<WalletHistoryReadModel>>> GetWalletHistory(
            int userId, 
            [FromQuery] int pageIndex = 0, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("正在查詢用戶錢包異動歷史 UserId: {UserId}, Page: {PageIndex}, Size: {PageSize}", 
                    userId, pageIndex, pageSize);

                // 驗證分頁參數
                if (pageIndex < 0) pageIndex = 0;
                if (pageSize <= 0 || pageSize > 100) pageSize = 10;

                var history = await _walletRepository.GetWalletHistoryAsync(userId, pageIndex, pageSize);
                
                _logger.LogInformation("成功取得用戶錢包異動歷史 UserId: {UserId}, Count: {Count}", 
                    userId, history.Count);

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢用戶錢包異動歷史時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得用戶可用優惠券
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>可用優惠券列表</returns>
        [HttpGet("coupons/{userId:int}")]
        public async Task<ActionResult<List<CouponOverviewReadModel>>> GetAvailableCoupons(int userId)
        {
            try
            {
                _logger.LogInformation("正在查詢用戶可用優惠券 UserId: {UserId}", userId);

                var coupons = await _walletRepository.GetAvailableCouponsAsync(userId);
                
                _logger.LogInformation("成功取得用戶可用優惠券 UserId: {UserId}, Count: {Count}", 
                    userId, coupons.Count);

                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢用戶可用優惠券時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得用戶可用電子禮券
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>可用電子禮券列表</returns>
        [HttpGet("evouchers/{userId:int}")]
        public async Task<ActionResult<List<EVoucherOverviewReadModel>>> GetAvailableEVouchers(int userId)
        {
            try
            {
                _logger.LogInformation("正在查詢用戶可用電子禮券 UserId: {UserId}", userId);

                var evouchers = await _walletRepository.GetAvailableEVouchersAsync(userId);
                
                _logger.LogInformation("成功取得用戶可用電子禮券 UserId: {UserId}, Count: {Count}", 
                    userId, evouchers.Count);

                return Ok(evouchers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢用戶可用電子禮券時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }
    }
}
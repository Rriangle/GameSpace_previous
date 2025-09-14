using GameSpace.Core.Models;
using GameSpace.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 簽到 API 控制器 - Stage 3 寫入操作
    /// 提供用戶簽到寫入端點，包含交易處理和冪等性
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SignInController : ControllerBase
    {
        private readonly ISignInWriteRepository _signInRepository;
        private readonly ILogger<SignInController> _logger;

        public SignInController(
            ISignInWriteRepository signInRepository,
            ILogger<SignInController> logger)
        {
            _signInRepository = signInRepository;
            _logger = logger;
        }

        /// <summary>
        /// 用戶簽到
        /// 包含交易處理、冪等性檢查、積分獎勵、經驗值獎勵、優惠券獎勵
        /// </summary>
        /// <param name="request">簽到請求</param>
        /// <returns>簽到響應</returns>
        [HttpPost]
        public async Task<ActionResult<SignInResponse>> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                // 驗證請求
                if (request.UserId <= 0)
                {
                    _logger.LogWarning("無效的用戶 ID UserId: {UserId}", request.UserId);
                    return BadRequest(new { Message = "無效的用戶 ID" });
                }

                if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
                {
                    _logger.LogWarning("缺少冪等性密鑰 UserId: {UserId}", request.UserId);
                    return BadRequest(new { Message = "冪等性密鑰為必要欄位" });
                }

                if (request.IdempotencyKey.Length > 100)
                {
                    _logger.LogWarning("冪等性密鑰過長 UserId: {UserId}, KeyLength: {KeyLength}", 
                        request.UserId, request.IdempotencyKey.Length);
                    return BadRequest(new { Message = "冪等性密鑰長度不能超過 100 字符" });
                }

                _logger.LogInformation("處理簽到請求 UserId: {UserId}, IdempotencyKey: {IdempotencyKey}", 
                    request.UserId, request.IdempotencyKey);

                // 處理簽到
                var response = await _signInRepository.ProcessSignInAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation("簽到成功 UserId: {UserId}, Points: {Points}, Exp: {Exp}, ConsecutiveDays: {ConsecutiveDays}", 
                        request.UserId, response.PointsGained, response.ExpGained, response.ConsecutiveDays);
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("簽到失敗 UserId: {UserId}, Message: {Message}", request.UserId, response.Message);
                    
                    // 根據錯誤類型返回適當的 HTTP 狀態碼
                    if (response.Message.Contains("已經簽到"))
                    {
                        return Conflict(response);
                    }
                    else
                    {
                        return BadRequest(response);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "簽到處理時發生未預期的錯誤 UserId: {UserId}, IdempotencyKey: {IdempotencyKey}", 
                    request.UserId, request.IdempotencyKey);
                
                return StatusCode(500, new SignInResponse
                {
                    Success = false,
                    Message = "伺服器內部錯誤，請稍後再試",
                    IdempotencyKey = request.IdempotencyKey
                });
            }
        }

        /// <summary>
        /// 獲取用戶簽到統計
        /// 輔助端點，用於查詢用戶的簽到歷史
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>簽到統計</returns>
        [HttpGet("stats/{userId:int}")]
        public async Task<ActionResult<SignInStats>> GetSignInStats(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    _logger.LogWarning("無效的用戶 ID UserId: {UserId}", userId);
                    return BadRequest(new { Message = "無效的用戶 ID" });
                }

                _logger.LogInformation("查詢簽到統計 UserId: {UserId}", userId);

                var stats = await _signInRepository.GetOrCreateSignInStatsAsync(userId);
                
                _logger.LogInformation("成功取得簽到統計 UserId: {UserId}, ConsecutiveDays: {ConsecutiveDays}, TotalSignIns: {TotalSignIns}", 
                    userId, stats.ConsecutiveDays, stats.TotalSignIns);

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢簽到統計時發生錯誤 UserId: {UserId}", userId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 檢查冪等性狀態
        /// 開發輔助端點，用於檢查冪等性密鑰狀態
        /// </summary>
        /// <param name="idempotencyKey">冪等性密鑰</param>
        /// <returns>冪等性檢查結果</returns>
        [HttpGet("idempotency/{idempotencyKey}")]
        public async Task<ActionResult> CheckIdempotency(string idempotencyKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idempotencyKey))
                {
                    return BadRequest(new { Message = "冪等性密鑰不能為空" });
                }

                _logger.LogInformation("檢查冪等性密鑰 IdempotencyKey: {IdempotencyKey}", idempotencyKey);

                var existingResponse = await _signInRepository.CheckIdempotencyAsync(idempotencyKey);
                
                if (existingResponse != null)
                {
                    _logger.LogInformation("找到現有冪等性記錄 IdempotencyKey: {IdempotencyKey}", idempotencyKey);
                    return Ok(new { 
                        Exists = true, 
                        Response = existingResponse,
                        Message = "冪等性密鑰已存在，返回快取結果"
                    });
                }
                else
                {
                    _logger.LogInformation("未找到冪等性記錄 IdempotencyKey: {IdempotencyKey}", idempotencyKey);
                    return Ok(new { 
                        Exists = false, 
                        Message = "冪等性密鑰不存在，可以執行新操作"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查冪等性時發生錯誤 IdempotencyKey: {IdempotencyKey}", idempotencyKey);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }
    }
}
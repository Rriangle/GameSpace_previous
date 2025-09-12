using GameSpace.Models;

namespace GameSpace.Services.MiniGame
{
    public interface IMiniGameService
    {
        Task<MiniGameResult> StartGameAsync(int userId, int petId, int level);
        Task<MiniGameResult> EndGameAsync(int playId, string result, int monsterCount, decimal speedMultiplier);
        Task<MiniGameResult> AbortGameAsync(int playId);
        Task<List<MiniGame>> GetUserGamesAsync(int userId, int page = 1, int pageSize = 20);
        Task<MiniGameResult> GetGameStatsAsync(int userId);
        Task<bool> CanPlayGameAsync(int userId);
        Task<int> GetRemainingGamesAsync(int userId);
    }

    public class MiniGameResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public MiniGame? Game { get; set; }
        public int PointsGained { get; set; }
        public int ExpGained { get; set; }
        public string? CouponGained { get; set; }
        public int RemainingGames { get; set; }
        public string? Message { get; set; }
    }
}
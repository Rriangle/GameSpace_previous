using GameSpace.Models;

namespace GameSpace.Services.SignIn
{
    public interface ISignInService
    {
        Task<SignInResult> SignInAsync(int userId);
        Task<bool> HasSignedInTodayAsync(int userId);
        Task<List<UserSignInStat>> GetSignInHistoryAsync(int userId, int days = 30);
        Task<int> GetConsecutiveDaysAsync(int userId);
        Task<SignInResult> GetSignInRewardsAsync(int userId);
    }

    public class SignInResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int PointsGained { get; set; }
        public int ExpGained { get; set; }
        public string? CouponGained { get; set; }
        public int ConsecutiveDays { get; set; }
        public UserSignInStat? SignInRecord { get; set; }
    }
}
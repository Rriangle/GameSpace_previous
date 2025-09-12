namespace GameSpace.Services.Validation
{
    /// <summary>
    /// Validation service interface
    /// </summary>
    public interface IValidationService
    {
        Task<bool> ValidateUserAsync(int userId);
        Task<bool> ValidatePetAsync(int petId);
        Task<bool> ValidatePointsAsync(int userId, int points);
        Task<bool> ValidateCouponAsync(string couponCode);
        Task<bool> ValidateGameSessionAsync(int userId, int petId);
    }
}

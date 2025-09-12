using GameSpace.Models;

namespace GameSpace.Services.Pet
{
    public interface IPetService
    {
        Task<PetResult> GetPetAsync(int userId);
        Task<PetResult> CreatePetAsync(int userId, string petName);
        Task<PetResult> FeedPetAsync(int userId);
        Task<PetResult> PlayWithPetAsync(int userId);
        Task<PetResult> BathePetAsync(int userId);
        Task<PetResult> RestPetAsync(int userId);
        Task<PetResult> ChangeSkinColorAsync(int userId, string skinColor);
        Task<PetResult> ChangeBackgroundColorAsync(int userId, string backgroundColor);
        Task<PetResult> UpdatePetAttributesAsync(int userId);
        Task<List<Pet>> GetUserPetsAsync(int userId);
    }

    public class PetResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Pet? Pet { get; set; }
        public string? Message { get; set; }
    }
}
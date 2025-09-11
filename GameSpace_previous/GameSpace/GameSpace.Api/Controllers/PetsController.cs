using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IUserReadOnlyRepository _userRepository;

        public PetsController(IUserReadOnlyRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PetReadModel>> GetPetByUserId(int userId)
        {
            try
            {
                var pet = await _userRepository.GetPetByUserIdAsync(userId);
                if (pet == null)
                {
                    return NotFound("用戶沒有寵物");
                }
                return Ok(pet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/mini-games")]
        public async Task<ActionResult<IEnumerable<MiniGameReadModel>>> GetUserMiniGames(int userId, int limit = 10)
        {
            try
            {
                var miniGames = await _userRepository.GetUserMiniGamesAsync(userId, limit);
                return Ok(miniGames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

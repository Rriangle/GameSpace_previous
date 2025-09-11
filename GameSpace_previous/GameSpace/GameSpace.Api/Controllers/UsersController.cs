using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserReadOnlyRepository _userRepository;

        public UsersController(IUserReadOnlyRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadModel>>> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadModel>> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("account/{account}")]
        public async Task<ActionResult<UserReadModel>> GetUserByAccount(string account)
        {
            try
            {
                var user = await _userRepository.GetUserByAccountAsync(account);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetUserCount()
        {
            try
            {
                var count = await _userRepository.GetUserCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

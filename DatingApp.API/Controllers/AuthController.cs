using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto user)
        {
            // validate request
            user.UserName = user.UserName.ToLower();

            if (await _repo.UserExists(user.UserName))
                return BadRequest("User already exists");

            var userToCreate = new User
            {
                UserName = user.UserName
            };

            var createdUser = await _repo.Register(userToCreate, user.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserRegisterDto user)
        {
            var userData = await _repo.Login(user.UserName, user.Password);
            if(userData == null)
            return BadRequest("Invalid User name or password.");

            return Ok(user);
        }
    }
}
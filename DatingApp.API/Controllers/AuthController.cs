using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        // interface that allow us to access data in appsetting.json
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            this._mapper = mapper;
            this._config = config;
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto user)
        {
            // validate request
            user.UserName = user.UserName.ToLower();

            if (await _repo.UserExists(user.UserName))
                return BadRequest("User already exists");

            var userToCreate = _mapper.Map<User>(user);

            var createdUser = await _repo.Register(userToCreate, user.Password);

            var userToReturn = _mapper.Map<UserDetailDto>(createdUser);

            return CreatedAtRoute("GetUser", new{ controller = "user", id =createdUser.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserRegisterDto user)
        {
            var userData = await _repo.Login(user.UserName, user.Password);
            if (userData == null)
                return Unauthorized();

            return Ok(user);
        }

        [HttpPost("logintoken")]
        public async Task<IActionResult> LoginToken(UserLoginDto userLogin)
        {
            var userData = await _repo.Login(userLogin.UserName, userLogin.Password);
            if (userData == null)
                return Unauthorized();

            // variable to store claims so to get user info no need to get from DB again. claim will be fill in token
            var claims = new[]{
                    new Claim(ClaimTypes.NameIdentifier, userData.Id.ToString()),
                    new Claim(ClaimTypes.Name, userData.UserName)
                };

            // encode key to sign the token, signing key is to make sure the token on the next request from client is valid token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            // signing and encript the key as cridential with algorithm to hash
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // create security token descriptor that contain claims, signing credential, exp token date
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // to validate token discriptor we need token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            // using token handler we create roken and pass the descriptor, this token that returned to the client
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserListDto>(userData);

            // before returned to client serialized token
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });

            // to encode or decode token you can visit https://jwt.io
        }
    }
}
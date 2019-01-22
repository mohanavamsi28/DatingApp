using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        //public async Task<IActionResult> Register([FromBody]string data)
        {
            //Validate Request
            // if(!ModelState.IsValid)
            //     return BadRequest("Please provide valid credentials.");
            // UserForRegisterDTO userForRegisterDTO = new UserForRegisterDTO();
            // userForRegisterDTO = JsonConvert.DeserializeObject<UserForRegisterDTO>(data);
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
            if(await _repository.UserExists(userForRegisterDTO.Username))
                return BadRequest("Username already exists.");
            var userToCreate = new User 
            {
                Username = userForRegisterDTO.Username
            };
            var createdUser = await _repository.Register(userToCreate, userForRegisterDTO.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            var userFromRepository = await _repository.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);
            if(userFromRepository == null)
                return Unauthorized();
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,userFromRepository.Id.ToString()),
                    new Claim(ClaimTypes.Name,userFromRepository.Username)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));
                var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = credentials
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
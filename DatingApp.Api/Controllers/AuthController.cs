using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _config = config;
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repository.UserExists(userForRegisterDto.Username))
                return BadRequest("Userbame already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repository.Register(userToCreate, userForRegisterDto.Password);

            // return CreatedAtAction();
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // First we're check in to make sure that we have a user and their 
            // username and password match is stored in the database .
            var userFromRepository = await _repository.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            
            // If the user doesn't exist we will return a Unauthorized error
            if (userFromRepository == null)
                return Unauthorized();

            // Now we need to start building up our token
            // Our token is going to contain two claims
            var claims = new[] {
                // This is the user's ID
                new Claim(ClaimTypes.NameIdentifier, userFromRepository.Id.ToString()),
                // This is the user's username
                new Claim(ClaimTypes.Name, userFromRepository.Username)
            };

            // In order to make sure the token are valid token, when it's
            // come back, the server needs to sign this token.
            // and that's what we're doing in this part
            // We're creating a security key an the we're using
            // this key as part of the sign in credentials and
            // encrypted this key with a hashing algorithm
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Now we start to actually create the token and we create a token
            // descriptor and we pass in our claims as the subjects we given
            // an expiry date and then we pass in the sign in credentials
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Following this we can create a new Jwt security token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Which is allow us to create the token based on the tokenDescriptor
            // being passed in here and we still listen this token variable and then
            // we use this token variable to write the token into a response that we send
            // back to out client
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
//
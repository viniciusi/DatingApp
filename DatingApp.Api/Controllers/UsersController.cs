using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            // Since this return a list of users the type we're returning 
            // is going to beIEnumerable
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        // We used the Name to tell our route in Register method inside AuthController
        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto) {
            // This will check if the client is updating his/her own data
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // This will return the method above

            var userFromRepo = await _repo.GetUser(id);

            // Now we need to do is take the information that's in our UserForUpdateDto and map this into 
            // userFromRepo and to do that we can use the mapper Map method.
            // Since we have already using the map (GetUser method above) inside this controller, we're just
            // going to open.
            // This is going to execute the mapping and effectively updates the values from userForUpdateDto
            // and write them into our userFromRepo
            _mapper.Map(userForUpdateDto, userFromRepo);

            // After mapping, we will save all our changes.
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed to save");
        }

    }
}
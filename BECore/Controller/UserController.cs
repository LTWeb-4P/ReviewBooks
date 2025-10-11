using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Data;
using BECore.Interface;
using Microsoft.AspNetCore.Mvc;
using BECore.Dto;
using BECore.Repository;
using BECore.Models;

namespace BECore.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUser userRepository;

        public UserController(ApplicationDbContext dbContext, IUser userRepository)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAysnc()
        {
            var users = await userRepository.GetUsersAsync();

            var UserDtos = new List<UserDto>();
            foreach (var user in users)
            {
                UserDtos.Add(new UserDto()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl,
                });
            }
                return Ok(users);
        }
    }

}
using Microsoft.AspNetCore.Mvc;
using BECore.Users.Services;
using BECore.Users.Dto;
using AutoMapper;

namespace BECore.Users.Controller
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _userService.GetAllUserAsync();
            var userDto = mapper.Map<List<UserDto>>(users);
            return Ok(userDto);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AddUserRequestDto dto)
        {
            var createdUser = await _userService.CreateUserAsync(dto);
            var createdUserDto = mapper.Map<UserDto>(createdUser);
            return CreatedAtAction(
                nameof(GetByIdAsync),
                new { id = createdUser.Id },
                createdUserDto
            );
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateUserRequestDto dto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, dto);
            if (updatedUser == null) return NotFound();
            return Ok(mapper.Map<UpdateUserRequestDto>(dto));
        }

        [HttpDelete("{id}")]
        public async Task<UserDto?> DeleteUserAsync(Guid id)
        {
            var deletedUser = await _userService.DeleteUserAsync(id);
            return deletedUser == null ? null : mapper.Map<UserDto>(deletedUser);
        }
    }
}

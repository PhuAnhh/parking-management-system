using Final_year_Project.Api.Authorization;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [RequirePermission("GET", "/api/User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [RequirePermission("GET", "/api/User/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [RequirePermission("POST", "/api/User")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto createUserDto)
        {
            var createdUser = await _userService.CreateAsync(createUserDto);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [RequirePermission("PUT", "/api/User/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(int id, UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateAsync(id, updateUserDto);

            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        [RequirePermission("DELETE", "/api/User/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [RequirePermission("PATCH", "/api/User/{id}/change-password")]
        [HttpPatch("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            var success = await _userService.ChangePasswordAsync(id, changePasswordDto);
            if (!success)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }
            return Ok(new { message = "Password changed successfully." });
        }

        [RequirePermission("PATCH", "/api/User/{id}/reset-password")]
        [HttpPatch("{id}/reset-password")]
        public async Task<ActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _userService.ResetPasswordAsync(id, resetPasswordDto);
                if (!success)
                {
                    return NotFound(new { message = $"User with ID {id} not found." });
                }
                return Ok(new { message = "Password reset successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while resetting the password.", error = ex.Message });
            }
        }
    }
}

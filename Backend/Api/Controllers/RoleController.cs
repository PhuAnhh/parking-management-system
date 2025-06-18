using Final_year_Project.Api.Authorization;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Final_year_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [RequirePermission("GET", "/api/role")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving roles.", error = ex.Message });
            }
        }

        [RequirePermission("GET", "/api/role/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetById(int id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new { message = $"Role with ID {id} not found." });
                }
                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the role.", error = ex.Message });
            }
        }

        [RequirePermission("POST", "/api/role")]
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdRole = await _roleService.CreateAsync(createRoleDto);
                return CreatedAtAction(nameof(GetById), new { id = createdRole.Id }, createdRole);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the role.", error = ex.Message });
            }
        }

        [RequirePermission("PUT", "/api/role/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRole = await _roleService.UpdateAsync(id, updateRoleDto);
                if (updatedRole == null)
                {
                    return NotFound(new { message = $"Role with ID {id} not found." });
                }
                return Ok(updatedRole);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the role.", error = ex.Message });
            }
        }

        [RequirePermission("DELETE", "/api/role/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _roleService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Role with ID {id} not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the role.", error = ex.Message });
            }
        }
    }
}

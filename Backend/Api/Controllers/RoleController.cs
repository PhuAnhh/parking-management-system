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
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [RequirePermission("GET", "/api/role/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }
            return Ok(role);
        }

        [RequirePermission("POST", "/api/role")]
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var createdRole = await _roleService.CreateAsync(createRoleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdRole.Id }, createdRole);
        }

        [RequirePermission("PUT", "/api/role/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            var updatedRole = await _roleService.UpdateAsync(id, updateRoleDto);
            if (updatedRole == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }
            return Ok(updatedRole);
        }

        [RequirePermission("DELETE", "/api/role/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _roleService.DeleteAsync(id, true);
            if (!deleted)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }
            return NoContent();
        }
    }
}

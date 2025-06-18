using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;
using Final_year_Project.Api.Authorization;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerGroupController : ControllerBase
    {
        private readonly ICustomerGroupService _customerGroupService;

        public CustomerGroupController(ICustomerGroupService customerGroupService)
        {
            _customerGroupService = customerGroupService;
        }

        [RequirePermission("GET", "/api/customergroup")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerGroupDto>>> GetAll()
        {
            var customerGroups = await _customerGroupService.GetAllAsync();
            return Ok(customerGroups);
        }

        [RequirePermission("GET", "/api/customergroup/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerGroupDto>> GetById(int id)
        {
            var customerGroup = await _customerGroupService.GetByIdAsync(id);

            if (customerGroup == null)
                return NotFound();

            return Ok(customerGroup);
        }

        [RequirePermission("POST", "/api/customergroup")]
        [HttpPost]
        public async Task<ActionResult<CustomerGroupDto>> Create(CreateCustomerGroupDto createCustomerGroupDto)
        {
            var createdCustomerGroup = await _customerGroupService.CreateAsync(createCustomerGroupDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCustomerGroup.Id }, createdCustomerGroup);
        }

        [RequirePermission("PUT", "/api/customergroup/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerGroupDto>> Update(int id, UpdateCustomerGroupDto updateCustomerGroupDto)
        {
            var updatedCustomerGroup = await _customerGroupService.UpdateAsync(id, updateCustomerGroupDto);

            if (updatedCustomerGroup == null)
                return NotFound();

            return Ok(updatedCustomerGroup);
        }

        [RequirePermission("DELETE", "/api/customergroup/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _customerGroupService.DeleteAsync(id, true);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}

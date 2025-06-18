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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [RequirePermission("GET", "/api/customer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        [RequirePermission("GET", "/api/customer/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [RequirePermission("POST", "/api/customer")]
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto createCustomerDto)
        {
            var createdCustomer = await _customerService.CreateAsync(createCustomerDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
        }

        [RequirePermission("PUT", "/api/customer/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> Update(int id, UpdateCustomerDto updateCustomerDto)
        {
            var updatedCustomer = await _customerService.UpdateAsync(id, updateCustomerDto);

            if (updatedCustomer == null)
                return NotFound();

            return Ok(updatedCustomer);
        }

        [RequirePermission("DELETE", "/api/customer/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}

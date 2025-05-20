using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        public readonly ParkingManagementContext _context;

        public CameraRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Camera>> GetAllAsync()
        {
            return await _context.Cameras.ToListAsync();
        }

        public async Task<Camera> GetByIdAsync(int id)
        {
            return await _context.Cameras.FindAsync(id);
        }

        public async Task CreateAsync(Camera camera)
        {
            await _context.Cameras.AddAsync(camera);
        }

        public void Update(Camera camera)
        {
            _context.Cameras.Update(camera);
        }

        public void Delete(Camera camera)
        {
            _context.Cameras.Remove(camera);
        }
    }
}

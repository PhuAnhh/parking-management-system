using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class LaneCameraRepository : ILaneCameraRepository
    {
        public readonly ParkingManagementContext _context;

        public LaneCameraRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LaneCamera>> GetAllAsync()
        {
            return await _context.LaneCameras.ToListAsync();
        }

        public async Task<LaneCamera> GetByIdAsync(int id)
        {
            return await _context.LaneCameras.FindAsync(id);
        }

        public async Task CreateAsync(LaneCamera laneCamera)
        {
            await _context.LaneCameras.AddAsync(laneCamera);
        }

        public void Update(LaneCamera laneCamera)
        {
            _context.LaneCameras.Update(laneCamera);
        }

        public void Delete(LaneCamera laneCamera)
        {
            _context.LaneCameras.Remove(laneCamera);
        }
    }
}

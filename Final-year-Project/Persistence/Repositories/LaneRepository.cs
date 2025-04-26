using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class LaneRepository : ILaneRepository
    {
        public readonly DeviceServiceContext _context;

        public LaneRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lane>> GetAllAsync()
        {
            return await _context.Lanes
                .Include(l => l.LaneCameras)
                .Include(l => l.LaneControlUnits)
                .ToListAsync();
        }

        public async Task<Lane> GetByIdAsync(int id)
        {
            return await _context.Lanes
                .Include(l => l.LaneCameras)
                .Include(l => l.LaneControlUnits)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task CreateAsync(Lane lane)
        {
            await _context.Lanes.AddAsync(lane);
        }

        public void Update(Lane lane)
        {
            _context.Lanes.Update(lane);
        }

        public void Delete(Lane lane)
        {
            _context.Lanes.Remove(lane);
        }
    }
}

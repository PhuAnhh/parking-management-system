using Final_year_Project.Device.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Repositories
{
    public interface IUnitOfWork
    {
        ICameraRepository Cameras { get; }
        IComputerRepository Computers { get; }
        IControlUnitRepository ControlUnits { get; }
        IGateRepository Gates { get; }
        ILaneRepository Lanes { get; }
        ILaneCameraRepository LaneCameras { get; }
        ILaneControlUnitRepository LaneControlUnits { get; }
        ILedRepository Leds { get; }
        Task<int> SaveChangesAsync();
    }
}

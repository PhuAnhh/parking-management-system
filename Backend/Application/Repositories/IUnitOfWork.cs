﻿using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
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

        ICustomerGroupRepository CustomerGroups { get; }

        ICustomerRepository Customers { get; }

        ICardGroupRepository CardGroups { get; }

        ICardRepository Cards { get; }

        ICardGroupLaneRepository CardGroupLanes { get; }

        IEntryLogRepository EntryLogs { get; }

        IExitLogRepository ExitLogs { get; }

        IRevenueReportRepository RevenueReports { get; }

        IWarningEventRepository WarningEvents { get; }

        IUserRepository Users { get; }

        IRoleRepository Roles { get; }

        IPermissionRepository Permissions { get; }

        IRolePermissionRepository RolePermissions { get; }

        Task<int> SaveChangesAsync();
    }
}

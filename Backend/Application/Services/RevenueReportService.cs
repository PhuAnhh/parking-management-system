using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class RevenueReportService : IRevenueReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RevenueReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RevenueReportDto>> GetAllAsync()
        {
            var reports = await _unitOfWork.RevenueReports.GetAllAsync();
            var reportDtos = new List<RevenueReportDto>();

            foreach (var report in reports)
            {
                reportDtos.Add(new RevenueReportDto
                {
                    Id = report.Id,
                    CardGroupId = report.CardGroupId,
                    ExitCount = report.ExitCount,
                    Revenue = report.Revenue,
                    CreatedAt = report.CreatedAt,
                    UpdatedAt = report.UpdatedAt
                });
            }

            return reportDtos;
        }

        public async Task<RevenueReportDto> GetByIdAsync(int id)
        {
            var report = await _unitOfWork.RevenueReports.GetByIdAsync(id);

            if (report == null) 
                return null;

            return new RevenueReportDto
            {
                Id = report.Id,
                CardGroupId = report.CardGroupId,
                ExitCount = report.ExitCount,
                Revenue = report.Revenue,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt
            };
        }
        public async Task<IEnumerable<RevenueReportDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var reports = await _unitOfWork.RevenueReports.GetByDateRangeAsync(fromDate, toDate);
            var reportDtos = new List<RevenueReportDto>();

            foreach (var report in reports)
            {
                reportDtos.Add(new RevenueReportDto
                {
                    Id = report.Id,
                    CardGroupId = report.CardGroupId,
                    ExitCount = report.ExitCount,
                    Revenue = report.Revenue,
                    CreatedAt = report.CreatedAt,
                    UpdatedAt = report.UpdatedAt
                });
            }

            return reportDtos;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _unitOfWork.RevenueReports.GetByIdAsync(id);

            if (report == null)
                return false;

            _unitOfWork.RevenueReports.Delete(report);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}

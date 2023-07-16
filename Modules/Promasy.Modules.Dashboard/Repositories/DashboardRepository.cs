using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Application.Persistence.Views;
using Promasy.Domain.Orders;
using Promasy.Modules.Dashboard.Dtos;
using Promasy.Modules.Dashboard.Interfaces;

namespace Promasy.Modules.Dashboard.Repositories;

internal class DashboardRepository : IDashboardRepository
{
    private readonly IDatabase _database;

    public DashboardRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task<DashboardCountsDto<int>> GetOrdersCountAsync(int year)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var count = await _database.Orders
            .AsNoTracking()
            .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
            .CountAsync();

        var countPeriod = 0;
        var now = DateTime.UtcNow;
        if (count > 0 && year == now.Year)
        {
            var lastMonth = now.AddMonths(-1);
            countPeriod = await _database.Orders
                .AsNoTracking()
                .Where(o => o.CreatedDate >= lastMonth && o.CreatedDate < now)
                .CountAsync();
        }

        return new DashboardCountsDto<int>(count, countPeriod);
    }

    public async Task<DashboardCountsDto<decimal>> GetFundingLeftAsync(OrderType type, int year)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Expression<Func<FinanceSubDepartmentsView, decimal>> typeSelector = type switch
        {
            OrderType.Material => v => v.LeftMaterials,
            OrderType.Equipment => v => v.LeftEquipment,
            OrderType.Service => v => v.LeftServices,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var count = await _database.FinanceSubDepartmentsWithSpendView
            .AsNoTracking()
            .Where(v => v.CreatedDate >= startDate && v.CreatedDate < endDate)
            .SumAsync(typeSelector);

        decimal countPeriod = 0;
        var now = DateTime.UtcNow;
        if (count > 0 && year == now.Year)
        {
            var lastMonth = now.AddMonths(-1);

            var finSubDepartmentIds = _database.FinanceSubDepartments
                .Where(v => v.CreatedDate >= startDate && v.CreatedDate < endDate)
                .Select(v => v.Id);

            countPeriod = await _database.Orders
                .AsNoTracking()
                .Where(o => o.Type == type)
                .Where(o => o.CreatedDate >= lastMonth && o.CreatedDate < now)
                .Where(o => finSubDepartmentIds.Contains(o.Id))
                .SumAsync(o => o.Total);
        }

        return new DashboardCountsDto<decimal>(count, countPeriod > 0 ? countPeriod * -1 : countPeriod);
    }
}
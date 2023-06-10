namespace Promasy.Modules.Dashboard.Dtos;

public record DashboardCountsDto<T>(T CountTotal, T CountByPeriod);
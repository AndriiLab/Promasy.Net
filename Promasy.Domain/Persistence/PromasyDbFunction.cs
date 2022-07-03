using System;
using Microsoft.EntityFrameworkCore;

namespace Promasy.Domain.Persistence;

#pragma warning disable CS8604

public static class PromasyDbFunction
{
    public static string? GetEmployeeShortName(int employeeId)
        => throw new NotSupportedException("It is DB function");

    public static void ConfigureCustomFunctions(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(
                typeof(PromasyDbFunction).GetMethod(nameof(GetEmployeeShortName), new[] {typeof(int)}))
            .HasName("fn_getemployeeshortname");
    }
}
#pragma warning restore CS8604
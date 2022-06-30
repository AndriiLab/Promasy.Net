using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Employees;
using Promasy.Domain.Finances;
using Promasy.Domain.Manufacturers;
using Promasy.Domain.Orders;
using Promasy.Domain.Organizations;
using Promasy.Domain.Persistence;
using Promasy.Domain.Suppliers;
using Promasy.Domain.Vocabulary;

namespace Promasy.Persistence.Context;

internal class PromasyDatabase : IDatabase
{
    private readonly PromasyContext _ctx;

    public PromasyDatabase(PromasyContext ctx)
    {
        _ctx = ctx;
    }

    public DbSet<Role> Roles => _ctx.Roles;
    public DbSet<Employee> Employees => _ctx.Employees;
    public DbSet<RefreshToken> RefreshTokens => _ctx.RefreshTokens;
    public DbSet<Address> Addresses => _ctx.Addresses;
    public DbSet<Unit> Units => _ctx.Units;
    public DbSet<OrderStatusHistory> OrderStatuses => _ctx.OrderStatuses;
    public DbSet<Order> Orders => _ctx.Orders;
    public DbSet<Cpv> Cpvs => _ctx.Cpvs;
    public DbSet<Department> Departments => _ctx.Departments;
    public DbSet<FinanceDepartment> FinanceDepartments => _ctx.FinanceDepartments;
    public DbSet<FinanceSource> FinanceSources => _ctx.FinanceSources;
    public DbSet<Organization> Organizations => _ctx.Organizations;
    public DbSet<Manufacturer> Manufacturers => _ctx.Manufacturers;
    public DbSet<ReasonForSupplierChoice> ReasonForSupplierChoice => _ctx.ReasonForSupplierChoice;
    public DbSet<SubDepartment> SubDepartments => _ctx.SubDepartments;
    public DbSet<Supplier> Suppliers => _ctx.Suppliers;

    public int SaveChanges() => _ctx.SaveChanges();
    public Task<int> SaveChangesAsync(CancellationToken ct = new()) => _ctx.SaveChangesAsync(ct);
}
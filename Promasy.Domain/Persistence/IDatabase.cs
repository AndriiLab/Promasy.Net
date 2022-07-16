using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Promasy.Domain.Employees;
using Promasy.Domain.Finances;
using Promasy.Domain.Manufacturers;
using Promasy.Domain.Orders;
using Promasy.Domain.Organizations;
using Promasy.Domain.Persistence.Views;
using Promasy.Domain.Suppliers;
using Promasy.Domain.Vocabulary;

namespace Promasy.Domain.Persistence;

public interface IDatabase
{
    DbSet<Role> Roles { get; }
    DbSet<Employee> Employees { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Unit> Units { get; }
    DbSet<OrderStatusHistory> OrderStatuses { get; }
    DbSet<Order> Orders { get; }
    DbSet<Cpv> Cpvs { get; }
    DbSet<Department> Departments { get; }
    DbSet<FinanceSubDepartment> FinanceSubDepartments { get; }
    DbSet<FinanceSource> FinanceSources { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Manufacturer> Manufacturers { get; }
    DbSet<ReasonForSupplierChoice> ReasonForSupplierChoice { get; }
    DbSet<SubDepartment> SubDepartments { get; }
    DbSet<Supplier> Suppliers { get; }

    DbSet<FinanceSubDepartmentsView> FinanceSubDepartmentsWithSpendView { get; }
    DbSet<FinanceSourceView> FinanceSourceWithSpendView { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = new());
    Task<int> SaveChangesAsync(CancellationToken ct = new());
}
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Promasy.Core.Persistence;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Domain.Finances;
using Promasy.Domain.Manufacturers;
using Promasy.Domain.Orders;
using Promasy.Domain.Organizations;
using Promasy.Domain.Persistence;
using Promasy.Domain.Suppliers;
using Promasy.Domain.Vocabulary;
using Z.EntityFramework.Plus;

namespace Promasy.Persistence.Context
{
    public class PromasyContext : DbContext
    {

        public PromasyContext(DbContextOptions<PromasyContext> options, IUserContext userContext) : base(options)
        {
            this.Filter<ISoftDeletable>(q => q.Where(i => !i.Deleted));
            if (userContext?.IsAuthenticated() ?? false)
            {
                this.Filter<Organization>(q => q.Where(o => o.Id == userContext.GetOrganizationId()));
                this.Filter<IOrganizationAssociated>(q => q.Where(i => i.OrganizationId == userContext.GetOrganizationId()));
            }
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<OrderStatusHistory> OrderStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cpv> Cpvs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<FinanceDepartment> FinanceDepartments { get; set; }
        public DbSet<FinanceSource> FinanceSources { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ReasonForSupplierChoice> ReasonForSupplierChoice { get; set; }
        public DbSet<SubDepartment> SubDepartments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromasyContext).Assembly);
            modelBuilder.UseIdentityAlwaysColumns();
            modelBuilder.ConfigureCustomFunctions();
            modelBuilder.HasDefaultSchema("PromasyCore");
        }
    }
}
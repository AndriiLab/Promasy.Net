using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

namespace Promasy.Persistence.Context
{
    public class PromasyContext : DbContext
    {
        private readonly IUserContext? _userContext;

        public PromasyContext(DbContextOptions<PromasyContext> options, IUserContext? userContext) : base(options)
        {
            _userContext = userContext;
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


        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(new CancellationToken());
        }

        public override int SaveChanges()
        {
            UpdateEntities();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            UpdateEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
                                                   CancellationToken cancellationToken = new())
        {
            UpdateEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromasyContext).Assembly);
            modelBuilder.UseIdentityAlwaysColumns();
            modelBuilder.ConfigureCustomFunctions();
            modelBuilder.HasDefaultSchema("PromasyCore");
        }

        private void UpdateEntities()
        {
            foreach (var e in ChangeTracker.Entries().Where(e => e.Entity is IEntity))
            {
                var entity = (IEntity) e.Entity;
                switch (e.State)
                {
                    case EntityState.Added:
                        entity.CreatedDate = DateTime.UtcNow;
                        entity.CreatorId = _userContext?.Id ?? 0;
                        break;
                    case EntityState.Modified:
                        entity.ModifiedDate = DateTime.UtcNow;
                        entity.ModifierId = _userContext?.Id;
                        break;
                    case EntityState.Deleted:
                        if (e.Entity is ISoftDeletable sd)
                        {
                            e.State = EntityState.Modified;
                            entity.ModifiedDate = DateTime.UtcNow;
                            entity.ModifierId = _userContext?.Id;
                            sd.Deleted = true;
                        }
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        break;
                }
            }
        }
    }
}
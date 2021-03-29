using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Promasy.Common.Persistence;
using Promasy.Domain.Bids;
using Promasy.Domain.Finances;
using Promasy.Domain.Institutes;
using Promasy.Domain.Producers;
using Promasy.Domain.Suppliers;
using Promasy.Domain.Users;
using Promasy.Domain.Vocabulary;

namespace Promasy.Persistence.Context
{
    public class PromasyContext : IdentityDbContext<Employee, Role, int>, IPersistedGrantDbContext
    {
        public PromasyContext(DbContextOptions<PromasyContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<AmountUnit> AmountUnits { get; set; }
        public DbSet<BidStatusHistory> BidStatuses { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Cpv> Cpvs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<FinanceDepartment> FinanceDepartments { get; set; }
        public DbSet<FinanceSource> FinanceSources { get; set; }
        public DbSet<Institute> Institutes { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<ReasonForSupplier> ReasonForSuppliers { get; set; }
        public DbSet<SubDepartment> SubDepartments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }


        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(new CancellationToken());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromasyContext).Assembly);
            modelBuilder.UseIdentityAlwaysColumns();
            modelBuilder.HasDefaultSchema("PromasyCore");
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

        private void UpdateEntities()
        {
            foreach (var e in ChangeTracker.Entries().Where(e => e.Entity is IEntity))
            {
                var entity = (IEntity) e.Entity;
                switch (e.State)
                {
                    case EntityState.Added:
                        entity.CreatedDate = DateTime.UtcNow;
                        //todo: entry.CreatedBy = ;
                        break;
                    case EntityState.Modified:
                        entity.ModifiedDate = DateTime.UtcNow;
                        //todo: entity.ModifiedBy = ;
                        break;
                    case EntityState.Deleted:
                        e.State = EntityState.Modified;
                        entity.ModifiedDate = DateTime.UtcNow;
                        //todo: entity.ModifiedBy = ;
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
using Microsoft.EntityFrameworkCore;
using Promasy.Persistence.Dao.Bids;
using Promasy.Persistence.Dao.Finances;
using Promasy.Persistence.Dao.Institutes;
using Promasy.Persistence.Dao.Internals;
using Promasy.Persistence.Dao.Producers;
using Promasy.Persistence.Dao.Suppliers;
using Promasy.Persistence.Dao.Users;
using Promasy.Persistence.Dao.Vocabulary;

namespace Promasy.Persistence.Context
{
    public class PromasyContext : DbContext
    {
        public PromasyContext()
        {
        }

        public PromasyContext(DbContextOptions<PromasyContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<AmountUnit> AmountUnits { get; set; }
        public DbSet<BidStatus> BidStatuses { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Cpv> Cpvs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<FinanceDepartment> FinanceDepartments { get; set; }
        public DbSet<FinanceSource> FinanceSources { get; set; }
        public DbSet<Institute> Institutes { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<ReasonForSupplier> ReasonForSuppliers { get; set; }
        public DbSet<RegistrationsLeft> Registrations { get; set; }
        public DbSet<Subdepartment> Subdepartments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Version> Versions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence("hilo_seqeunce");
        }
    }
}
